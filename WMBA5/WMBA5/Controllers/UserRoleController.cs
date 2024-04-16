using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMBA5.CustomControllers;
using WMBA5.Data;
using WMBA5.ViewModels;

namespace WMBA5.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserRoleController : CognizantController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly WMBAContext _wmbaContext;

        public UserRoleController(ApplicationDbContext context, UserManager<IdentityUser> userManager, WMBAContext wmbaContext)
        {
            _context = context;
            _userManager = userManager;
            _wmbaContext = wmbaContext;
        }
        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await (from u in _context.Users
                               .OrderBy(u => u.UserName)
                               select new UserVM
                               {
                                   ID = u.Id,
                                   UserName = u.UserName
                               }).ToListAsync();
            foreach (var u in users)
            {
                var _user = await _userManager.FindByIdAsync(u.ID);
                u.UserRoles = (List<string>)await _userManager.GetRolesAsync(_user);
                //Note: we needed the explicit cast above because GetRolesAsync() returns an IList<string>
            };
            return View(users);
        }
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var _user = await _userManager.FindByIdAsync(id);//IdentityRole
            if (_user == null)
            {
                return NotFound();
            }
            UserVM user = new UserVM
            {
                ID = _user.Id,
                UserName = _user.UserName,
                UserRoles = (List<string>)await _userManager.GetRolesAsync(_user)
            };
            PopulateAssignedRoleData(user);

            var teams = await _wmbaContext.Teams.ToListAsync();

            // Generate team-specific roles for the current user
            foreach (var team in teams)
            {
                var teamCoachRole = team.TeamName + " - Coach";
                var teamScorekeeperRole = team.TeamName + " - Scorekeeper";
                if (!user.UserRoles.Contains(teamCoachRole))
                {
                    user.UserRoles.Add(teamCoachRole);
                }
                if (!user.UserRoles.Contains(teamScorekeeperRole))
                {
                    user.UserRoles.Add(teamScorekeeperRole);
                }
            }

            ViewBag.Teams = teams;
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, string[] selectedRoles)
        {
            var _user = await _userManager.FindByIdAsync(Id);
            if (_user == null)
            {
                return NotFound();
            }

            try
            {
                // Get the current roles of the user
                var userRoles = await _userManager.GetRolesAsync(_user);

                // Remove all existing roles from the user
                var result = await _userManager.RemoveFromRolesAsync(_user, userRoles);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update user roles.");
                    return View();
                }

                // Add the selected roles to the user
                result = await _userManager.AddToRolesAsync(_user, selectedRoles);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update user roles.");
                    return View();
                }

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.");
                return View();
            }
        }

        private void PopulateAssignedRoleData(UserVM user)
        {//Prepare checkboxes for all Roles
            var allRoles = _context.Roles;
            var currentRoles = user.UserRoles;
            var viewModel = new List<RoleVM>();
            foreach (var r in allRoles)
            {
                viewModel.Add(new RoleVM
                {
                    RoleID = r.Id,
                    RoleName = r.Name,
                    Assigned = currentRoles.Contains(r.Name)
                });
            }
            ViewBag.Roles = viewModel;
        }

        private async Task UpdateUserRoles(string[] selectedRoles, UserVM userToUpdate)
        {
            var UserRoles = userToUpdate.UserRoles;//Current roles use is in
            var _user = await _userManager.FindByIdAsync(userToUpdate.ID);//IdentityUser

            if (selectedRoles == null)
            {
                //No roles selected so just remove any currently assigned
                foreach (var r in UserRoles)
                {
                    await _userManager.RemoveFromRoleAsync(_user, r);
                }
            }
            else
            {
                //At least one role checked so loop through all the roles
                //and add or remove as required

                //We need to do this next line because foreach loops don't always work well
                //for data returned by EF when working async.  Pulling it into an IList<>
                //first means we can safely loop over the colleciton making async calls and avoid
                //the error 'New transaction is not allowed because there are other threads running in the session'
                IList<IdentityRole> allRoles = _context.Roles.ToList<IdentityRole>();

                foreach (var r in allRoles)
                {
                    if (selectedRoles.Contains(r.Name))
                    {
                        if (!UserRoles.Contains(r.Name))
                        {
                            await _userManager.AddToRoleAsync(_user, r.Name);
                        }
                    }
                    else
                    {
                        if (UserRoles.Contains(r.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(_user, r.Name);
                        }
                    }
                }
            }
        }

        private async Task UpdateTeamRoles(string[] selectedRoles, UserVM userToUpdate, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userToUpdate.ID);

            if (selectedRoles == null || !selectedRoles.Any())
            {
                // Remove all team roles of the specified type
                var teamRolesToRemove = await _userManager.GetRolesAsync(user);
                teamRolesToRemove = teamRolesToRemove.Where(r => r.StartsWith(roleName)).ToList();
                foreach (var roleToRemove in teamRolesToRemove)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleToRemove);
                }
            }
            else
            {
                // Add or remove team roles based on selection
                foreach (var selectedRole in selectedRoles)
                {
                    var roleExist = await _userManager.IsInRoleAsync(user, selectedRole);
                    if (!roleExist)
                    {
                        await _userManager.AddToRoleAsync(user, selectedRole);
                    }
                }

                var rolesToRemove = (await _userManager.GetRolesAsync(user))
                    .Where(r => r.StartsWith(roleName) && !selectedRoles.Contains(r));
                foreach (var roleToRemove in rolesToRemove)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleToRemove);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
                _userManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
