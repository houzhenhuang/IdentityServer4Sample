using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MvcCookieAuthSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCookieAuthSample.Data
{
    public class ApplicationDbContextSeed
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationUserRole> _roleManager;

        public async Task SeedAsync(ApplicationDbContext context, IServiceProvider services)
        {
            if (!context.Roles.Any())
            {
                _roleManager = services.GetRequiredService<RoleManager<ApplicationUserRole>>();
                var role = new ApplicationUserRole
                {
                    Name = "Administrators",
                    NormalizedName = "Administrators"
                };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception("初始默认角色失败：" + result.Errors.SelectMany(e => e.Description));
                }
            }

            if (!context.Users.Any())
            {
                _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                var defaultUser = new ApplicationUser
                {
                    UserName = "Administrator",
                    Email = "jessetalk@163.com",
                    NormalizedUserName = "admin",
                    Avatar = "data:image/png;base64,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",
                };

                var result = await _userManager.CreateAsync(defaultUser, "123456");
                await _userManager.AddToRoleAsync(defaultUser, "Administrators");
                if (!result.Succeeded)
                {
                    throw new Exception("初始默认用户失败");
                }
            }
        }
    }
}
