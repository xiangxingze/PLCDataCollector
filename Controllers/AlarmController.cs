using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using PLCDataCollector.API.Data;
using PLCDataCollector.API.Models;
using PLCDataCollector.API.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AlarmController : ControllerBase
{
    private readonly PLCDataContext _context;
    private readonly AlarmService _alarmService;
    private readonly AlarmStatisticsService _statisticsService;

    public AlarmController(
        PLCDataContext context, 
        AlarmService alarmService,
        AlarmStatisticsService statisticsService)
    {
        _context = context;
        _alarmService = alarmService;
        _statisticsService = statisticsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlarmRecord>>> GetAlarms()
    {
        return await _context.AlarmRecords
            .OrderByDescending(a => a.Timestamp)
            .Take(100)
            .ToListAsync();
    }

    [Authorize(Roles = "Administrator,Operator")]
    [HttpPost("{id}/acknowledge")]
    public async Task<IActionResult> AcknowledgeAlarm(int id)
    {
        await _alarmService.AcknowledgeAlarm(id);
        return Ok();
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportAlarms([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime)
    {
        var query = _context.AlarmRecords.AsQueryable();
        
        if (startTime.HasValue)
            query = query.Where(a => a.Timestamp >= startTime.Value);
        
        if (endTime.HasValue)
            query = query.Where(a => a.Timestamp <= endTime.Value);

        var alarms = await query.OrderByDescending(a => a.Timestamp).ToListAsync();

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        csv.WriteRecords(alarms);
        writer.Flush();
        
        return File(memoryStream.ToArray(), "text/csv", $"alarms_{DateTime.Now:yyyyMMddHHmmss}.csv");
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<AlarmStatistics>> GetStatistics(
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
    {
        var statistics = await _statisticsService.GetStatisticsAsync(startTime, endTime);
        return Ok(statistics);
    }
} 