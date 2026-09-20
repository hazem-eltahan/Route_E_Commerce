using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Identity.Data;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class IdentityDataSeeder : IDataSeeder
    {
        private readonly StoreIdentityDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityDataSeeder> _logger;

        public IdentityDataSeeder(StoreIdentityDbContext dbContext,UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,ILogger<IdentityDataSeeder> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedingDataAsync(CancellationToken ct = default)
        {
            try
            {
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync(ct);
                if (pendingMigrations.Any())
                {
                    await _dbContext.Database.MigrateAsync(ct);
                }

                var requiredRoles = new[] { "Admin", "SuperAdmin" };

                foreach (var requiredRole in requiredRoles)
                {
                    if (await _roleManager.RoleExistsAsync(requiredRole))
                    {
                        _logger.LogInformation("Role '{RequiredRole}' already exists!", requiredRole);
                        continue;
                    }

                    var creatingRoleResult = await _roleManager.CreateAsync(new IdentityRole(requiredRole));
                    if (creatingRoleResult.Succeeded)
                    {
                        _logger.LogInformation("Role '{RequiredRole}' created successfully!", requiredRole);
                    }
                    else
                    {
                        var roleErrors = string.Join(", ", creatingRoleResult.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to create role '{RequiredRole}' - Errors: {Errors}", requiredRole, roleErrors);
                    }
                }

                var adminEmail = "hazem@gmail.com";

                if (await _userManager.FindByEmailAsync(adminEmail) is null)
                {
                    var admin = new ApplicationUser()
                    {
                        DisplayName = "Hazem Eltahan",
                        Email = adminEmail,
                        UserName = "HazemEltahan",
                        PhoneNumber = "01111111111",
                    };
                    var creatingUserResult = await _userManager.CreateAsync(admin, "P@ssw0rd");
                    if (creatingUserResult.Succeeded)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(admin, "SuperAdmin");
                        if (!roleResult.Succeeded)
                        {
                            var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                            _logger.LogWarning("Failed to add role SuperAdmin to {AdminEmail} - Errors: {Errors}", admin.Email, roleErrors);
                        }
                    }
                    else
                    {
                        var userErrors = string.Join(", ", creatingUserResult.Errors.Select(e => e.Description));
                        _logger.LogWarning("Creating default admin {AdminEmail} failed - Errors: {UserErrors}", admin.Email, userErrors);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Identity data seeding failed!");
                return;
            }
        }
    }
}
