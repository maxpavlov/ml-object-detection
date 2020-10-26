using System;

namespace waoeml.Providers.Common
{
    public class PredictionConfig
    {
        public string ModelProvider => Environment.GetEnvironmentVariable("MODEL_PROVIDER") ?? throw new ArgumentNullException("MODEL_PROVIDER cannot be null");
        public string ModelFile => Environment.GetEnvironmentVariable("MODEL_FILE") ?? throw new ArgumentNullException("MODEL_FILE cannot be null");
        public int MaxHeight => 480;
    }
}