using System;
using System.IO;

namespace waoeml.Providers.Common
{
    public class PredictionConfig
    {
        public string ModelProvider => Environment.GetEnvironmentVariable("MODEL_PROVIDER") ?? "YOLOV4";
        public string ModelFile => Environment.GetEnvironmentVariable("MODEL_FILE") ?? Path.GetFullPath("temp/yolov4/yolov4.onnx");
        public int MaxHeight => 480;
    }
}