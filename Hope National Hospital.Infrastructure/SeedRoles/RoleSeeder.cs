using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Hope_National_Hospital.Infrastructure.SeedRoles
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole>roleManager)
        {
            string[] roles =
            {
                "SuperAdmin",
                "Admin",
                "Doctor",
                "Nurse",
                "Receptionist",
                "Pharmacist",
                "LabTechnician",
                "Accountant",
                "HR",
                "InventoryManager"
            };

            foreach(var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }
        }
    }
}
