using Microsoft.AspNetCore.Mvc;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Exceptions;
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

        try
        {
            var clienteModel = _serviceCliente.RegistrarCliente(model);
            GravarIdClienteSession(clienteModel.Id);
            ExibirAlerta("Cliente registrado com sucesso.");
            return RedirectToAction("NovoAtendimento", "Atendimento");
        }
        catch (ServiceException erro)
        {
            ModelState.AddModelError(string.Empty, erro.Message);
            return View("Registrar", model);
        }
        
    }

    [HttpGet]
    public IActionResult Consultar()
    {
        return View("Consultar");
    }

    [HttpPost]
    public IActionResult Consultar(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ModelState.AddModelError("Email", "E-mail não informado.");
            return View("Consultar");
        }
        var clienteModel = _serviceCliente.ObterClientePorEmail(email);
        if (clienteModel == null)
        {
            ModelState.AddModelError("Email", "Nenhum cliente encontrato com o e-mail informado.");
            return View("Consultar");
        }
        GravarIdClienteSession(clienteModel.Id);
        return RedirectToAction("NovoAtendimento", "Atendimento");
    }

}