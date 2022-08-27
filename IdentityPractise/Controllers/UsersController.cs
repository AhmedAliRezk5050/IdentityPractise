using IdentityPractise.Models;
using IdentityPractise.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityPractise.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = (await _userManager.Users.ToListAsync()).Select(
            user => new UserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = _userManager.GetRolesAsync(user).Result
            }
        );

        return View(users);
    }

    public async Task<IActionResult> ManageRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var roleViewModelList = (await _roleManager.Roles.ToListAsync()).Select(
            role => new RoleViewModel()
            {
                Id = role.Id,
                Name = role.Name,
                Selected = _userManager.IsInRoleAsync(user, role.Name).Result
            }
        ).ToList();


        return View(new UserRolesViewModel()
        {
            Roles = roleViewModelList,
            UserId = id,
            UserName = user.UserName
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageRoles(UserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var modelRoles = model.Roles;

        foreach (var role in modelRoles)
        {
            if (userRoles.Any(r => r == role.Name) && !role.Selected)
            {
                await _userManager.RemoveFromRoleAsync(user, role.Name);
            }
            
            if (!userRoles.Any(r => r == role.Name) && role.Selected)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }
        }
        
        return RedirectToAction(nameof(Index));
    }
}