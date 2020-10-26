using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using waoeml.Providers.Common;
using waoeml.Providers.YoloV4.Models;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;

namespace waoeml.Providers.YoloV4
{
    [ProviderFor("YOLOV4")]
    public class YoloV4PredictionProvider : IPredictionProvider
    {
        private readonly PredictionEngine<YoloV4BitmapData, YoloV4Prediction> predictionEngine;
        private readonly string[] allCategories = new string[] { "person", "bicycle", "car", "motorbike", "aeroplane", "bus", "train", "truck", "boat", "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "sofa", "pottedplant", "bed", "diningtable", "toilet", "tvmonitor", "laptop", "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush" };
        private readonly string[] badCategories = new string[] { "person", "bicycle", "car", "motorbike", "bus", "truck" };
        private readonly string[] animalCategories = new string[] { "bird", "cat", "dog", "horse", "sheep", "cow", "elephant", "bear" };
        private readonly string[] validCategories;
        private readonly PredictionConfig predictionConfig;
        private readonly ILogger<YoloV4PredictionProvider> logger;
        private readonly Pen boxPen = new Pen(Color.FromArgb(255, 0, 0, 255), 5);

        public YoloV4PredictionProvider(PredictionConfig predictionConfig, ILogger<YoloV4PredictionProvider> logger)
        {
            this.predictionConfig = predictionConfig;
            this.logger = logger;
            this.validCategories = badCategories.Concat(animalCategories).ToArray();

            this.logger.LogInformation("Modelfile is {}", predictionConfig.ModelFile);

            var mlContext = new MLContext();
            var pipeline = mlContext.Transforms.ResizeImages(inputColumnName: "bitmap", outputColumnName: "input_1:0", imageWidth: 416, imageHeight: 416, resizing: ResizingKind.IsoPad)
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input_1:0", scaleImage: 1f / 255f, interleavePixelColors: true))
                .Append(mlContext.Transforms.ApplyOnnxModel(
                    shapeDictionary: new Dictionary<string, int[]>()
                    {
                        { "input_1:0", new[] { 1, 416, 416, 3 } },
                        { "Identity:0", new[] { 1, 52, 52, 3, 85 } },
                        { "Identity_1:0", new[] { 1, 26, 26, 3, 85 } },
                        { "Identity_2:0", new[] { 1, 13, 13, 3, 85 } },
                    },
                    inputColumnNames: new[]
                    {
                        "input_1:0"
                    },
                    outputColumnNames: new[]
                    {
                        "Identity:0",
                        "Identity_1:0",
                        "Identity_2:0"
                    },
                    modelFile: predictionConfig.ModelFile));
            this.logger.LogInformation("Pipline ready");

            // Fit on empty list to obtain input data schema
            var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<YoloV4BitmapData>()));

            // Create prediction engine
            this.predictionEngine = mlContext.Model.CreatePredictionEngine<YoloV4BitmapData, YoloV4Prediction>(model);
            this.logger.LogInformation("PredictionEngine ready");
        }

        public IEnumerable<string> GetCategories()
        {
            return this.validCategories.OrderBy(l => l);
        }

        public PredictionSummary Predict(Bitmap image)
        {
            // Predict
            this.logger.LogInformation("Prediction started");
            var stopWatch = Stopwatch.StartNew();
            var predict = predictionEngine.Predict(new YoloV4BitmapData() { Image = image });
            var results = predict.GetResults(allCategories, 0.3f, 0.7f).Where(r => validCategories.Contains(r.Label)).ToList();
            stopWatch.Stop();
            this.logger.LogInformation("Prediction ended. Got {} results in {}ms.", results.Count, stopWatch.ElapsedMilliseconds);

            // Draw boxes
            using (var g = Graphics.FromImage(image))
            {
                foreach (var res in results)
                {
                    var x1 = res.BBox[0];
                    var y1 = res.BBox[1];
                    var x2 = res.BBox[2];
                    var y2 = res.BBox[3];
                    
                    g.DrawRectangle(boxPen, x1, y1, x2 - x1, y2 - y1);

                    // using (var brushes = new SolidBrush(Color.FromArgb(50, Color.Red)))
                    // {
                    //     g.FillRectangle(brushes, x1, y1, x2 - x1, y2 - y1);
                    // }
                    // g.DrawString($"{res.Label} {(int)res.Confidence * 100}", this.font, Brushes.Blue, new PointF(x1, y1));
                }
            }

            return new PredictionSummary
            {
                Image = image.Resize(predictionConfig.MaxHeight).AsBase64(),
                Results = results.Select(r => new PredictionResult
                {
                    Category = r.Label,
                    Score = (int)(r.Confidence * 100)
                })
            };
        }
    }
}