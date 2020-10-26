using System.Collections.Generic;
using System.Drawing;

namespace waoeml.Providers.Common
{
    public interface IPredictionProvider
    {
        PredictionSummary Predict(Bitmap image);
        IEnumerable<string> GetCategories();
    }
}