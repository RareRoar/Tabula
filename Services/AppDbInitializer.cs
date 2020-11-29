using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tabula.Interfaces;
using Tabula.Models;

namespace Tabula.Services
{
    public static class AppDbInitializer
    {
        public static async Task InitializeAsync(UserManager<Profile> userManager, RoleManager<IdentityRole> roleManager,
            IAppDbInitData initData)
        {
            foreach(var roleName in initData.RolesList)
            {
                if (await roleManager.FindByNameAsync(roleName) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            if (await userManager.FindByNameAsync(initData.AdminName) == null)
            {
                Profile admin = new Profile { Email = initData.AdminEmail, UserName = initData.AdminName };
                IdentityResult result = await userManager.CreateAsync(admin, initData.AdminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }

    public class AppDbInitData : IAppDbInitData
    {
        public const string Section = "AppDbInitData";
        public List<string> RolesList { get; set; }
        public string AdminName { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPassword { get; set; }

    }
}
