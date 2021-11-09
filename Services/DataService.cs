using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Enums;
using BlogProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbContext,
                           RoleManager<IdentityRole> roleManager,
                           UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // wrapper function
        public async Task ManageDataAsync()
        {
            // create DB from bigrations
            await _dbContext.Database.MigrateAsync();

            // Step 1: seed some roles into the system
            await SeedRolesAsync();

            // Step 2: seed users into the system
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            // if there are already roles in the system, do nothing
            if (_dbContext.Roles.Any()) return;

            // otherwise we'll create a few roles.
            foreach(var role in Enum.GetNames(typeof(BlogRole)))
            {
                // use role manager, to create roles
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            // If there are already users in the system, do nothing.
            if (_dbContext.Users.Any()) return;

            // otherwise, seed new users in the system.

            // adds new instance of BlogUser
            var adminUser = new BlogUser()
            {
                Email = "austin.b.johnson98@gmail.com",
                UserName = "austin.b.johnson98@gmail.com",
                FirstName = "Austin",
                LastName = "Johnson",
                DisplayName = "Random",
                PhoneNumber = "(800) 555-1212",
                EmailConfirmed = true
            };

            // use the usermanager to create a new user that is defined by the AdminUser variable
            await _userManager.CreateAsync(adminUser, "Abc&123!");

            // add new user to administrator role
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            // create a moderator user
            var modUser = new BlogUser()
            {
                Email = "shival.bigman@gmail.com",
                UserName = "shival.bigman@gmail.com",
                FirstName = "Austin",
                LastName = "Grande",
                DisplayName = "Bored",
                PhoneNumber = "(800) 555-1213", 
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(modUser, "Abc&123!");
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Moderator.ToString());
        }
    }
}
