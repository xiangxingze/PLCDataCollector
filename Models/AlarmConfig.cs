namespace PLCDataCollector.API.Models
{
    public class AlarmConfig
    {
        public string DeviceName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public double HighLimit { get; set; }
        public double LowLimit { get; set; }
        public bool Enabled { get; set; }
    }
} 