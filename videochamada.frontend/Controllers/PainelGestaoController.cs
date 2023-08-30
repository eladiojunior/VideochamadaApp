using Microsoft.AspNetCore.Mvc;

namespace VideoChatApp.FrontEnd.Controllers;

public class PainelGestaoController : GenericController
{
    public IActionResult Index()
    {
        return View();
    }
}