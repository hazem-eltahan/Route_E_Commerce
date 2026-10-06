using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using E_Commerce.Application.DTOs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }
        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            var userResult = await _identityService.FindUserByEmailAsync(loginDto.Email, ct);
            if (!userResult.IsSuccess)
                return Result<UserDto>.Fail(userResult.Errors);

            var passwordResult = await _identityService.CheckPasswordAsync(loginDto.Email, loginDto.Password, ct);
            if (!passwordResult.IsSuccess)
                return Result<UserDto>.Fail(passwordResult.Errors);

            if (!passwordResult.data)
                return Result<UserDto>.Fail(Error.Unauthorized("Invalid email or password!"));

            var user = userResult.data;

            var rolesResult = await _identityService.GetUserRolesAsync(user.Email, ct);
            var roles = rolesResult.data;

            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);

            return new UserDto()
            {
                Email = loginDto.Email,
                DisplayName = userResult.data.DisplayName,
                Token = token
            };
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var userResult = await _identityService.CreateUserAsync(registerDto, ct);
            if(!userResult.IsSuccess)
            {
                return Result<UserDto>.Fail(userResult.Errors);
            }
            var user = userResult.data;

            var rolesResult = await _identityService.GetUserRolesAsync(user.Email, ct);
            var roles = rolesResult.data;

            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);

            return Result<UserDto>.OK(new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = token
            });
        }
    }
}
