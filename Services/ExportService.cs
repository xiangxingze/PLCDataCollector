using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.Configuration;
using PLCDataCollector.API.Data;

public class ExportService
{
    private readonly PLCDataContext _context;

    public ExportService(PLCDataContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportDataToCsv(DateTime? startTime = null, DateTime? endTime = null)
    {
        var query = _context.CollectedData.AsQueryable();
        
        if (startTime.HasValue)
            query = query.Where(d => d.Timestamp >= startTime.Value);
        
        if (endTime.HasValue)
            query = query.Where(d => d.Timestamp <= endTime.Value);

        var data = await query.OrderByDescending(d => d.Timestamp).ToListAsync();

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ","
        });

        // 写入标题
        csv.WriteHeader<CollectedDataExport>();
        await csv.NextRecordAsync();

        // 写入数据
        foreach (var item in data)
        {
            var export = new CollectedDataExport
            {
                设备名称 = item.DeviceName,
                地址 = item.Address,
                数值 = item.Value.ToString(),
                时间 = item.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                描述 = item.Description
            };
            csv.WriteRecord(export);
            await csv.NextRecordAsync();
        }

        await writer.FlushAsync();
        return memoryStream.ToArray();
    }
}

public class CollectedDataExport
{
    public string 设备名称 { get; set; }
    public string 地址 { get; set; }
    public string 数值 { get; set; }
    public string 时间 { get; set; }
    public string 描述 { get; set; }
} 