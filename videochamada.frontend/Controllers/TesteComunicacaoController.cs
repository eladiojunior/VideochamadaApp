using Microsoft.AspNetCore.Mvc;

namespace VideoChatApp.FrontEnd.Controllers;

public class TesteComunicacaoController : Controller
{
    public IActionResult Index()
    {
        return View("TesteComunicacaoCliente");
    }
    
    public IActionResult Cliente()
    {
        return View("TesteComunicacaoCliente");
    }
    public IActionResult Profissional()
    {
        return View("TesteComunicacaoProfissional");
    }
}