using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PowerStats.API;
using PowerStats.API.Controllers;
using PowerStats.API.Interfaces;
using PowerStats.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PowerStats.UnitTests
{
    [Trait("Category", "Controllers")]
    public class PowerStatisticsControllerTests
    {
        private readonly Mock<IOptions<PowerStatisticsSettings>> _configurationMock;
        private readonly Mock<ILogger<PowerStatisticsController>> _loggerMock;
        private readonly Mock<IPowerStatisticsService> _powerStatisticsMock;

        public PowerStatisticsControllerTests()
        {
            _configurationMock = new Mock<IOptions<PowerStatisticsSettings>>();
            _loggerMock = new Mock<ILogger<PowerStatisticsController>>();
            _powerStatisticsMock = new Mock<IPowerStatisticsService>();

            var settings = new PowerStatisticsSettings
            {
                DataFilesPath = "data",
                DataFileExtension = ".csv",
                MedianTolerancePercentage = 20
            };
            _configurationMock.Setup(x => x.Value).Returns(settings);

            _powerStatisticsMock.Setup(s => s.ProcessPowerStatistics(
                _configurationMock.Object.Value.DataFilesPath,
                _configurationMock.Object.Value.DataFileExtension,
                 _configurationMock.Object.Value.MedianTolerancePercentage))
                 .Returns(GetPowerStatistics());
        }

        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            //Arrange
            var controller = new PowerStatisticsController(
                _configurationMock.Object,
                _loggerMock.Object,
                _powerStatisticsMock.Object);

            // Act
            var okResult = controller.Get();

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsListOfPowerStatisticsModel()
        {
            //Arrange
            var controller = new PowerStatisticsController(
                _configurationMock.Object,
                _loggerMock.Object, 
                _powerStatisticsMock.Object);

            // Act
            var result = controller.Get().Result as OkObjectResult;

            // Assert
            var items = Assert.IsAssignableFrom<IList<PowerStatisticsModel>>(result.Value);
            Assert.Equal(2, items.Count);
            Assert.Equal("One.csv", items[0].FileName);
        }

        private Task<IList<PowerStatisticsModel>> GetPowerStatistics()
        {
            IList<PowerStatisticsModel> statistics = new List<PowerStatisticsModel>()
            {
                new PowerStatisticsModel(){ FileName = "One.csv", ConsumptionDate = new DateTime(2018, 1, 1),
                    Value = new Decimal(1.5), MedianValue = new Decimal(1.25)},
                new PowerStatisticsModel(){ FileName = "Two.csv", ConsumptionDate = new DateTime(2018, 1, 1),
                    Value = new Decimal(2.5), MedianValue = new Decimal(2.25)}
            };

            return Task.FromResult(statistics);
        }
    }
}
