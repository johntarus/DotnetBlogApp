using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

public class UsersController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}