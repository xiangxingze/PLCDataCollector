using PLCDataCollector.API.Enums;

namespace PLCDataCollector.API.Models
{
    public class AlarmRecord
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public double Limit { get; set; }
        public AlarmTypeEnum Type { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsAcknowledged { get; set; }
    }
} 