using IdentityPractise.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityPractise.Controllers;

[Authorize(Roles = "Admin")]
public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        
        return View(roles);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(RoleFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Index), await FetchRoles());
        }

        if (await _roleManager.RoleExistsAsync(model.Name))
        {
            ModelState.AddModelError("Name", "Role exists");
            return View(nameof(Index), await FetchRoles());
        }

        await _roleManager.CreateAsync(new IdentityRole(model.Name));
        return RedirectToAction(nameof(Index));
    }

    private async Task<IEnumerable<IdentityRole>> FetchRoles()
    {
        return await _roleManager.Roles.ToListAsync();
    }
}