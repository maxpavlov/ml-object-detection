using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using waoeml.Providers;
using waoeml.Providers.Common;

namespace waoeml.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    public class ClassifyController : ControllerBase
    {
        private readonly IPredictionProvider provider;
        private readonly ILogger<ClassifyController> logger;

        public ClassifyController(IPredictionProvider provider, ILogger<ClassifyController> logger)
        {
            this.provider = provider;
            this.logger = logger;
        }

        [HttpGet("categories")]
        public IEnumerable<string> Categories()
        {
            return provider.GetCategories();
        }

        [HttpPost("classify")]
        public async Task<PredictionSummaryResponse> Classify(IFormFile file)
        {
            this.logger.LogInformation("Classifying {}", file.FileName);
            var summary = this.provider.Predict(await file.AsBitMapAsync());
            return summary.AsResponse();
        }
    }
}
