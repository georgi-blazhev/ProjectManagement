using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Data
{
    public class DatabaseSeeder
    {
        public static void Seed(IServiceProvider applicationServices)
        {
            using (IServiceScope serviceScope = applicationServices.CreateScope())
            {
                DatabaseContext context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();

                if (context.Database.EnsureCreated())
                {
                    PasswordHasher<User> hasher = new PasswordHasher<User>();

                    User userAdmin = new User()
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Email = "admin@test.com",
                        NormalizedEmail = "admin@test.com".ToUpper(),
                        EmailConfirmed = true,
                        UserName = "admin",
                        NormalizedUserName = "admin".ToUpper(),
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        FirstName = "admin"
                    };

                    userAdmin.PasswordHash = hasher.HashPassword(userAdmin, "adminpass");

                    User managerUser = new User()
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Email = "manager@test.com",
                        NormalizedEmail = "manager@test.com".ToUpper(),
                        EmailConfirmed = true,
                        UserName = "manager",
                        NormalizedUserName = "manager".ToUpper(),
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        FirstName = "manager"
                    };

                    managerUser.PasswordHash = hasher.HashPassword(managerUser, "managerpass");


                    IdentityRole identityRoleAdmin = new IdentityRole()
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Name = "Admin",
                        NormalizedName = "Admin".ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString("D")
                    };

                    IdentityRole identityRoleManager = new IdentityRole()
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Name = "Manager",
                        NormalizedName = "Manager".ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString("D")
                    };

                    IdentityRole identityRoleRegularUser = new IdentityRole()
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Name = "Regular User",
                        NormalizedName = "Regular User".ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString("D")
                    };

                    IdentityUserRole<string> identityAdminUserRole = new IdentityUserRole<string>() { RoleId = identityRoleAdmin.Id, UserId = userAdmin.Id };
                    IdentityUserRole<string> identityManagerUserRole = new IdentityUserRole<string>() { RoleId = identityRoleManager.Id, UserId = managerUser.Id };

                    context.Roles.Add(identityRoleAdmin);
                    context.Roles.Add(identityRoleManager);
                    context.Roles.Add(identityRoleRegularUser);
                    context.Users.Add(userAdmin);
                    context.Users.Add(managerUser);
                    context.UserRoles.Add(identityAdminUserRole);
                    context.UserRoles.Add(identityManagerUserRole);

                    context.SaveChanges();
                }
            }
        }
    }
}
