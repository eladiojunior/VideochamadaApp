using Microsoft.AspNetCore.Mvc;

namespace VideoChatApp.FrontEnd.Controllers;

public class EquipeSaudeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}