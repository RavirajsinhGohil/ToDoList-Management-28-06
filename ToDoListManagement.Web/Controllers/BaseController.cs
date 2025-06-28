using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

public class BaseController : Controller
{
    protected UserViewModel? SessionUser;
    private readonly IAuthService _authService;
    public BaseController(IAuthService authService)
    {
        _authService = authService;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        string? token = HttpContext.Request.Cookies["Token"];
        if (!string.IsNullOrEmpty(token))
        {
            SessionUser = await _authService.GetUserFromToken(token);
            if (SessionUser != null)
            {
                ViewBag.SessionUser = SessionUser;
            }
        }
        await next();
    }
}
