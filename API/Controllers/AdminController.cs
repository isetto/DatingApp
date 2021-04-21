using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly DataContext context;
        public AdminController(UserManager<AppUser> userManager, DataContext context)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            // var userRoleArray = await userManager.Users
            // .Include(user => user.UserRoles)
            // .ThenInclude(userRole => userRole.Role)
            // .OrderBy(user => user.UserName)
            // .Select(user => new
            // {
            //     user.Id,
            //     Username = user.UserName,
            //     Roles = user.UserRoles.Select(r => r.Role.Name).ToList()
            // })
            // .ToListAsync();

            var usersRoles = await context.UserRoles.ToListAsync(); //temp solution since above one doesnt work
            var roles = await context.Roles.ToListAsync();
            var users = await context.Users.ToListAsync();

            var userRoleArray = new List<TempUserRole>();
            foreach (var userRole in usersRoles)
            {
                foreach (var role in roles)
                {
                    if(userRole.RoleId == role.Id) userRoleArray.Add(new TempUserRole
                    {
                        Id = userRole.UserId,
                        Role = role.Name
                    });
                }
            }

            for (int i = 0; i < userRoleArray.Count; i++)
            {
                  foreach (var user in users)
                {
                    if(userRoleArray[i].Id == user.Id) userRoleArray[i].Username = user.UserName;
                }
            }

           
            return Ok(userRoleArray);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();
            var user = await userManager.FindByNameAsync(username);
            if(user == null) return NotFound("Could not find user");
            var userRoles = await userManager.GetRolesAsync(user);
            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if(!result.Succeeded) return BadRequest("Failed to add to roles");
            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if(!result.Succeeded) return BadRequest("Failed to remove from roles");
            return Ok(await userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or moderators admins can see this");
        }
    }
}