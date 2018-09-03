namespace PowerStats.API
{
    public class PowerStatisticsSettings
    {
        public PowerStatisticsSettings()
        {
            // set default value
            DataFileExtension = ".csv";
            MedianTolerancePercentage = 20;
        }

        public string DataFilesPath { get; set; }
        public string DataFileExtension { get; set; }
        public int MedianTolerancePercentage { get; set; }
    }
}
