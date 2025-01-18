namespace PLCDataCollector.API.Models
{
    public class CollectedData
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public string DataType { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 