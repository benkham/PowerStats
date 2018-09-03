using Microsoft.Extensions.Logging;
using PowerStats.API.Interfaces;
using PowerStats.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PowerStats.API.Services
{
    public class PowerStatisticsService : IPowerStatisticsService
    {
        private readonly ILogger<PowerStatisticsService> _logger;

        public PowerStatisticsService(ILogger<PowerStatisticsService> logger)
        {
            _logger = logger;
        }

        public async Task<IList<PowerStatisticsModel>> ProcessPowerStatistics(string dataFilePath,
            string dataFileExtension, int medianTolerancePercentage)
        {
            var powerStatisticsList = new List<PowerStatisticsModel>();

            try
            {
                // get all the data files from the data path
                var csvFiles = GetDataFiles(dataFilePath, dataFileExtension);

                // compute statistics for each file and add to the collection
                foreach (FileInfo fi in csvFiles)
                {
                    powerStatisticsList.AddRange(ComputePowerStatistics(fi, medianTolerancePercentage));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return await Task.FromResult(powerStatisticsList);
        }

        public IList<FileInfo> GetDataFiles(string dataFilePath, string dataFileExtension)
        {
            IList<FileInfo> fileList = new List<FileInfo>();

            try
            {
                // take a snapshot of the file system
                DirectoryInfo dir = new DirectoryInfo(dataFilePath);

                fileList = dir.GetFiles("*.*", SearchOption.AllDirectories).ToList();

                // create files query  
                IEnumerable<FileInfo> fileQuery =
                    (from file in fileList
                     where file.Extension == dataFileExtension
                     orderby file.Name
                     select file).AsEnumerable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return fileList;
        }

        public IList<PowerStatisticsModel> ComputePowerStatistics(FileInfo fileInfo, int medianTolerancePercentage)
        {
            IList<PowerStatisticsModel> powerStatisticsList = new List<PowerStatisticsModel>();

            try
            {
                // read from the csv data source  
                string[] csvLines = File.ReadAllLines(fileInfo.FullName);

                // sort the power data list 
                var powerStatisticsSortedList = (from line in csvLines.Skip(1)
                                                 let splitLine = line.Split(',')
                                                 // sort by DataValue/Energy column
                                                 orderby Convert.ToDecimal(splitLine[5])
                                                 select new PowerStatisticsModel()
                                                 {
                                                     FileName = fileInfo.Name,
                                                     ConsumptionDate = Convert.ToDateTime(splitLine[3]),
                                                     Value = Convert.ToDecimal(splitLine[5]),
                                                     MedianValue = Convert.ToDecimal(0)
                                                 }).ToList();

                // compute median value
                decimal median = ComputeMedian(powerStatisticsSortedList);

                // update statistics with median value
                powerStatisticsSortedList = powerStatisticsSortedList.Select(c => { c.MedianValue = median; return c; }).ToList();

                // filter percentage above or below the median value
                powerStatisticsList = ComputeMedianRange(powerStatisticsSortedList, median, medianTolerancePercentage);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return powerStatisticsList;
        }

        public decimal ComputeMedian(IList<PowerStatisticsModel> sortedList)
        {
            int count = sortedList.Count();
            int itemIndex = count / 2;

            // even number of items
            if (count % 2 == 0)
            {
                return (sortedList.ElementAt(itemIndex).Value + sortedList.ElementAt(itemIndex - 1).Value) / 2;
            }

            // odd number of items 
            return sortedList.ElementAt(itemIndex).Value;
        }

        public IList<PowerStatisticsModel> ComputeMedianRange(IList<PowerStatisticsModel> statisticsList,
            decimal median, int tolerancePercentage)
        {
            decimal medianRangePercentage = (median * tolerancePercentage) / 100;

            // query to filter percentage above or below the median value
            var result = from statistics in statisticsList
                         .Where(m => m.Value >= median - medianRangePercentage
                            && m.Value <= median + medianRangePercentage)
                         select statistics;

            return result.ToList();
        }
    }
}
