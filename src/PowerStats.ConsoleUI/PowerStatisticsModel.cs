using System;

namespace PowerStats.ConsoleUI
{
    public class PowerStatisticsModel
    {
        public string FileName { get; set; }
        public DateTime ConsumptionDate { get; set; }
        public decimal Value { get; set; }
        public decimal MedianValue { get; set; }
    }
}
