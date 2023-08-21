using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Controllers;
using VideoChatApp.FrontEnd.Services;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace videochamada.frontend.Controllers;

public class HomeController : GenericController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IServiceCliente _serviceCliente;

    public HomeController(ILogger<HomeController> logger, IServiceCliente cliente)
    {
        _logger = logger;
        _serviceCliente = cliente;
    }

    public IActionResult Index()
    {
        var idUsuario = ObterIdUsuario();
        var cliente = _serviceCliente.Obter(idUsuario);
        return View(cliente);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}