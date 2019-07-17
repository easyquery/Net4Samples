using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using EqAspNet4Demo.Models;
using EqAspNet4Demo.Services;

namespace EqAspNet4Demo
{
    internal static class IdentityHelper
    {

        public static void SeedEqManagerRole()
        {

            const string eqManagerRole = "eq-manager";

            using (var context = ApplicationDbContext.Create())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                if (!roleManager.RoleExists(eqManagerRole)) {
                    roleManager.Create(new IdentityRole { Name = eqManagerRole });
                }
            }
        }

        public static void SeedDefaultUser()
        {
            const string defaultUserEmail = "demo@korzh.com";
            const string defaultUserPassword = "demo";

            using (var context = ApplicationDbContext.Create())
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                // Configure validation logic for passwords
                userManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 4,
                    RequireNonLetterOrDigit = false,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                };

                var reportGenerator = new DefaultReportGenerator(context);
                var user = userManager.FindByEmail(defaultUserEmail);
                if (user == null) {
                    user = new ApplicationUser {
                        Email = defaultUserEmail,
                        UserName = defaultUserEmail,
                        EmailConfirmed = true
                    };

                    var result = userManager.Create(user, defaultUserPassword);
                    if (result.Succeeded) {
                        reportGenerator.Generate(user);
                    }
                }
            }
        }
    }
}