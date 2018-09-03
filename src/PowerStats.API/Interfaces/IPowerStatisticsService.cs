using PowerStats.API.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PowerStats.API.Interfaces
{
    public interface IPowerStatisticsService
    {
        Task<IList<PowerStatisticsModel>> ProcessPowerStatistics(string dataFilePath,
            string dataFileExtension, int medianTolerancePercentage);

        IList<FileInfo> GetDataFiles(string dataFilePath, string dataFileExtension);

        IList<PowerStatisticsModel> ComputePowerStatistics(FileInfo fileInfo, int medianTolerancePercentage);

        decimal ComputeMedian(IList<PowerStatisticsModel> sortedList);

        IList<PowerStatisticsModel> ComputeMedianRange(IList<PowerStatisticsModel> statisticsList,
            decimal median, int tolerancePercentage);
    }
}
