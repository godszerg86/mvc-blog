namespace BlogApp.Migrations
{
    using BlogApp.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BlogApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BlogApp.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            //admin

            ApplicationUser adminUser = new ApplicationUser(); 

            if (!context.Users.Any(item => item.UserName == "admin@admin.com"))
            {
                adminUser.UserName = "admin@admin.com";
                adminUser.Email = "admin@admin.com";
                adminUser.LastName = "Admin";
                adminUser.FirstName = "Zerg";
                adminUser.DisplayName = "z3rg";
                

                userManager.Create(adminUser, "admin@admin.com");
            } else
            {
                adminUser = context.Users.FirstOrDefault(item => item.UserName == "admin@admin.com"); 
            }

            if (!userManager.IsInRole(adminUser.Id,"Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }
            //Moderator

            ApplicationUser moderatorUser = new ApplicationUser();
            if (!context.Users.Any(item => item.UserName == "moderator@moderator.com"))
            {
                moderatorUser.UserName = "moderator@moderator.com";
                moderatorUser.Email = "moderator@moderator.com";
                moderatorUser.LastName = "Moderator";
                moderatorUser.FirstName = "MD";
                moderatorUser.DisplayName = "DM";


                userManager.Create(moderatorUser, "moderator@moderator.com");
            }
            else
            {
                moderatorUser = context.Users.FirstOrDefault(item => item.UserName == "moderator@moderator.com");
            }

            if (!userManager.IsInRole(moderatorUser.Id, "Moderator"))
            {
                userManager.AddToRole(moderatorUser.Id, "Moderator");
            }

        }
    }
}
