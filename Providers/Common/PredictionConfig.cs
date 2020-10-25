using System;

namespace waoeml.Providers.Common
{
    public class PredictionConfig
    {
        public string ModelFile => Environment.GetEnvironmentVariable("MODEL_FILE");
        public int MaxHeight => 480;
    }
}