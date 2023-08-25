using System.Collections.Concurrent;
using videochamada.frontend.Helper;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Enums;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Services;

public class ServiceAtendimento : IServiceAtendimento
{
    private static readonly ConcurrentDictionary<string, AtendimentoModel> _atendimentos = new ConcurrentDictionary<string, AtendimentoModel>();
    private IServiceCliente _serviceCliente;

    public ServiceAtendimento(IServiceCliente serviceCliente)
    {
        _serviceCliente = serviceCliente;
    }
    
    public AtendimentoModel CriarAtendimento(string idCliente)
    {
        var cliente = _serviceCliente.ObterCliente(idCliente);
        if (cliente == null)
            return null;
        
        var atendimento = new AtendimentoModel();
        atendimento.IdCliente = cliente.Id;
        atendimento.Id = ServiceHelper.GerarId(); //Gerar ID
        atendimento.Situacao = SituacaoAtendimentoEnum.Registrado;
        atendimento.DataRegistro = DateTime.Now;  
        atendimento.DataInicial = null;
        atendimento.DataFinal = null;
        
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
            c.IdCliente.Equals(clienteModel.Id) && listaSituacaoAberto.Contains(c.Situacao) && c.DataFinal == null);
        if (atendimentoAberto == null)
            return null;
        
        return atendimentoAberto;
        
    }

    public AtendimentoModel EntrarFilaAtendimento(AtendimentoModel atendimento)
    {
        var clienteModel = _serviceCliente.ObterCliente(atendimento.IdCliente);
        if (clienteModel == null)
            return null;
        var posicaoFila = GerenciadorFilaCliente.Get().PosicaoNaFila(atendimento.IdCliente);
        if (posicaoFila == 0) 
        {//Registrar cliente na fila...
            AtualizarSituacaoAtendimento(atendimento.Id, SituacaoAtendimentoEnum.FilaAtendimento);
            GerenciadorFilaCliente.Get().EntrarNaFila(clienteModel);
        }
        return atendimento;
    }

    private void AtualizarSituacaoAtendimento(string idAtendimento, SituacaoAtendimentoEnum situacaoAtendimento)
    {
        var atendimento = _atendimentos.GetValueOrDefault(idAtendimento);
        if (atendimento != null)
            atendimento.Situacao = situacaoAtendimento;
    }

    public int PosicaoFilaAtendimento(string idCliente)
    {
        return GerenciadorFilaCliente.Get().PosicaoNaFila(idCliente);
    }

    public void EncerrarAtendimento(AvaliacaoAtendimentoModel atendimento, SituacaoAtendimentoEnum situacaoAtendimento)
    {
        var clienteModel = _serviceCliente.ObterCliente(atendimento.IdCliente);
        if (clienteModel == null)
            return;

        var atendimentoModel = _atendimentos.GetValueOrDefault(atendimento.IdAtendimento);
        if (atendimentoModel == null)
            return;
        
        atendimentoModel.DataFinal = DateTime.Now;
        atendimentoModel.Situacao = situacaoAtendimento;
        atendimentoModel.Nota = atendimento.Nota;
        atendimentoModel.ComentarioNota = atendimento.Comentario;

        VerificarRemoverClienteFilaAtendimento(atendimento.IdCliente);
        
    }

    private void VerificarRemoverClienteFilaAtendimento(string idCliente)
    {

        var posicaoNaFila = GerenciadorFilaCliente.Get().PosicaoNaFila(idCliente);
        if (posicaoNaFila != 0)
            GerenciadorFilaCliente.Get().RemoverDaFila(idCliente);

    }

    public AtendimentoModel IniciarAtendimentoProfissionalSaude(AtendimentoModel atendimento, ProfissionalSaudeModel profissional)
    {
        throw new NotImplementedException();
    }

    public int QtdClienteFilaAtendimento()
    {
        return GerenciadorFilaCliente.Get().QuantidadeClientesFila();
    }

    public List<AtendimentoModel> ListarAtendimentosCliente(string idCliente)
    {
        return _atendimentos.Values.Where(w => w.IdCliente.Equals(idCliente)).OrderByDescending(o => o.DataRegistro).ToList();
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
}