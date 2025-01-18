using Microsoft.AspNetCore.Mvc;
using PLCDataCollector.API.Models;
using PLCDataCollector.API.Services;

namespace PLCDataCollector.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.LoginAsync(model.Username, model.Password);
            if (result.Succeeded)
            {
                return Ok(new { Token = result.Token });
            }
            return Unauthorized();
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
} 