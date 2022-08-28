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
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserStore<ApplicationUser> userStore
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
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

    public async Task<IActionResult> Add()
    {
        return View(new AddUserViewModel()
        {
            Roles = await FetchViewModelRoles()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddUserViewModel model)
    {
        if (!model.Roles.Any(r => r.Selected))
        {
            ModelState.AddModelError("Roles", "Add at least one role");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // extra step to add already taken email error to model errors
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(model);
            }

            var user = CreateUser();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            await _userStore.SetUserNameAsync(user, model.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, model.Roles?
                    .Where(r => r.Selected).Select(r => r.Name));
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateUserName")
                {
                    ModelState.AddModelError("UserName", "User name already exists");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", "Failed to add user. Try again later");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }


        return View(new EditUserViewModel()
        {
            Id = id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.Id);

        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.UserName = model.UserName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        return RedirectToAction(nameof(Index));
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

    private async Task<List<RoleViewModel>> FetchViewModelRoles()
    {
        var viewModelRoles = await _roleManager.Roles
            .Select(r => new RoleViewModel()
            {
                Id = r.Id,
                Name = r.Name
            }).ToListAsync();

        return viewModelRoles;
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                                                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }

        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
    
    [HttpDelete]
    public async Task<ActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded) return Json(new { result = "success", userName = user.UserName });
        
        var errors = result.Errors.Select(error => error.Description).ToList();

        return StatusCode(500, new { result = "failure", errors });
    }
}