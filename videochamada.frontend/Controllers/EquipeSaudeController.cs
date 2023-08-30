using Microsoft.AspNetCore.Mvc;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Exceptions;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Controllers;

public class EquipeSaudeController : GenericController
{
    private readonly IServiceEquipeSaude _serviceEquipeSaude;
    private readonly IServiceAtendimento _serviceAtendimento;
    
    public EquipeSaudeController(IServiceEquipeSaude serviceEquipeSaude, IServiceAtendimento serviceAtendimento)
    {
        _serviceEquipeSaude = serviceEquipeSaude;
        _serviceAtendimento = serviceAtendimento;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        if (HasProfissionalSaudeLogado())
            return RedirectToAction("AreaAtendimento", "EquipeSaude");
        return View();
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
        model.QtdClientesNaFila = _serviceAtendimento.QtdClienteFilaAtendimento();
        model.QtdProfissionaisOnline = _serviceEquipeSaude.QtdProfissionalSaudeOnline();
        model.Atendimentos = _serviceAtendimento.ListarAtendimentosProfissional(idProfissional);
        
        return View(model);
    }

    [HttpPost]
    public IActionResult Acessar(ProfissionalSaudeAcessoModel model)
    {
        
        if (!ModelState.IsValid)
            return View("Index", model);
        
        try
        {
            var profissionalModel = _serviceEquipeSaude.AutenticarProfissionalSaude(model.Email, model.Senha);
            if (profissionalModel == null)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível autenticar o profissional de saúde.");
                return View("Index", model);
            }

            GravarIdProfissionalSaude(profissionalModel.Id);
            ExibirAlerta($"Bem-vindo, {profissionalModel.Nome}");
            return RedirectToAction("AreaAtendimento", "EquipeSaude");
        }
        catch (ServiceException erro)
        {
            ModelState.AddModelError(string.Empty, erro.Message);
            return View("Index", model);
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
            LogoffProfissionalSaude();
        return RedirectToAction("Index", "EquipeSaude");
    }
}