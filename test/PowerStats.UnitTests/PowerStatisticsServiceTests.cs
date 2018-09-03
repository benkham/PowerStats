using Microsoft.Extensions.Logging;
using Moq;
using PowerStats.API;
using PowerStats.API.Models;
using PowerStats.API.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace PowerStats.UnitTests
{
    [Trait("Category", "Services")]
    public class PowerStatisticsServiceTests
    {
        private readonly Mock<ILogger<PowerStatisticsService>> _loggerMock;
        private readonly PowerStatisticsSettings _settings;

        public PowerStatisticsServiceTests()
        {
            _loggerMock = new Mock<ILogger<PowerStatisticsService>>();

            _settings = new PowerStatisticsSettings
            {
                DataFilesPath = "C:\\PROJECTS\\PowerStats\\src\\PowerStats.API\\data",
                DataFileExtension = ".csv",
                MedianTolerancePercentage = 20
            };
        }

        [Fact]
        public async void ProcessPowerStatistics_DataFilesPathExtensionMedianPercentagePassed_ReturnsListOfPowerStatisticsModelAsync()
        {
            //Arrange
            var service = new PowerStatisticsService(_loggerMock.Object);
            var expectedMedian = new decimal(0);

            // Act
            var statistics = await service.ProcessPowerStatistics(_settings.DataFilesPath,
                _settings.DataFileExtension, _settings.MedianTolerancePercentage);

            // Assert
            Assert.Equal(623, statistics.Count);
            Assert.Equal(expectedMedian, statistics[0].MedianValue);
        }

        [Fact]
        public void GetDataFiles_DataFilesPathExtensionPassed_ReturnsEnumerableOfFileInfo()
        {
            //Arrange
            var service = new PowerStatisticsService(_loggerMock.Object);

            // Act
            var files = service.GetDataFiles(_settings.DataFilesPath, _settings.DataFileExtension);

            // Assert
            Assert.Equal(".csv", files[0].Extension);
            Assert.Equal(6, files.Count);
        }

        [Fact]
        public void ComputePowerStatistics_FileInfoMedianRangePassed_ReturnsListOfPowerStatisticsModel()
        {
            //Arrange
            var service = new PowerStatisticsService(_loggerMock.Object);
            var file = new FileInfo(@_settings.DataFilesPath + "\\LP_214612653_20150907T220027915.csv");
            var expectedFirstValue = new decimal(1.52);
            var expectedMedian = new decimal(1.89);

            // Act
            var statistics = service.ComputePowerStatistics(file, 20);

            // Assert
            Assert.Equal(101, statistics.Count);
            Assert.Equal(expectedFirstValue, statistics[0].Value);
            Assert.Equal(expectedMedian, statistics[0].MedianValue);
        }

        [Fact]
        public void ComputeMedian_PowerStatisticsModelSortedListPassed_ReturnsMedianOfDecimal()
        {
            //Arrange
            var service = new PowerStatisticsService(_loggerMock.Object);
            var statisticsList = GetPowerStatisticsList();
            var expectedMedian = new decimal(1.25);

            // Act
            var median = service.ComputeMedian(statisticsList);

            // Assert
            Assert.Equal(expectedMedian, median);
        }

        [Fact]
        public void ComputeMedianRange_PowerStatisticsListMedianRangePassed_ReturnsListOfPowerStatisticsModel()
        {
            //Arrange
            var service = new PowerStatisticsService(_loggerMock.Object);
            var statisticsList = GetPowerStatisticsList();
            var median = new decimal(1.25);
            var expectedFirstValue = new decimal(1.1);

            // Act
            var medianRange = service.ComputeMedianRange(statisticsList, median, 20);

            // Assert
            Assert.Equal(3, medianRange.Count);
            Assert.Equal(expectedFirstValue, medianRange[0].Value);
        }

        private IList<PowerStatisticsModel> GetPowerStatisticsList()
        {
            IList<PowerStatisticsModel> statistics = new List<PowerStatisticsModel>()
            {
                new PowerStatisticsModel(){ FileName = "One.csv", ConsumptionDate = new DateTime(2018, 1, 1),
                    Value = new decimal(-1), MedianValue = new decimal(0)},
                new PowerStatisticsModel(){ FileName = "One.csv", ConsumptionDate = new DateTime(2018, 1, 1),
                    Value = new decimal(1.1), MedianValue = new decimal(0)},
                new PowerStatisticsModel(){ FileName = "One.csv", ConsumptionDate = new DateTime(2018, 1, 1),
                    Value = new decimal(1.25), MedianValue = new decimal(0)},
                new PowerStatisticsModel(){ FileName = "One.csv", ConsumptionDate = new DateTime(2018, 1, 1),
                    Value = new decimal(1.4), MedianValue = new decimal(0)},
                new PowerStatisticsModel(){ FileName = "One.csv", ConsumptionDate = new DateTime(2018, 1, 1),
                    Value = new decimal(2), MedianValue = new decimal(0)}
            };

            return statistics;
        }
    }
}
