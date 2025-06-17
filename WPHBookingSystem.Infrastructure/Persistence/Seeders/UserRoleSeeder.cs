using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WPHBookingSystem.Infrastructure.Identity;

namespace WPHBookingSystem.Infrastructure.Persistence.Seeders
{
    public static class UserRoleSeeder
    {
        // Static GUIDs for seeding
        public static readonly Guid AdministratorRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid AdministratorUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static void Seed(ModelBuilder modelBuilder)
        {
            // Seed Administrator Role
            modelBuilder.Entity<IdentityRole>().HasData(new
            {
                Id = AdministratorRoleId.ToString(),
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                ConcurrencyStamp = "1"
            });

            // Seed Administrator User
            //var hasher = new PasswordHasher<ApplicationUser>();
            //var passwordHash = hasher.HashPassword(null, "Admin123!");

            modelBuilder.Entity<ApplicationUser>().HasData(new
            {
                Id = AdministratorUserId.ToString(),
                UserName = "admin@wphhotel.com",
                NormalizedUserName = "ADMIN@WPHHOTEL.COM",
                Email = "admin@wphhotel.com",
                NormalizedEmail = "ADMIN@WPHHOTEL.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEEURhob03jhq0fPvh4RXUr+lTBQ4x5Hyptyk5uRIDYKspR8eYQLb7Fffg25dghmJBA==",
                SecurityStamp = "1",
                ConcurrencyStamp = "1",
                PhoneNumber = "09158902395",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnd = (DateTimeOffset?)null,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                Firstname = "Administrator",
                LastName = "Administrator"
            });

            // Seed User Role relationship
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new
            {
                UserId = AdministratorUserId.ToString(),
                RoleId = AdministratorRoleId.ToString()
            });
        }
    }
} 