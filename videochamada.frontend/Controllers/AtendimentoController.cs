using Microsoft.AspNetCore.Mvc;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Controllers;

public class AtendimentoController : GenericController
{

    private readonly IServiceCliente _serviceCliente;
    
    public AtendimentoController(IServiceCliente cliente)
    {
        _serviceCliente = cliente;
    }
    
    [HttpGet]
    public IActionResult NovoAtendimento()
    {
        var idUsuario = ObterIdUsuario();
        var modelCliente = _serviceCliente.ObterCliente(idUsuario);
        if (modelCliente == null)
            return RedirectToAction("Index", "Home");
        var modelAtendimento = _serviceCliente.NovoAtendimento(modelCliente);
        return View("InicioAtendimento", modelAtendimento);
    }
    
    [HttpGet]
    public IActionResult VerificarDispositivo()
    {
        var idUsuario = ObterIdUsuario();
        var modelAtendimento = _serviceCliente.ObterAtendimentoAberto(idUsuario);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        return View("DispositivoAtendimento", modelAtendimento);
    }

    [HttpGet]
    public IActionResult FilaAtendimento()
    {
        var idUsuario = ObterIdUsuario();
        var modelAtendimento = _serviceCliente.ObterAtendimentoAberto(idUsuario);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        return View("FilaAtendimento", modelAtendimento);
    }

    [HttpGet]
    public IActionResult SairFilaAtendimento()
    {
        var idUsuario = ObterIdUsuario();
        var modelAtendimento = _serviceCliente.ObterAtendimentoAberto(idUsuario);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        var modelAvaliacao = new AvaliacaoAtendimentoModel();
        modelAvaliacao.IdCliente = modelAtendimento.IdCliente;
        modelAvaliacao.IdAtendimento = modelAtendimento.IdAtendimento;
        modelAvaliacao.HasDesistencia = true;
        return View("AvaliarAtendimento", modelAvaliacao);
    }

    [HttpPost]
    public IActionResult AvaliarAtendimento(AvaliacaoAtendimentoModel model)
    {
        
        if (!ModelState.IsValid)
            return View("AvaliarAtendimento", model);

        _serviceCliente.EncerrarAtendimento(model);
        
        return RedirectToAction("Index", "Home");
        
    }
}