using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Exceptions;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Controllers;

public class EquipeSaudeController : GenericController
{
    private readonly IServiceEquipeSaude _serviceEquipeSaude;
    private readonly IServiceAtendimento _serviceAtendimento;
    private readonly IServiceCliente _serviceCliente;
    
    public EquipeSaudeController(IServiceEquipeSaude serviceEquipeSaude, IServiceAtendimento serviceAtendimento, IServiceCliente serviceCliente)
    {
        _serviceEquipeSaude = serviceEquipeSaude;
        _serviceAtendimento = serviceAtendimento;
        _serviceCliente = serviceCliente;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        if (HasProfissionalSaudeLogado())
            return RedirectToAction("AreaAtendimento", "EquipeSaude");
        return View("Acesso");
    }

    [HttpGet]
    public IActionResult AreaAtendimento()
    {
        if (!HasProfissionalSaudeLogado())
        {
            ExibirAlerta("Não identificamos um profissional de saúde logado.");
            return RedirectToAction("Index", "EquipeSaude");
        }

        var idProfissional = ObterIdProfissionalSaude();
        var profissionalSaude = _serviceEquipeSaude.ObterProfissionalSaude(idProfissional);
        if (profissionalSaude == null)
        {
            ExibirAlerta("Não identificamos um profissional de saúde pelo sua identificação.");
            return RedirectToAction("Index", "EquipeSaude");
        }

        var model = new AreaAtendimentoModel();
        model.ProfissionalSaude = profissionalSaude;
        model.QtdProfissionaisOnline = _serviceEquipeSaude.QtdProfissionalSaudeOnline();
        model.QtdClientesNaFila = _serviceAtendimento.QtdClienteFilaAtendimento();
        model.QtdClientesEmAtendimento = _serviceAtendimento.QtdClienteEmAtendimento();
        model.Atendimentos = _serviceAtendimento.ListarAtendimentosProfissional(idProfissional, true);
        
        return View(model);
    }

    [HttpPost]
    public IActionResult Acessar(ProfissionalSaudeAcessoModel model)
    {
        
        if (!ModelState.IsValid)
            return View("Acesso", model);
        
        try
        {
            var profissionalModel = _serviceEquipeSaude.AutenticarProfissionalSaude(model.Email, model.Senha);
            if (profissionalModel == null)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível autenticar o profissional de saúde.");
                return View("Acesso", model);
            }

            GravarIdProfissionalSaude(profissionalModel.Id);
            ExibirAlerta($"Bem-vindo, {profissionalModel.Nome}");
            return RedirectToAction("AreaAtendimento", "EquipeSaude");
        }
        catch (ServiceException erro)
        {
            ModelState.AddModelError(string.Empty, erro.Message);
            return View("Acesso", model);
        }
        
    }

    [HttpGet]
    public IActionResult Registrar()
    {
        if (HasProfissionalSaudeLogado())
            return RedirectToAction("AreaAtendimento", "EquipeSaude");       
        var model = new ProfissionalSaudeRegistroModel();
        return View("Registrar", model);
    }
    
    [HttpPost]
    public IActionResult Registrar(ProfissionalSaudeRegistroModel model)
    {
        
        if (!ModelState.IsValid)
            return View("Registrar", model);

        try
        {
            var profissionalModel = _serviceEquipeSaude.RegistrarProfissionalSaude(model);
            
            GravarIdProfissionalSaude(profissionalModel.Id);
            
            ExibirAlerta("Profissional de saúde registrado com sucesso.");
            
            return RedirectToAction("AreaAtendimento", "EquipeSaude");
        }
        catch (ServiceException erro)
        {
            ModelState.AddModelError(string.Empty, erro.Message);
            return View("Registrar", model);
        }
        
    }

    [HttpGet]
    public IActionResult Sair()
    {
        if (HasProfissionalSaudeLogado())
        {
            var idProfissional = ObterIdProfissionalSaude();
            _serviceEquipeSaude.LogoffProfissionalSaude(idProfissional);
            LogoffProfissionalSaudeSession();
        }
        return RedirectToAction("Index", "EquipeSaude");
    }
    
    [HttpGet]
    public IActionResult SituacaoFilaAtendimento()
    {
        var filaAtendimento = new ProfissionalSaudeFilaAtendimentoModel();
        var idProfissional = ObterIdProfissionalSaude();
        var profissionalSaude = _serviceEquipeSaude.ObterProfissionalSaude(idProfissional);
        if (profissionalSaude == null)
            return JsonResultErro("Profissional de Saúde não encontrado.");            
        
        filaAtendimento.IdProfissional = profissionalSaude.Id;
        filaAtendimento.QtdClientesEmAtendimento = _serviceAtendimento.QtdClienteEmAtendimento();
        filaAtendimento.QtdClientesFila = _serviceAtendimento.QtdClienteFilaAtendimento();
        filaAtendimento.QtdProfissionaisOnline = _serviceEquipeSaude.QtdProfissionalSaudeOnline();
        
        return JsonResultSucesso(filaAtendimento);
        
    }
    
    [HttpPost]
    public IActionResult AtualizarSituacaoAtendimentoProfissional(bool hasSituacaoAtendimento)
    {
        var idProfissional = ObterIdProfissionalSaude();
        var profissionalSaude = _serviceEquipeSaude.ObterProfissionalSaude(idProfissional);
        if (profissionalSaude == null)
            return JsonResultErro("Profissional de Saúde não encontrado.");
        try
        {
            _serviceEquipeSaude.AtualizarSituacaoProfissionalAendimento(idProfissional, hasSituacaoAtendimento);
            
            var filaAtendimento = new ProfissionalSaudeFilaAtendimentoModel();
            filaAtendimento.IdProfissional = profissionalSaude.Id;
            filaAtendimento.QtdClientesEmAtendimento = _serviceAtendimento.QtdClienteEmAtendimento();
            filaAtendimento.QtdClientesFila = _serviceAtendimento.QtdClienteFilaAtendimento();
            filaAtendimento.QtdProfissionaisOnline = _serviceEquipeSaude.QtdProfissionalSaudeOnline();
            return JsonResultSucesso(filaAtendimento);            
        }
        catch (Exception erro)
        {
            return JsonResultErro(erro.Message);
        }
    }
    
    [HttpGet]
    public IActionResult CarregarAtendimentosProfissional(bool hasAtendimentosRealizados)
    {
        var idProfissional = ObterIdProfissionalSaude();
        var profissionalSaude = _serviceEquipeSaude.ObterProfissionalSaude(idProfissional);
        if (profissionalSaude == null)
            return JsonResultErro("Profissional de Saúde não encontrado.");
        
        var listaAtendimentos = _serviceAtendimento.ListarAtendimentosProfissional(idProfissional, hasAtendimentosRealizados);
        var model = hasAtendimentosRealizados
            ? RenderRazorViewToString("_AtendimentosRealizadosPartial", listaAtendimentos)
            : RenderRazorViewToString("_AtendimentosEmAndamentoPartial", listaAtendimentos);
        return JsonResultSucesso(model);
    }

    [HttpGet]
    public IActionResult FinalizarAtendimento()
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    public IActionResult EmAtendimento(string idAtendimento)
    {
        
        if (string.IsNullOrEmpty(idAtendimento))
            return RedirectToAction("Index", "EquipeSaude");
        
        var atendimento = _serviceAtendimento.ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return RedirectToAction("Index", "EquipeSaude");
        
        var model = new ProfissionalSaudeEmAtendimentoModel();
        model.IdAtendimento = atendimento.Id;
        model.IdCliente = atendimento.Cliente.Id;
        model.Cliente = atendimento.Cliente;
        model.Cliente.Arquivos = _serviceAtendimento.ListarArquivosAtendimento(atendimento.Id);

        //Recuperar profissional do atendimento...
        var profissionalAtendimento = atendimento.ProfissionalSaude;
        if (profissionalAtendimento == null)
            return RedirectToAction("Index", "EquipeSaude");
        
        model.IdProfissionalSaude = profissionalAtendimento.Id;        
        model.ProfissionalSaude = profissionalAtendimento;

        //Recuperar o histórico de chat do atendimento...
        model.ChatAtendimento = _serviceAtendimento.ObterChatAtendimento(atendimento.Id);
        
        return View("ProfissionalEmAtendimento", model);
        
    }

}