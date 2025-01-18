using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PLCDataCollector.API.Data;
using PLCDataCollector.Models;

namespace PLCDataCollector.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PLCDeviceController : ControllerBase
    {
        private readonly PLCDataContext _context;

        public PLCDeviceController(PLCDataContext context)
        {
            _context = context;
        }

        // GET: api/PLCDevice
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PLCDevice>>> GetDevices()
        {
            return await _context.PLCDevices.ToListAsync();
        }

        // GET: api/PLCDevice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PLCDevice>> GetDevice(int id)
        {
            var device = await _context.PLCDevices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return device;
        }

        // POST: api/PLCDevice
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<PLCDevice>> CreateDevice(PLCDevice device)
        {
            _context.PLCDevices.Add(device);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDevice), new { id = device.Id }, device);
        }

        // PUT: api/PLCDevice/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateDevice(int id, PLCDevice device)
        {
            if (id != device.Id)
            {
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/PLCDevice/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.PLCDevices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.PLCDevices.Remove(device);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
} 