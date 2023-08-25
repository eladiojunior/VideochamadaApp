using Microsoft.AspNetCore.Mvc;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Enums;
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
        var modelAtendimento = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (modelAtendimento != null)
        {
            ExibirAlerta($"Existe um atendimento na situação [{modelAtendimento.Situacao.ObterTextoEnum()}] continue o atendimento ou cancele.");
            return RedirectToAction("Index", "Home");
        }
        
        modelAtendimento = _serviceAtendimento.CriarAtendimento(idCliente);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        
        return RedirectToAction("InicioAtendimento", "Atendimento");
        
    }

    [HttpGet]
    public IActionResult InicioAtendimento()
    {
        var idCliente = ObterIdClienteSession();
        var modelCliente = _serviceCliente.ObterCliente(idCliente);
        if (modelCliente == null)
        {
            ExibirAlerta("Erro ao identificar o Cliente do atendimento.");
            return RedirectToAction("Index", "Home");
        }

        return View("InicioAtendimento", modelCliente);

    }

    [HttpGet]
    public IActionResult ContinuarAtendimento(string idAtendimento)
    {
        
        var atendimento = _serviceAtendimento.ObterAtendimento(idAtendimento);
        if (atendimento == null)
        {
            ExibirAlerta("Atendimento não encontrado para continuar.");
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("InicioAtendimento", "Atendimento");
        
    }

    [HttpGet]
    public IActionResult CancelarAtendimento(string idAtendimento)
    {
        
        var atendimento = _serviceAtendimento.ObterAtendimento(idAtendimento);
        if (atendimento == null)
        {
            ExibirAlerta("Atendimento não encontrado para continuar.");
        }
        var modelCliente = _serviceCliente.ObterCliente(atendimento.IdCliente);
        if (modelCliente == null)
        {
            ExibirAlerta("Erro ao identificar o Cliene do atendimento.");
        }

        if (HasAlerta())
        {//Cancelar atendimento
            var avaliacaoAtendimento = new AvaliacaoAtendimentoModel();
            avaliacaoAtendimento.IdAtendimento = idAtendimento;
            avaliacaoAtendimento.IdCliente = atendimento.IdCliente;
            avaliacaoAtendimento.Comentario = "Cancelado pelo Cliente.";
            _serviceAtendimento.EncerrarAtendimento(avaliacaoAtendimento, SituacaoAtendimentoEnum.Cancelado);            
        }
        
        return RedirectToAction("Index", "Home");
        
    }
    
    [HttpGet]
    public IActionResult VerificarDispositivo()
    {
        var idCliente = ObterIdClienteSession();
        var modelCliente = _serviceCliente.ObterCliente(idCliente);
        if (modelCliente == null)
            return RedirectToAction("Index", "Home");
        var modelAtendimento = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        return View("DispositivoAtendimento", modelCliente);
    }

    [HttpGet]
    public IActionResult FilaAtendimento()
    {
        var idCliente = ObterIdClienteSession();
        var modelCliente = _serviceCliente.ObterCliente(idCliente);
        if (modelCliente == null)
            return RedirectToAction("Index", "Home");
        var modelAtendimento = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        
        _serviceAtendimento.EntrarFilaAtendimento(modelAtendimento);

        var model = new ClienteFilaAtendimentoModel();
        model.Cliente = modelCliente;
        model.PosicaoNaFila = _serviceAtendimento.PosicaoFilaAtendimento(idCliente);
        model.QtdProfissionaisOnline = _serviceEquipeSaude.QtdProfissionalSaudeOnline();
        
        return View("FilaAtendimento", model);
        
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

        var situacaoAtendimento = model.HasDesistencia
            ? SituacaoAtendimentoEnum.Desistencia
            : SituacaoAtendimentoEnum.Finalizado;
        _serviceAtendimento.EncerrarAtendimento(model, situacaoAtendimento);
        return RedirectToAction("Index", "Home");
        
    }

    [HttpGet]
    public IActionResult PosicaoFilaAtendimento()
    {
        var posicaoAtendimento = new PosicaoAtendimentoModel();
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