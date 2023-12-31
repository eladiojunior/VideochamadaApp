﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Enums;
using VideoChatApp.FrontEnd.Services.Exceptions;
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
        
        var idCliente = ObterIdCliente();
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
        var idCliente = ObterIdCliente();
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
            case SituacaoAtendimentoEnum.VerificacaoDispositivo:
                return RedirectToAction("VerificarDispositivo", "Atendimento");
            case SituacaoAtendimentoEnum.FilaAtendimento:
                return RedirectToAction("FilaAtendimento", "Atendimento");
            case SituacaoAtendimentoEnum.EmAtendimento:
                return RedirectToAction("EmAtendimento", "Atendimento");
            default:
                return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public IActionResult CancelarAtendimento(string idAtendimento)
    {
        
        var atendimento = _serviceAtendimento.ObterAtendimento(idAtendimento);
        if (atendimento == null)
            ExibirErro("Atendimento não encontrado para continuar.");

        var cliente = atendimento.Cliente;
        if (cliente == null)
            ExibirErro("Erro ao identificar o Cliene do atendimento.");

        if (!HasErros())
        {//Cancelar atendimento
            var avaliacaoAtendimento = new AvaliacaoAtendimentoModel();
            avaliacaoAtendimento.IdAtendimento = idAtendimento;
            avaliacaoAtendimento.IdCliente = atendimento.Cliente.Id;
            avaliacaoAtendimento.Comentario = "Cancelado pelo Cliente.";
            _serviceAtendimento.EncerrarAtendimento(idAtendimento, SituacaoAtendimentoEnum.Cancelado);            
        }
        
        ExibirAlerta("Cancelamos seu atendimento em aberto, ficará no seu histórico para consulta.");
        return RedirectToAction("Index", "Home");
        
    }
    
    [HttpGet]
    public IActionResult VerificarDispositivo()
    {
        var idCliente = ObterIdCliente();
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
        var idCliente = ObterIdCliente();
        var modelCliente = _serviceCliente.ObterCliente(idCliente);
        if (modelCliente == null)
            return RedirectToAction("Index", "Home");
        var modelAtendimento = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        if (modelAtendimento.Situacao == SituacaoAtendimentoEnum.EmAtendimento)
            return RedirectToAction("EmAtendimento", "Atendimento");
        
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
        
        var idCliente = ObterIdCliente();
        var atendimentoAberto = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (atendimentoAberto == null)
            return RedirectToAction("Index", "Home");
        
        var modelAvaliacao = new AvaliacaoAtendimentoModel();
        modelAvaliacao.IdCliente = atendimentoAberto.Cliente.Id;
        modelAvaliacao.IdAtendimento = atendimentoAberto.Id;
        modelAvaliacao.HasDesistencia = (atendimentoAberto.Situacao != SituacaoAtendimentoEnum.EmAtendimento);
        
        var situacaoAtendimento = modelAvaliacao.HasDesistencia
            ? SituacaoAtendimentoEnum.Desistencia
            : SituacaoAtendimentoEnum.Finalizado;
        
        _serviceAtendimento.EncerrarAtendimento(atendimentoAberto.Id, situacaoAtendimento);
        
        return View("AvaliarAtendimento", modelAvaliacao);
        
    }

    [HttpPost]
    public IActionResult AvaliarAtendimento(AvaliacaoAtendimentoModel model)
    {
        
        if (!ModelState.IsValid)
            return View("AvaliarAtendimento", model);

        _serviceAtendimento.AvaliarAtendimento(model);
        
        return RedirectToAction("Index", "Home");
        
    }

    [HttpGet]
    public IActionResult PosicaoFilaAtendimento()
    {
        var posicaoAtendimento = new ClientePosicaoAtendimentoModel();
        var idCliente = ObterIdCliente();
        if (!string.IsNullOrEmpty(idCliente))
        {
            posicaoAtendimento.IdCliente = idCliente;
            posicaoAtendimento.PosicaoNaFila = _serviceAtendimento.PosicaoFilaAtendimento(idCliente);
            posicaoAtendimento.QtdClientesFila = _serviceAtendimento.QtdClienteFilaAtendimento();
            posicaoAtendimento.QtdProfissionaisOnline = _serviceEquipeSaude.QtdProfissionalSaudeOnline();
        }
        return JsonResultSucesso(posicaoAtendimento);
    }
    
    [HttpPost]
    public IActionResult SolicitarPrioridade(SolicitacaoPrioridadeAtendimentoModel model)
    {

        return JsonResultSucesso("Solicitação de prioridade está em avalidação.");

    }
    
    [HttpGet]
    public IActionResult EmAtendimento()
    {
        
        var idCliente = ObterIdCliente();
        var cliente = _serviceCliente.ObterCliente(idCliente);
        var atendimentoAberto = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (atendimentoAberto == null)
            return RedirectToAction("Index", "Home");
        
        var model = new UsuarioEmAtendimentoModel();
        model.IdAtendimento = atendimentoAberto.Id;
        model.IdCliente = idCliente;
        model.Cliente = cliente;

        //Recuperar profissional do atendimento...
        var profissionalAtendimento = atendimentoAberto.ProfissionalSaude;
        if (profissionalAtendimento == null)
            return RedirectToAction("FilaAtendimento", "Atendimento");
        model.IdProfissionalSaude = profissionalAtendimento.Id;        
        model.ProfissionalSaude = profissionalAtendimento;

        //Recuperar o histórico de chat do atendimento...
        model.ChatAtendimento = _serviceAtendimento.ObterChatAtendimento(atendimentoAberto.Id);
        
        //Recuperar os arquivos do atendimento...
        model.ArquivosAtendimento = _serviceAtendimento.ListarArquivosAtendimento(atendimentoAberto.Id);
        
        return View("ClienteEmAtendimento", model);
    }

    [HttpPost]
    public IActionResult RecuperarProximoClienteFilaAtendimento(string idProfissional)
    {
        if (string.IsNullOrEmpty(idProfissional))
            return JsonResultErro(new { EmAtendimento = false }, "Id do Profissional de Saúde não informado.");
        var profissional = _serviceEquipeSaude.ObterProfissionalSaude(idProfissional);
        if (profissional == null)
            return JsonResultErro(new { EmAtendimento = false }, $"Profissional de Saúde não encontrado com o ID [{idProfissional}].");
        if (!profissional.Online)
            return JsonResultErro(new { EmAtendimento = false }, "Profissional de Saúde não está disponível (online) para atendimento.");
        if (profissional.EmAtendimento)
        {
            var atendimentoProfissional = _serviceAtendimento.ObterAtendimentoAbertoPorProfissional(idProfissional);
            if (atendimentoProfissional != null)
                return JsonResultErro(new { EmAtendimento = true }, "Profissional de Saúde já em atendimento... aguarde.");
        }
        
        var clienteAtendimento = _serviceAtendimento.ObterProximoClienteAtendimento();
        if (clienteAtendimento == null)
            return JsonResultErro(new { EmAtendimento = false }, "Nenhum cliente na fila de atendimento.");

        var atendimento = _serviceAtendimento.IniciarAtendimentoProfissionalSaude(clienteAtendimento.Id, idProfissional);
        if (atendimento == null)
            return JsonResultErro(new { EmAtendimento = false }, $"Atendimento não aberto para o próximo cliente [{clienteAtendimento.Nome}].");

        return JsonResultSucesso(atendimento, "Próximo cliente recuperado e atendimento em andamento.");

    }
    
    [HttpGet]
    public IActionResult SairDoAtendimento(string idAtendimento)
    {
        
        var atendimento = _serviceAtendimento.ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return RedirectToAction("Index", "Home");

        //Verificar se o atendimento está em andamento...
        if (atendimento.Situacao == SituacaoAtendimentoEnum.EmAtendimento)
        {
            _serviceAtendimento.EncerrarAtendimento(idAtendimento, SituacaoAtendimentoEnum.Finalizado);
        }
            
        var modelAvaliacao = new AvaliacaoAtendimentoModel();
        modelAvaliacao.IdCliente = atendimento.Cliente.Id;
        modelAvaliacao.IdAtendimento = atendimento.Id;
        modelAvaliacao.HasDesistencia = false;
        
        return View("AvaliarAtendimento", modelAvaliacao);
        
    }
    
    [HttpGet]
    public ActionResult DownloadArquivoAtendimento(string idAtendimento, string idArquivo)
    {
        
        var atendimento = _serviceAtendimento.ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return View("Error");
        
        var arquivoAtendimento = _serviceAtendimento.ObterArquivoAtendimento(atendimento.Id, idArquivo);
        if (arquivoAtendimento == null)
            return View("Error");

        return File(arquivoAtendimento.BytesArquivo, 
            arquivoAtendimento.TipoExtensao,
            arquivoAtendimento.NomeOriginal);

    }

    [HttpPost]
    public IActionResult RemoverArquivoAtendimento(string idAtendimento, string idArquivo)
    {
        
        var atendimento = _serviceAtendimento.ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return JsonResultErro($"Atendimento não identificado com o Id {idAtendimento}.");
        
        _serviceAtendimento.RemoverArquivoAtendimento(atendimento.Id, idArquivo);

        return JsonResultSucesso($"Arquivo removido do atendimento.");;
        
    }

    [HttpPost]
    public IActionResult EnviarArquivosAtendimento(ArquivoAtendimentoEnviarModel model)
    {

        if (!ModelState.IsValid)
            return JsonResultErro(ModelState);

        var atendimentoAberto = _serviceAtendimento.ObterAtendimento(model.IdAtendimento);
        if (atendimentoAberto == null)
            return JsonResultErro("Não identificamos um atendimento para registro do arquivo.");

        try
        {
            var arquivoModel = ObterArquivoModel(model.Arquivo);
            arquivoModel.IdUsuario = model.IdUsuario;
            var arquivoRegistrado = _serviceAtendimento.RegistrarArquivoAtendimentoModel(model.IdAtendimento, arquivoModel);
            return JsonResultSucesso(arquivoRegistrado,"Arquivo registrado com sucesso.");
        }
        catch (ServiceException erro)
        {
            return JsonResultErro(erro.Message);
        }

    }

    /// <summary>
    /// Recupera as informações do arquivo (FornFile) enviado da View para extração 
    /// </summary>
    /// <param name="arquivo">Arquivo enviado da View.</param>
    /// <returns></returns>
    private ArquivoAtendimentoModel ObterArquivoModel(IFormFile arquivo)
    {
        var arquivoResult = new ArquivoAtendimentoModel();
        arquivoResult.NomeOriginal = arquivo.FileName;
        arquivoResult.TipoExtensao = arquivo.ContentType;
        using var memoryStream = new MemoryStream();
        arquivo.CopyToAsync(memoryStream);
        arquivoResult.TamanhoBytes = memoryStream.Length;
        arquivoResult.BytesArquivo = memoryStream.ToArray();
        return arquivoResult;
    }
    
    [HttpGet]
    public ActionResult ListarArquivosAtendimento(string idAtendimento, string idUsuario)
    {
        
        if (string.IsNullOrEmpty(idAtendimento))
            return JsonResultErro($"Não identificamos um atendimento com Id {idAtendimento}.");
        
        var listarArquivos = _serviceAtendimento.ListarArquivosAtendimento(idAtendimento);

        ViewData["IdUsuario"] = idUsuario == null ? "" : idUsuario;
        ViewData["IdAtendimento"] = idAtendimento == null ? "" : idAtendimento;
        return JsonResultSucesso(RenderRazorViewToString("_ArquivosAtendimentoPartial", listarArquivos), "Lista de arquivos do atendimento atualizada.");

    }
    
    [HttpPost]
    public IActionResult GravarTextoMotivoAtendimento(string idAtendimento, string textoMotivoAtendimento)
    {
        
        var atendimento = _serviceAtendimento.ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return JsonResultErro($"Atendimento não identificado com o Id {idAtendimento}.");
        
        _serviceAtendimento.GravarTextoMotivoAtendimento(atendimento.Id, textoMotivoAtendimento);
        
        var model = new { DataHoraGravacao = DateTime.Now.ToString("G") };
        return JsonResultSucesso(model, "Texto gravado com sucesso.");
        
    }
    
    
}