using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> CheckExistingEmailAsync(string email, CancellationToken ct = default)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<bool>.Fail(Error.NotFound("Email.NotFound", $"User with email {email} is not found!"));
            else
                return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var user = new ApplicationUser()
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,
            };
            var userResult = await _userManager.CreateAsync(user, registerDto.Password);
            if(!userResult.Succeeded)
            {
                var errors = userResult.Errors.Select(e=> new Error(e.Code,e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(errors);
            }

            return Result<IdentityUserResult>.OK(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }

        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<IdentityUserResult>.Fail(Error.NotFound("Email.NotFound", $"User with email {email} is not found!"));
            else
                return Result<IdentityUserResult>.OK(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }

        public async Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.Users.Include(u=>u.Address).FirstOrDefaultAsync(u => u.Email == email, ct);

            if (user?.Address == null) return Result<AddressDto>.Fail(Error.NotFound("Address.NotFound", $"Address for user with email: {email} does not exist!"));

            return new AddressDto()
            {
                City = user.Address.City,
                Street = user.Address.Street,
                Country = user.Address.Country,
                FirstName = user.Address.FirstName,
                LastName = user.Address.LastName,
            };
        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Error.NotFound("User.NotFound", $"User with email {email} is not found!");

            var roles = await _userManager.GetRolesAsync(user);

            return roles.ToList();
        }
    }
}
