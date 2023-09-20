using Microsoft.AspNetCore.Mvc;
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
                return RedirectToAction("InicioAtendimento", "Atendimento");
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
            _serviceAtendimento.EncerrarAtendimento(avaliacaoAtendimento, SituacaoAtendimentoEnum.Cancelado);            
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
        var modelAtendimento = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (modelAtendimento == null)
            return RedirectToAction("Index", "Home");
        
        var modelAvaliacao = new AvaliacaoAtendimentoModel();
        modelAvaliacao.IdCliente = modelAtendimento.Cliente.Id;
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
        
        var model = new ClenteEmAtendimentoModel();
        model.IdAtendimento = atendimentoAberto.Id;
        model.IdCliente = idCliente;
        model.Cliente = cliente;
        model.Cliente.Arquivos = _serviceAtendimento.ListarArquivosAtendimento(atendimentoAberto.Id);

        //Recuperar profissional do atendimento...
        var profissionalAtendimento = atendimentoAberto.ProfissionalSaude;
        if (profissionalAtendimento == null)
            return RedirectToAction("FilaAtendimento", "Atendimento");
        model.IdProfissionalSaude = profissionalAtendimento.Id;        
        model.ProfissionalSaude = profissionalAtendimento;

        //Recuperar o histórico de chat do atendimento...
        model.ChatAtendimento = _serviceAtendimento.ObterChatAtendimento(atendimentoAberto.Id);
        
        return View("ClienteEmAtendimento", model);
    }

    [HttpGet]
    public IActionResult RecuperarProximoClienteFilaAtendimento(string idProfissional)
    {
        
        if (string.IsNullOrEmpty(idProfissional))
            return JsonResultErro("Id do Profissional de Saúde não informado.");
        var profissional = _serviceEquipeSaude.ObterProfissionalSaude(idProfissional);
        if (profissional == null)
            return JsonResultErro($"Profissional de Saúde não encontrado com o ID [{idProfissional}].");
        if (!profissional.Online)
            return JsonResultErro("Profissional de Saúde não está disponível (online) para atendimento.");

        var clienteAtendimento = _serviceAtendimento.ObterProximoClienteAtendimento();
        if (clienteAtendimento == null)
            return JsonResultErro("Nenhum Cliente na fila de atendimento.");

        var atendimento = _serviceAtendimento.IniciarAtendimentoProfissionalSaude(clienteAtendimento.Id, idProfissional);
        if (atendimento == null)
            return JsonResultErro($"Atendimento não aberto para o próximo Cliente [{clienteAtendimento.Nome}].");

        return JsonResultSucesso(atendimento, "Próximo cliente recuperado e atendimento em andamento.");

    }
    
    [HttpGet]
    public IActionResult SairDoAtendimento()
    {
        
        var idCliente = ObterIdCliente();
        var cliente = _serviceCliente.ObterCliente(idCliente);
        if (cliente == null)
            return RedirectToAction("Index", "Home");
        
        var atendimentoAberto = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (atendimentoAberto == null)
            return RedirectToAction("Index", "Home");

        //Verificar se o atendimento está em andamento...
        if (atendimentoAberto.Situacao != SituacaoAtendimentoEnum.EmAtendimento)
            return RedirectToAction("Index", "Home");
            
        var modelAvaliacao = new AvaliacaoAtendimentoModel();
        modelAvaliacao.IdCliente = atendimentoAberto.Cliente.Id;
        modelAvaliacao.IdAtendimento = atendimentoAberto.Id;
        modelAvaliacao.HasDesistencia = true;
        
        return View("AvaliarAtendimento", modelAvaliacao);
        
    }
    
    [HttpGet]
    public ActionResult DownloadArquivoAtendimento(string idArquivo)
    {
        
        var idCliente = ObterIdCliente();
        if (string.IsNullOrEmpty(idCliente))
            return View("Error");
        
        var cliente = _serviceCliente.ObterCliente(idCliente);
        if (cliente == null)
            return View("Error");
        
        var atendimentoAberto = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (atendimentoAberto == null)
            return View("Error");
        
        var arquivoAtendimento = _serviceAtendimento.ObterArquivoAtendimento(atendimentoAberto.Id, idArquivo);
        if (arquivoAtendimento == null)
            return View("Error");

        return File(arquivoAtendimento.BytesArquivo, arquivoAtendimento.TipoExtensao,
            arquivoAtendimento.NomeOriginal);

    }

    [HttpPost]
    public IActionResult RemoverArquivoAtendimento(string idArquivo)
    {
        
        var idCliente = ObterIdCliente();
        if (string.IsNullOrEmpty(idCliente))
            return JsonResultErro("Id do cliente não identificado.");
        var cliente = _serviceCliente.ObterCliente(idCliente);
        if (cliente == null)
            return JsonResultErro($"Cliente não identificado com o Id {idCliente}.");
        var atendimentoAberto = _serviceAtendimento.ObterAtendimentoAberto(idCliente);
        if (atendimentoAberto == null)
            return JsonResultErro($"Atendimento não identificado para o Cliente Id {idCliente}.");
        
        _serviceAtendimento.RemoverArquivoAtendimento(atendimentoAberto.Id, idArquivo);

        return JsonResultSucesso($"Arquivo removido do atendimento.");;
        
    }

    [HttpPost]
    public IActionResult EnviarArquivosAtendimento(ArquivoClienteAtendimentoEnviarModel model)
    {

        if (!ModelState.IsValid)
            return JsonResultErro(ModelState);

        var cliente = _serviceCliente.ObterCliente(model.IdCliente);
        if (cliente == null)
            return JsonResultErro("Cliente não identificado.");

        var atendimentoAberto = _serviceAtendimento.ObterAtendimento(model.IdAtendimento);
        if (atendimentoAberto == null)
            return JsonResultErro("Não identificamos um atendimento para registro do arquivo.");

        try
        {
            var arquivoModel = ObterArquivoModel(model.Arquivo);
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
    private ArquivoClienteAtendimentoModel ObterArquivoModel(IFormFile arquivo)
    {
        var arquivoResult = new ArquivoClienteAtendimentoModel();
        arquivoResult.NomeOriginal = arquivo.FileName;
        arquivoResult.TipoExtensao = arquivo.ContentType;
        using var memoryStream = new MemoryStream();
        arquivo.CopyToAsync(memoryStream);
        arquivoResult.TamanhoBytes = memoryStream.Length;
        arquivoResult.BytesArquivo = memoryStream.ToArray();
        return arquivoResult;
    }
    
    [HttpGet]
    public ActionResult ListarArquivosAtendimento(string idAtendimento)
    {
        
        if (string.IsNullOrEmpty(idAtendimento))
            return JsonResultErro($"Não identificamos um atendimento com Id {idAtendimento}.");
        
        var listarArquivos = _serviceAtendimento.ListarArquivosAtendimento(idAtendimento);

        return JsonResultSucesso(RenderRazorViewToString("_ClienteArquivosAtendimentoPartial", listarArquivos), "Lista de arquivos do atendimento atualizada.");

    }
    
}