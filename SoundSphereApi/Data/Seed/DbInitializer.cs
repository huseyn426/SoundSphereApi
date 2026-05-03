using SoundSphereApi.Data.Context;
using SoundSphereApi.Models.Identity;

namespace SoundSphereApi.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(AppDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }
        }
    }
}
