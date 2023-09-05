using Microsoft.AspNetCore.Mvc;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Enums;
using VideoChatApp.FrontEnd.Services.Exceptions;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Controllers;

public class PainelGestaoController : GenericController
{
    private readonly IServicePainelGestao _servicePainelGestao;
    private readonly IServiceCliente _serviceCliente;
    private readonly IServiceEquipeSaude _serviceEquipeSaude;
    private readonly IServiceAtendimento _serviceAtendimento;
    
    public PainelGestaoController(IServicePainelGestao servicePainelGestao, IServiceCliente serviceCliente, IServiceEquipeSaude serviceEquipeSaude, IServiceAtendimento serviceAtendimento)
    {
        _serviceEquipeSaude = serviceEquipeSaude;
        _serviceAtendimento = serviceAtendimento;
        _servicePainelGestao = servicePainelGestao;
        _serviceCliente = serviceCliente;
    }
    
    public IActionResult Index()
    {
        if (!HasUsuarioGestorLogado())
            return View("Acesso");
        var modelPainel = CarregarPainelGestao(ObterIdUsuarioGestor());
        return View("AreaPainelGestao", modelPainel);
    }

    private AreaPainelGestaoModel CarregarPainelGestao(string idUsuarioGestor)
    {
        var usuarioGestor = _servicePainelGestao.ObterUsuarioGestor(idUsuarioGestor);
        if (usuarioGestor == null)
            return null;
        var model = new AreaPainelGestaoModel();
        model.UsuarioGestor = new UsuarioGestorModel();
        model.UsuarioGestor.Id = usuarioGestor.Id;
        model.UsuarioGestor.Nome = usuarioGestor.Nome;
        model.UsuarioGestor.Email = usuarioGestor.Email;
        //Clientes
        model.QtdClientesRegistrados = _serviceCliente.QtdClientesRegistrados();
        model.QtdClientesEmAtendimento = _serviceAtendimento.QtdClienteEmAtendimento();
        model.QtdClientesFilaAtendimento = _serviceAtendimento.QtdClienteFilaAtendimento();
        //Profissionais de Saúde
        model.QtdProfissionaisOnline = _serviceEquipeSaude.QtdProfissionalSaudeOnline();
        model.QtdProfissionaisLogados = _serviceEquipeSaude.QtdProfissionalSaudeLogados();
        model.QtdProfissionaisEmAtendimento = _serviceEquipeSaude.QtdProfissionalSaudeEmAtendimento();
        //Atendimentos
        model.DataHoraAtendimentos = DateTime.Now;
        model.QtdAtendimentosCancelados = _serviceAtendimento.QtdAtendimentos(SituacaoAtendimentoEnum.Cancelado);
        model.QtdAtendimentosDesistencias = _serviceAtendimento.QtdAtendimentos(SituacaoAtendimentoEnum.Desistencia);
        model.QtdAtendimentosFinalizados = _serviceAtendimento.QtdAtendimentos(SituacaoAtendimentoEnum.Finalizado);
        var tempoMedioAtendimento = _serviceAtendimento.TempoMedioAtendimentos(model.DataHoraAtualizacaoPainel);
        model.TempoMedioAtendimentos = tempoMedioAtendimento.ToString(@"hh\:mm"); 
        var tempoMedioNaFilaAtendimento = _serviceAtendimento.TempoMedioNaFilaAtendimento(model.DataHoraAtualizacaoPainel);
        model.TempoMedioNaFilaAtendimento = tempoMedioNaFilaAtendimento.ToString(@"hh\:mm");
        var notaMediaAtendimento = _serviceAtendimento.NotaMediaAtendimentos(model.DataHoraAtualizacaoPainel);
        model.AvaliacaoMediaAtendimentos = notaMediaAtendimento.ToString("00");
        //Painel
        model.DataHoraAtualizacaoPainel = DateTime.Now;
        return model;
    }

    [HttpPost]
    public IActionResult Acessar(GestorAcessoModel model)
    {
        if (!ModelState.IsValid)
            return View("Acesso", model);
        
        try
        {
            var gestorModel = _servicePainelGestao.AutenticarUsuarioGestor(model.Email, model.Senha);
            if (gestorModel == null)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível autenticar o usuário.");
                return View("Acesso", model);
            }

            GravarIdUsuarioGestor(gestorModel.Id);
            ExibirAlerta($"Bem-vindo, {gestorModel.Nome}");

            var modelPainel = CarregarPainelGestao(gestorModel.Id);
            if (modelPainel == null)
                return View("Acesso", model);
            
            return View("AreaPainelGestao", modelPainel);
        }
        catch (ServiceException erro)
        {
            ModelState.AddModelError(string.Empty, erro.Message);
            return View("Acesso", model);
        }
    }
    
    [HttpGet]
    public IActionResult Sair()
    {
        if (HasUsuarioGestorLogado())
        {
            var idUsuarioGestor = ObterIdUsuarioGestor();
            _servicePainelGestao.LogoffUsuarioGestor(idUsuarioGestor);
            LogoffUsuarioGestorSession();
        }
        return RedirectToAction("Index", "PainelGestao");
    }

    [HttpGet]
    public IActionResult InfoPainelGestao()
    {
        
        if (!HasUsuarioGestorLogado())
            return JsonResultErro("Usuário de gestão não está logado.");
        var modelPainel = CarregarPainelGestao(ObterIdUsuarioGestor());
        return JsonResultSucesso(modelPainel);
        
    }
    
}