using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PLCDataCollector.API.Data;
using PLCDataCollector.API.Models;
using PLCDataCollector.Models;

namespace PLCDataCollector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataCollectionController : ControllerBase
    {
        private readonly PLCDataContext _context;

        public DataCollectionController(PLCDataContext context)
        {
            _context = context;
        }

        // GET: api/DataCollection
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CollectedData>>> GetData([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = _context.CollectedData.AsQueryable();
            
            if (from.HasValue)
                query = query.Where(d => d.Timestamp >= from.Value);
            
            if (to.HasValue)
                query = query.Where(d => d.Timestamp <= to.Value);

            return await query.OrderByDescending(d => d.Timestamp)
                            .Take(1000)
                            .ToListAsync();
        }

        // GET: api/DataCollection/device/{deviceName}
        [HttpGet("device/{deviceName}")]
        public async Task<ActionResult<IEnumerable<CollectedData>>> GetDeviceData(string deviceName)
        {
            return await _context.CollectedData
                .Where(d => d.DeviceName == deviceName)
                .OrderByDescending(d => d.Timestamp)
                .Take(100)
                .ToListAsync();
        }
    }
} 