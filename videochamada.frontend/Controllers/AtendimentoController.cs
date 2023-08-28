using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        var modelCliente = _serviceCliente.ObterCliente(idCliente);
        if (modelCliente == null)
        {
            ExibirErro("Erro ao identificar o Cliente do atendimento.");
            return RedirectToAction("Index", "Home");
        }
        return View("InicioAtendimento", modelCliente);

    }
    
    [HttpPost]
    public IActionResult NovoAtendimento(NovoAtendimentoModel model)
    {
        
        var idCliente = model.IdCliente;
        var modelAtendimento = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (modelAtendimento != null)
        {
            ExibirErro($"Existe um atendimento em aberto [{modelAtendimento.Situacao.ObterTextoEnum()}] continue o atendimento ou cancele.");
            return RedirectToAction("Index", "Home");
        }

        if (model.HasTermoUso == false)
        {
            ExibirErro($"Você não aceitou o Termo de Uso, infelizmente não podemos seguir com o atendimento.");
            return RedirectToAction("InicioAtendimento", "Atendimento");
        }
        
        modelAtendimento = _serviceAtendimento.CriarAtendimento(model);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        
        return RedirectToAction("VerificarDispositivo", "Atendimento");
        
    }

    [HttpGet]
    public IActionResult InicioAtendimento()
    {
        var idCliente = ObterIdClienteSession();
        var modelCliente = _serviceCliente.ObterCliente(idCliente);
        if (modelCliente == null)
        {
            ExibirErro("Erro ao identificar o Cliente do atendimento.");
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
            ExibirErro("Atendimento não encontrado para continuar.");
            return RedirectToAction("Index", "Home");
        }

        switch (atendimento.Situacao)
        {
            case SituacaoAtendimentoEnum.Registrado:
                return RedirectToAction("InicioAtendimento", "Atendimento");
            case SituacaoAtendimentoEnum.VerificacaoDispositivo:
                return RedirectToAction("VerificarDispositivo", "Atendimento");
            case SituacaoAtendimentoEnum.FilaAtendimento:
                return RedirectToAction("FilaAtendimento", "Atendimento");
            default:
                return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public IActionResult CancelarAtendimento(string idAtendimento)
    {
        
        var atendimento = _serviceAtendimento.ObterAtendimento(idAtendimento);
        if (atendimento == null)
        {
            ExibirErro("Atendimento não encontrado para continuar.");
        }
        var modelCliente = _serviceCliente.ObterCliente(atendimento.IdCliente);
        if (modelCliente == null)
        {
            ExibirErro("Erro ao identificar o Cliene do atendimento.");
        }

        if (!HasErros())
        {//Cancelar atendimento
            var avaliacaoAtendimento = new AvaliacaoAtendimentoModel();
            avaliacaoAtendimento.IdAtendimento = idAtendimento;
            avaliacaoAtendimento.IdCliente = atendimento.IdCliente;
            avaliacaoAtendimento.Comentario = "Cancelado pelo Cliente.";
            _serviceAtendimento.EncerrarAtendimento(avaliacaoAtendimento, SituacaoAtendimentoEnum.Cancelado);            
        }
        
        ExibirAlerta("Cancelamos seu atendimento em aberto, ficará no seu histórico para consulta.");
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

        _serviceAtendimento.VerificarDispositivoParaAtendimento(modelAtendimento.Id);
        
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
        return JsonResultSucesso(posicaoAtendimento);
    }
    
}