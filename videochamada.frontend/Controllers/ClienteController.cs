using Microsoft.AspNetCore.Mvc;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Controllers;

public class ClienteController : GenericController
{
    private readonly IServiceCliente _serviceCliente;
    
    public ClienteController(IServiceCliente cliente)
    {
        _serviceCliente = cliente;
    }

    [HttpGet]
    public IActionResult Registrar()
    {
        var model = new ClienteRegistroModel();
        return View("Registrar", model);
    }

    [HttpPost]
    public IActionResult Registrar(ClienteRegistroModel model)
    {
        if (!ModelState.IsValid)
            return View("Registrar", model);

        model.Id = ObterIdUsuario();
        var modelAtendimento = _serviceCliente.Registrar(model);
        
        return View("Atendimento", modelAtendimento);
        
    }

    [HttpGet]
    public IActionResult NovoAtendimento()
    {
        var idUsuario = ObterIdUsuario();
        var modelAtendimento = _serviceCliente.Obter(idUsuario);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        return View("Atendimento", modelAtendimento);
    }
}