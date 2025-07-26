using Backend.DTOs.Users;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Tags("Authentication")]
    public class AuthController(IUserServices _userService) : ControllerBase
    {
        /// <summary>
        /// Authenticate user and return JWT token if credentials are valid.
        /// </summary>
        /// <param name="dto">Login credentials</param>
        /// <returns>JWT token or Unauthorized</returns>
        /// <response code="200">Returns JWT token</response>
        /// <response code="401">Invalid username or password</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            Log.Information("Login attempt for username: {Username}", dto.Username);

            var token = await _userService.AuthenticateAsync(dto.Username, dto.Password);

            if (token == null)
            {
                Log.Warning("Login failed for username: {Username}", dto.Username);
                return Unauthorized("Invalid username or password.");
            }

            Log.Information("Login successful for username: {Username}", dto.Username);
            return Ok(new { token });
        }
    }
}