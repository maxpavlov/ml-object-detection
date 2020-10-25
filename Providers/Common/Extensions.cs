using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using waoeml.Providers.Common;
using waoeml.Providers.YoloV4;

namespace waoeml.Providers
{
    public static class Extensions
    {
        public static IServiceCollection AddPredictionProvider(this IServiceCollection services)
        {
            // Register config
            services.AddSingleton<PredictionConfig>();

            // Resolve provider
            string provider = Environment.GetEnvironmentVariable("MODEL_PROVIDER");
            switch (provider)
            {
                case "YOLOV4":
                    services.AddSingleton<IPredictionProvider, YoloV4PredictionProvider>();
                    break;
                default:
                    throw new NotImplementedException($"Provider:{provider} has no implementation.");
            }

            return services;
        }

        public static async Task<Bitmap> AsBitMapAsync(this IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return new Bitmap(ms);
        }

        public static string AsBase64(this Bitmap bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            return "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
        }

        public static Bitmap Resize(this Bitmap bitmap, int maxHeight)
        {
            var ratio = (double)maxHeight / bitmap.Height;
            var width = (int)(bitmap.Width * ratio);
            var height = (int)(bitmap.Height * ratio);
            var resized = new Bitmap(width, height);

            using (var g = Graphics.FromImage(resized))
            {
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingMode = CompositingMode.SourceCopy;
                g.DrawImage(bitmap, 0, 0, width, height);
            }

            return resized;
        }

        public static PredictionSummaryResponse AsResponse(this PredictionSummary summary)
        {
            return new PredictionSummaryResponse { Summary = summary };
        }
    }
}