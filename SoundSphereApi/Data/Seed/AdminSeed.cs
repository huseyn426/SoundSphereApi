using Microsoft.AspNetCore.Identity;
using SoundSphereApi.Data.Context;
using SoundSphereApi.Models.Identity;

namespace SoundSphereApi.Data.Seed
{
    public static class AdminSeed
    {
        public static async Task SeedAdminUserAsync(AppDbContext context)
        {
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");

            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Name = "Admin"
                };

                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();
            }

            var existingAdmin = context.Users.FirstOrDefault(u => u.Email == "admin@soundsphere.com");

            if (existingAdmin != null)
            {
                return;
            }

            var adminUser = new User
            {
                FullName = "System Administrator",
                UserName = "admin",
                Email = "admin@soundsphere.com",
                RoleId = adminRole.Id,
                IsActive = true
            };

            var passwordHasher = new PasswordHasher<User>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
