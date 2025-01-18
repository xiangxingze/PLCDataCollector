using Microsoft.EntityFrameworkCore;
using PLCDataCollector.API.Data;
using PLCDataCollector.API.Models;
using PLCDataCollector.API.Enums;

public class AlarmStatisticsService
{
    private readonly PLCDataContext _context;

    public AlarmStatisticsService(PLCDataContext context)
    {
        _context = context;
    }

    public async Task<AlarmStatistics> GetStatisticsAsync(DateTime startTime, DateTime endTime)
    {
        var alarms = await _context.AlarmRecords
            .Where(a => a.Timestamp >= startTime && a.Timestamp <= endTime)
            .ToListAsync();

        return new AlarmStatistics
        {
            TotalAlarms = alarms.Count,
            UnacknowledgedAlarms = alarms.Count(a => !a.IsAcknowledged),
            HighLimitAlarms = alarms.Count(a => a.Type == AlarmTypeEnum.HighLimit),
            LowLimitAlarms = alarms.Count(a => a.Type == AlarmTypeEnum.LowLimit),
            DeviceStatistics = alarms.GroupBy(a => a.DeviceName)
                .Select(g => new DeviceAlarmStatistics
                {
                    DeviceName = g.Key,
                    AlarmCount = g.Count()
                }).ToList()
        };
    }
}

public class AlarmStatistics
{
    public int TotalAlarms { get; set; }
    public int UnacknowledgedAlarms { get; set; }
    public int HighLimitAlarms { get; set; }
    public int LowLimitAlarms { get; set; }
    public List<DeviceAlarmStatistics> DeviceStatistics { get; set; }
}

public class DeviceAlarmStatistics
{
    public string DeviceName { get; set; }
    public int AlarmCount { get; set; }
} 