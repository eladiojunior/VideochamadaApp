using Microsoft.AspNetCore.Mvc;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Controllers;

public class AtendimentoController : GenericController
{

    private readonly IServiceCliente _serviceCliente;
    private readonly IServiceAtendimento _serviceAtendimento;
    private readonly IServiceEquipeSaude _serviceEquipeSaude;
    
    public AtendimentoController(IServiceCliente serviceCliente, IServiceAtendimento serviceAtendimento, IServiceEquipeSaude serviceEquipeSaude)
    {
        _serviceCliente = serviceCliente;
        _serviceAtendimento = serviceAtendimento;
        _serviceEquipeSaude = serviceEquipeSaude;
    }
    
    [HttpGet]
    public IActionResult NovoAtendimento()
    {
        var idCliente = ObterIdClienteSession();
        var modelAtendimento = _serviceAtendimento.CriarAtendimento(idCliente);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        return View("InicioAtendimento", modelAtendimento);
    }
    
    [HttpGet]
    public IActionResult VerificarDispositivo()
    {
        var idCliente = ObterIdClienteSession();
        var modelAtendimento = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        return View("DispositivoAtendimento", modelAtendimento);
    }

    [HttpGet]
    public IActionResult FilaAtendimento()
    {
        var idCliente = ObterIdClienteSession();
        var modelAtendimento = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        modelAtendimento = _serviceAtendimento.EntrarFilaAtendimento(modelAtendimento);
        return View("FilaAtendimento", modelAtendimento);
    }

    [HttpGet]
    public IActionResult SairFilaAtendimento()
    {
        
        var idCliente = ObterIdClienteSession();
        var modelAtendimento = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        
        var modelAvaliacao = new AvaliacaoAtendimentoModel();
        modelAvaliacao.IdCliente = modelAtendimento.IdCliente;
        modelAvaliacao.IdAtendimento = modelAtendimento.Id;
        modelAvaliacao.HasDesistencia = true;
        
        return View("AvaliarAtendimento", modelAvaliacao);
        
    }

    [HttpPost]
    public IActionResult AvaliarAtendimento(AvaliacaoAtendimentoModel model)
    {
        
        if (!ModelState.IsValid)
            return View("AvaliarAtendimento", model);

        _serviceAtendimento.EncerrarAtendimento(model);
        return RedirectToAction("Index", "Home");
        
    }

    [HttpGet]
    public IActionResult PosicaoFilaAtendimento()
    {
        var posicaoAtendimento = new PosicaoAtencimentoModel();
        var idCliente = ObterIdClienteSession();
        if (!string.IsNullOrEmpty(idCliente))
        {
            posicaoAtendimento.IdCliente = idCliente;
            posicaoAtendimento.PosicaoNaFila = _serviceAtendimento.PosicaoFilaAtendimento(idCliente);
            posicaoAtendimento.QtdClientesFila = _serviceAtendimento.QtdClienteFilaAtendimento();
            posicaoAtendimento.QtdProfissionaisOnline = _serviceEquipeSaude.QtdProfissionalSaudeOnline();
        }
        return JsonResultSucesso(new JsonResultModel(posicaoAtendimento));
    }
    
}