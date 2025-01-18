using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PLCDataCollector.API.Data;
using PLCDataCollector.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PLCDataController : ControllerBase
{
    private readonly PLCDataContext _context;
    private readonly ExportService _exportService;

    public PLCDataController(PLCDataContext context, ExportService exportService)
    {
        _context = context;
        _exportService = exportService;
    }

    /// <summary>
    /// 获取最新的PLC数据
    /// </summary>
    /// <returns>最新的100条数据记录</returns>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(IEnumerable<CollectedData>), 200)]
    public async Task<ActionResult<IEnumerable<CollectedData>>> GetLatestData()
    {
        var data = await _context.CollectedData
            .OrderByDescending(d => d.Timestamp)
            .Take(100)
            .ToListAsync();
        return Ok(data);
    }

    /// <summary>
    /// 获取指定设备的数据
    /// </summary>
    /// <param name="deviceName">设备名称</param>
    /// <returns>指定设备的最新100条数据记录</returns>
    [HttpGet("device/{deviceName}")]
    [ProducesResponseType(typeof(IEnumerable<CollectedData>), 200)]
    public async Task<ActionResult<IEnumerable<CollectedData>>> GetDeviceData(string deviceName)
    {
        var data = await _context.CollectedData
            .Where(d => d.DeviceName == deviceName)
            .OrderByDescending(d => d.Timestamp)
            .Take(100)
            .ToListAsync();
        return Ok(data);
    }

    /// <summary>
    /// 导出数据为CSV格式
    /// </summary>
    /// <returns>CSV文件</returns>
    [HttpGet("export")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportData([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime)
    {
        var fileBytes = await _exportService.ExportDataToCsv(startTime, endTime);
        var fileName = $"PLC数据_{DateTime.Now:yyyyMMddHHmmss}.csv";
        return File(fileBytes, "text/csv", fileName);
    }
} 