using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

public class ProfileController : BaseController
{
    private readonly IProfileService _profileService;

    public ProfileController(IAuthService authService, IProfileService profileService) : base(authService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ProfileViewModel? user = await _profileService.GetUserProfileAsync(SessionUser?.Email);
        if (user == null)
        {
            return NotFound("User profile not found.");
        }
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool isUpdated = await _profileService.UpdateUserProfileAsync(model);
            if (isUpdated)
            {
                TempData["SuccessMessage"] = "Profile updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update profile. Please try again.";
            }
            return View("Index", model);
        }
        return View("Index", model);
    }
}
