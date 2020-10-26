using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using waoeml.Providers.Common;

namespace waoeml.Providers
{
    public static class ProviderExtensions
    {
        public static IServiceCollection AddPredictionProvider(this IServiceCollection services)
        {
            // Register config
            var config = new PredictionConfig();
            services.AddSingleton(config);

            // Resolve provider
            var requiredInterfaceType = typeof(IPredictionProvider);
            var requiredAttributeType = typeof(ProviderForAttribute);

            var providerType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => requiredInterfaceType.IsAssignableFrom(t))
                .Where(t => t.GetCustomAttributes(requiredAttributeType)
                    .Cast<ProviderForAttribute>()
                    .Any(a => config.ModelProvider == a.GetName()))
                .Single();

            services.AddSingleton(requiredInterfaceType, providerType);
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