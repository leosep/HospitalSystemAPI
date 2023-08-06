using HospitalSystem.Domain.Entities;
using HospitalSystem.Services.AuthService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="user">The user to authenticate.</param>
        /// <returns>The JWT.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(User user)
        {
            var token = await _authService.AuthenticateUserAsync(user.Username, user.Password);

            if (token == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(new { token });
        }
    }
}
