using System.Collections.Generic;
using System.Linq;

namespace waoeml.Providers.Common
{
    public class PredictionSummary
    {
        public string Image { get; set; }
        public IEnumerable<PredictionResult> Results { get; set; } = Enumerable.Empty<PredictionResult>();
    }
}