using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerStats.API.Interfaces;
using PowerStats.API.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PowerStats.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerStatisticsController : Controller
    {
        private readonly IOptions<PowerStatisticsSettings> _configuration;
        private readonly ILogger<PowerStatisticsController> _logger;
        private readonly IPowerStatisticsService _powerStatistics;

        public PowerStatisticsController(IOptions<PowerStatisticsSettings> configuration,
            ILogger<PowerStatisticsController> logger,
            IPowerStatisticsService powerStatistics)
        {
            _configuration = configuration;
            _logger = logger;
            _powerStatistics = powerStatistics;
        }

        // GET api/powerstatistics
        [HttpGet]
        [ProducesResponseType(typeof(IList<PowerStatisticsModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            var statistics = await _powerStatistics.ProcessPowerStatistics(
                _configuration.Value.DataFilesPath,
                _configuration.Value.DataFileExtension,
                _configuration.Value.MedianTolerancePercentage);

            return Ok(statistics);
        }
    }
}