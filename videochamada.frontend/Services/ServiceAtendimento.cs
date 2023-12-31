﻿using System.Collections.Concurrent;
using videochamada.frontend.Helper;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Enums;
using VideoChatApp.FrontEnd.Services.Exceptions;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Services;

public class ServiceAtendimento : IServiceAtendimento
{
    private static readonly ConcurrentDictionary<string, AtendimentoModel> _atendimentos = new ConcurrentDictionary<string, AtendimentoModel>();
    private IServiceCliente _serviceCliente;
    private IServiceEquipeSaude _serviceEquipeSaude;
    private IListenerServerClient _listenerServerClient;
    private SituacaoAtendimentoEnum[] situacoesEncerradas = 
        { SituacaoAtendimentoEnum.Finalizado, SituacaoAtendimentoEnum.Cancelado, SituacaoAtendimentoEnum.Desistencia };

    public ServiceAtendimento(IServiceCliente serviceCliente, IListenerServerClient listenerServerClient, IServiceEquipeSaude serviceEquipeSaude)
    {
        _serviceCliente = serviceCliente;
        _listenerServerClient = listenerServerClient;
        _serviceEquipeSaude = serviceEquipeSaude;
    }
    
    public AtendimentoModel CriarAtendimento(NovoAtendimentoModel model)
    {
        var cliente = _serviceCliente.ObterCliente(model.IdCliente);
        if (cliente == null)
            return null;
        
        var atendimento = new AtendimentoModel();
        atendimento.Id = ServiceHelper.GerarId(); //Gerar ID
        atendimento.Cliente = cliente;
        atendimento.Situacao = SituacaoAtendimentoEnum.Registrado;
        atendimento.DataRegistro = DateTime.Now;
        atendimento.HasTermoUso = model.HasTermoUso;
        atendimento.DataInicial = null;
        atendimento.DataFinal = null;
        atendimento.Latitude = Convert.ToDouble(model.Latitude.Replace(".",","));
        atendimento.Longitude = Convert.ToDouble(model.Longitude.Replace(".",","));
        atendimento.IpMaquinaUsuario = _listenerServerClient.IpMaquinaUsuario();

        //Registrar histórico de situação.
        atendimento.AddHistorico(new HistoricoSituacaoAtendimentoModel(atendimento.Id, atendimento.Situacao));
        
        _atendimentos.TryAdd(atendimento.Id, atendimento);
        
        return atendimento;
        
    }

    public AtendimentoModel ObterAtendimentoAberto(string idCliente)
    {
        
        var clienteModel = _serviceCliente.ObterCliente(idCliente);
        if (clienteModel == null)
            return null;
        
        var listaSituacaoAberto = new[]
        {
            SituacaoAtendimentoEnum.Registrado, 
            SituacaoAtendimentoEnum.EmAtendimento,
            SituacaoAtendimentoEnum.VerificacaoDispositivo,
            SituacaoAtendimentoEnum.FilaAtendimento
        };
        
        var atendimentoAberto = _atendimentos.Values.FirstOrDefault(c => 
            c.Cliente.Id.Equals(clienteModel.Id) && listaSituacaoAberto.Contains(c.Situacao));
        if (atendimentoAberto == null)
            return null;
        
        return atendimentoAberto;
        
    }

    public AtendimentoModel EntrarFilaAtendimento(AtendimentoModel atendimento)
    {
        var cliente = _serviceCliente.ObterCliente(atendimento.Cliente.Id);
        if (cliente == null)
            return null;
        var posicaoFila = GerenciadorFilaCliente.Get().PosicaoNaFila(cliente.Id);
        if (posicaoFila == 0) 
        {//Registrar cliente na fila...
            AtualizarSituacaoAtendimento(atendimento.Id, SituacaoAtendimentoEnum.FilaAtendimento);
            GerenciadorFilaCliente.Get().EntrarNaFila(cliente);
        }
        return atendimento;
    }

    private void AtualizarSituacaoAtendimento(string idAtendimento, SituacaoAtendimentoEnum situacaoAtendimento)
    {
        var atendimento = _atendimentos.GetValueOrDefault(idAtendimento);
        if (atendimento != null)
        {
            //Registrar histórico de situação.
            atendimento.AddHistorico(new HistoricoSituacaoAtendimentoModel(idAtendimento, situacaoAtendimento));
            atendimento.Situacao = situacaoAtendimento;
        }
            
    }

    public int PosicaoFilaAtendimento(string idCliente)
    {
        return GerenciadorFilaCliente.Get().PosicaoNaFila(idCliente);
    }

    public void EncerrarAtendimento(string idAtendimento, SituacaoAtendimentoEnum situacaoAtendimento)
    {
        var atendimento = _atendimentos.GetValueOrDefault(idAtendimento);
        if (atendimento == null)
            return;

        atendimento.DataFinal = DateTime.Now;
        atendimento.Situacao = situacaoAtendimento;

        //Registrar histórico...
        AtualizarSituacaoAtendimento(atendimento.Id, situacaoAtendimento);
        
        VerificarRemoverClienteFilaAtendimento(atendimento.Cliente.Id);
        
    }

    public void AvaliarAtendimento(AvaliacaoAtendimentoModel avaliarAtendimento)
    {
        var atendimento = _atendimentos.GetValueOrDefault(avaliarAtendimento.IdAtendimento);
        if (atendimento == null)
            return;

        atendimento.Nota = avaliarAtendimento.Nota;
        atendimento.ComentarioNota = avaliarAtendimento.Comentario;
        
    }

    private void VerificarRemoverClienteFilaAtendimento(string idCliente)
    {

        var posicaoNaFila = GerenciadorFilaCliente.Get().PosicaoNaFila(idCliente);
        if (posicaoNaFila != 0)
            GerenciadorFilaCliente.Get().RemoverDaFila(idCliente);

    }

    public AtendimentoModel IniciarAtendimentoProfissionalSaude(string idCliente, string idProfissional)
    {
        
        var atendimento = ObterAtendimentoAberto(idCliente);
        if (atendimento == null)
            throw new ServiceException($"Nenhum atendimento encontrado com para o Cliente ID {idCliente}");
        
        if (situacoesEncerradas.Contains(atendimento.Situacao))
            throw new ServiceException($"Atendimento encerrado [{atendimento.Situacao.ObterTextoEnum()}] não será possível iniciar.");

        var profissional = _serviceEquipeSaude.ObterProfissionalSaude(idProfissional);
        if (profissional == null)
            throw new ServiceException($"Profissional de saúde não encontrado com o ID {idProfissional}");
        
        //Atualizar atendimento...
        profissional.EmAtendimento = true;
        profissional.Online = true;
        atendimento.ProfissionalSaude = profissional;
        atendimento.DataInicial = DateTime.Now;
        
        AtualizarSituacaoAtendimento(atendimento.Id, SituacaoAtendimentoEnum.EmAtendimento);

        return atendimento;
        
    }

    public List<AtendimentoModel> ListarAtendimentosProfissional(string idProfissional, bool hasRealizados)
    {
        var lista = hasRealizados ? 
                _atendimentos.Values.Where(w => w.ProfissionalSaude != null && w.ProfissionalSaude.Id.Equals(idProfissional) && w.Situacao == SituacaoAtendimentoEnum.Finalizado).ToList() :
                _atendimentos.Values.Where(w => w.ProfissionalSaude != null && w.ProfissionalSaude.Id.Equals(idProfissional) && w.Situacao == SituacaoAtendimentoEnum.EmAtendimento).ToList();
        return lista;
    }

    public int QtdClienteFilaAtendimento()
    {
        return GerenciadorFilaCliente.Get().QuantidadeClientesFila();
    }

    public int QtdClienteEmAtendimento()
    {
        return _atendimentos.Values.Count(w => w.ProfissionalSaude != null && w.Situacao == SituacaoAtendimentoEnum.EmAtendimento);
    }

    public List<AtendimentoModel> ListarAtendimentosCliente(string idCliente)
    {
        return _atendimentos.Values.Where(w => w.Cliente.Id.Equals(idCliente)).OrderByDescending(o => o.DataRegistro).ToList();
    }

    public AtendimentoModel ObterAtendimento(string idAtendimento)
    {
        var atendimento = _atendimentos.Values.FirstOrDefault(c => c.Id.Equals(idAtendimento));
        if (atendimento == null)
            return null;
        return atendimento;
    }

    public void VerificarDispositivoParaAtendimento(string idAtendimento)
    {
        if (string.IsNullOrEmpty(idAtendimento)) 
            return;
        AtualizarSituacaoAtendimento(idAtendimento, SituacaoAtendimentoEnum.VerificacaoDispositivo);
    }
    
    public int QtdAtendimentos(SituacaoAtendimentoEnum filtroSituacao)
    {
        var qtd = _atendimentos.Values.Count(w => w.Situacao == filtroSituacao);
        return qtd;
    }

    public TimeSpan TempoMedioAtendimentos(DateTime filtroDatahora)
    {
        
        //TMA = Tempo Médio de Atendimento
        //Cálculo: Tempo Total na Fila + Tempo Total de Atendimento / Qtd Total Adendimento
        
        var listaAtendimentos = _atendimentos.Values.Where(
            w => w.DataRegistro.Date >= filtroDatahora.Date && situacoesEncerradas.Contains(w.Situacao));
        
        var qtdAtendimentos = listaAtendimentos.Count();
        if (qtdAtendimentos == 0)
            return new TimeSpan(0,0,0);

        var tempoEmAtendimento = CalcularTempoMedioAtendimento(listaAtendimentos, SituacaoAtendimentoEnum.EmAtendimento);
        var tempoNaFilaAtendimento = CalcularTempoMedioAtendimento(listaAtendimentos, SituacaoAtendimentoEnum.FilaAtendimento);
        var tempoTotalAtendimento = tempoEmAtendimento + tempoNaFilaAtendimento;
        var tma = new TimeSpan(tempoTotalAtendimento.Ticks / qtdAtendimentos);

        return tma;
        
    }
    
    private TimeSpan CalcularTempoMedioAtendimento(IEnumerable<AtendimentoModel> listaAtendimentos, SituacaoAtendimentoEnum situacaoCalculo)
    {
        var tempoTotalAtendimento = new TimeSpan(0,0,0);
        DateTime? dataHoraInicial = null;
        foreach (var atendimento in listaAtendimentos)
        {
            var listaHistorico = atendimento.HistoricoSituacaoAtendimento;
            if (listaHistorico == null || !listaHistorico.Any())
                continue;
            foreach (var historico in listaHistorico.OrderBy(o => o.DataRegistro))
            {
                if (dataHoraInicial.HasValue && situacoesEncerradas.Contains(historico.Situacao))
                {//Calcular tempo...
                    var dataHoraFinal = historico.DataRegistro;
                    var tempoEspera = CalcularTempoEspera(dataHoraInicial.Value, dataHoraFinal);
                    tempoTotalAtendimento = tempoTotalAtendimento.Add(tempoEspera);
                    dataHoraInicial = null;
                } else if (!dataHoraInicial.HasValue && historico.Situacao == situacaoCalculo)
                    dataHoraInicial = historico.DataRegistro;
            }
        }
        return tempoTotalAtendimento;
    }

    private TimeSpan CalcularTempoEspera(DateTime dataHoraInicial, DateTime dataHoraFinal)
    {
        return (dataHoraFinal - dataHoraInicial);
    }

    public TimeSpan TempoMedioNaFilaAtendimento(DateTime filtroDatahora)
    {
        //TMF - Tempo Médio na Fila
        //Cálculo: Tempo Total na Fila / Qtd Total Adendimento
        
        var listaAtendimentos = _atendimentos.Values.Where(
            w => w.DataRegistro.Date >= filtroDatahora.Date && situacoesEncerradas.Contains(w.Situacao));
        
        var qtdAtendimentos = listaAtendimentos.Count();
        if (qtdAtendimentos == 0)
            return new TimeSpan(0,0,0);

        var tempoTotalFila = CalcularTempoMedioAtendimento(listaAtendimentos, SituacaoAtendimentoEnum.FilaAtendimento);
        var tmf = new TimeSpan(tempoTotalFila.Ticks / qtdAtendimentos);

        return tmf;
        
    }

    public int NotaMediaAtendimentos(DateTime? filtroDatahora)
    {
        
        var listaAtendimentos =
            (filtroDatahora.HasValue ? 
            _atendimentos.Values.Where(w => w.DataRegistro.Date >= filtroDatahora.Value.Date && situacoesEncerradas.Contains(w.Situacao)) :
            _atendimentos.Values.Where(w => situacoesEncerradas.Contains(w.Situacao)));

        var qtdAtendimentos = listaAtendimentos.Count(c => c.Nota != 0); //Realizou avalidação do atendimento.
        if (qtdAtendimentos == 0)
            return 0;
        
        var totalNotas = listaAtendimentos.Sum(atendimento => atendimento.Nota);
        return totalNotas / qtdAtendimentos;
        
    }

    public ArquivoAtendimentoModel ObterArquivoAtendimento(string idAtendimento, string idArquivo)
    {
        if (string.IsNullOrEmpty(idAtendimento) || string.IsNullOrEmpty(idArquivo))
            return null;
        var atendimento = ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return null;
        if (atendimento.ArquivosAtendimento == null || !atendimento.ArquivosAtendimento.Any())
            return null;
        var arquivo = atendimento.ArquivosAtendimento.FirstOrDefault(w => w.Id.Equals(idArquivo));
        return arquivo;
    }

    public void RemoverArquivoAtendimento(string idAtendimento, string idArquivo)
    {
        var arquivo = ObterArquivoAtendimento(idAtendimento, idArquivo);
        if (arquivo == null) return;
        var atendimento = ObterAtendimento(idAtendimento);
        atendimento.ArquivosAtendimento.Remove(arquivo);
    }

    public List<ArquivoAtendimentoModel> ListarArquivosAtendimento(string idAtendimento)
    {
        if (string.IsNullOrEmpty(idAtendimento))
            return null;
        var atendimento = ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return null;
        return atendimento.ArquivosAtendimento;
    }

    public ArquivoAtendimentoModel RegistrarArquivoAtendimentoModel(string idAtendimento, ArquivoAtendimentoModel arquivo)
    {
        if (string.IsNullOrEmpty(idAtendimento))
            return null;
        var atendimento = ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return null;
        //Atualizar informação do Arquivo...
        arquivo.Id = ServiceHelper.GerarId();
        arquivo.NomeFisico = GerarNomeArquivoFisico(arquivo);
        arquivo.DataHoraEnvio = DateTime.Now;
        atendimento.AddArquivo(arquivo);
        return arquivo;
    }

    public ClienteModel ObterProximoClienteAtendimento()
    {
        return GerenciadorFilaCliente.Get().AtenderProximoCliente();
        
    }

    public async Task<MensagemChatModel> RegistrarMensagemChatAtendimento(string idAtendimento, string idUsuario, string mensagem)
    {
        
        var atendimento = ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return null;
        
        var mensagemChat = new MensagemChatModel();
        mensagemChat.IdUsuario = idUsuario;
        mensagemChat.UsuarioUsuarioOrigem = ObterTipoOrigemUsuario(atendimento, idUsuario);
        mensagemChat.DataHoraEnvio = DateTime.Now;
        mensagemChat.Texto = mensagem;
        mensagemChat.IpMaquinaUsuario = _listenerServerClient.IpMaquinaUsuario();
        mensagemChat.Latitude = atendimento.Latitude;
        mensagemChat.Longitude = atendimento.Longitude;
        
        //Registrar a mensagem.
        if (atendimento.ChatAtendimento == null)
            atendimento.ChatAtendimento = new ChatAtendimentoModel();
        atendimento.ChatAtendimento.AddMensagem(mensagemChat);

        return mensagemChat;
    }

    private UsuarioOrigemMensagemEnum ObterTipoOrigemUsuario(AtendimentoModel atendimento, string idUsuario)
    {
        var hasProfissional = (atendimento.ProfissionalSaude!.Id.Equals(idUsuario));
        return (hasProfissional ? UsuarioOrigemMensagemEnum.ProfissionalSaude : UsuarioOrigemMensagemEnum.Cliente);
    }

    private string GerarNomeArquivoFisico(ArquivoAtendimentoModel arquivo)
    {
        var posInit = arquivo.NomeOriginal.IndexOf(".");
        var posTamanho = arquivo.NomeOriginal.Length - posInit;
        var extencao = arquivo.NomeOriginal.Substring(posInit, posTamanho);
        return $"{arquivo.Id}{extencao}";
    }

    public ChatAtendimentoModel ObterChatAtendimento(string idAtendimento)
    {
        if (string.IsNullOrEmpty(idAtendimento))
            return null;
        var atendimento = ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return null;
        return atendimento.ChatAtendimento;
    }

    public void FinalizarAtendimento(string idAtendimento)
    {
        
        var atendimento = ObterAtendimento(idAtendimento);
        if (atendimento == null)
            throw new ServiceException($"Atendimento [{idAtendimento}] não encontrado.");
        
        //Verificar se o atendimento está em andamento...
        if (atendimento.Situacao != SituacaoAtendimentoEnum.EmAtendimento)
            return;

        atendimento.DataFinal = DateTime.Now;
        var idProfissional = atendimento.ProfissionalSaude?.Id;
        if (string.IsNullOrEmpty(idProfissional))
            return;

        var profissionalSaude = _serviceEquipeSaude.ObterProfissionalSaude(idProfissional);
        if (profissionalSaude != null)
        {
            profissionalSaude.EmAtendimento = false;
        }
        
    }

    /// <summary>
    /// Recuperar um Atendimento Aberto que o Profissional de Saúde seja o Id informado.
    /// </summary>
    /// <param name="idProfissional">Identificador do profissional de saúde.</param>
    /// <returns></returns>
    public AtendimentoModel ObterAtendimentoAbertoPorProfissional(string idProfissional)
    {
        var atendimento = _atendimentos.Values
            .FirstOrDefault(c => 
                c.Situacao == SituacaoAtendimentoEnum.EmAtendimento && 
                c.ProfissionalSaude!=null && c.ProfissionalSaude.Id.Equals(idProfissional));
        return atendimento ?? null;
    }

    /// <summary>
    /// Grava o texto do motivo do atendimento, um histórico do atendimento do cliente.
    /// </summary>
    /// <param name="idAtendimento">Identificador do atendimento.</param>
    /// <param name="textoMotivoAtendimento">Texto do Motivo do Atendimento.</param>
    public void GravarTextoMotivoAtendimento(string idAtendimento, string textoMotivoAtendimento)
    {
        
        var atendimento = ObterAtendimento(idAtendimento);
        if (atendimento == null)
            return;
        
        atendimento.TextoMotivoAtendimento = textoMotivoAtendimento;
        
    }
}