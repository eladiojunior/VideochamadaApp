using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Controllers;
using VideoChatApp.FrontEnd.Services;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace videochamada.frontend.Controllers;

public class HomeController : GenericController
{
    private readonly IServiceCliente _serviceCliente;
    private readonly IServiceAtendimento _serviceAtendimento;

    public HomeController(IServiceCliente cliente, IServiceAtendimento atendimento)
    {
        _serviceCliente = cliente;
        _serviceAtendimento = atendimento;
    }

    public IActionResult Index()
    {
        var idCliente = ObterIdCliente();
        var cliente = _serviceCliente.ObterCliente(idCliente);
        if (cliente != null)
            cliente.Atendimentos = _serviceAtendimento.ListarAtendimentosCliente(idCliente);
        return View(cliente);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    [HttpGet("get-ip")]
    public IActionResult GetClientIp()
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ipAddress))
        {
            return BadRequest("Não foi possível obter o endereço IP.");
        }
        return Ok(new { IPAddress = ipAddress });
    }
        
}