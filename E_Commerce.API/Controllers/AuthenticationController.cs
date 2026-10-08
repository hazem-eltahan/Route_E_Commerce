using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using E_Commerce.Application.DTOs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.API.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto, CancellationToken ct)
        {
            return ToActionResult(await _authenticationService.LoginAsync(loginDto, ct));
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto, CancellationToken ct)
        {
            return ToActionResult(await _authenticationService.RegisterAsync(registerDto, ct));
        }

        [HttpGet("ExistingEmail")]
        public async Task<ActionResult<bool>> CheckExistingEmail([FromQuery]string email, CancellationToken ct)
        => ToActionResult(await _authenticationService.CheckExistingEmailAsync(email, ct));

        [Authorize]
        [HttpGet("CurrentUser")]
        public async Task<ActionResult<UserDto>> CurrentUser(CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException("No email claim found!");

            return ToActionResult(await _authenticationService.GetCurrentUserAsync(email, ct));
        }
    }
}
    