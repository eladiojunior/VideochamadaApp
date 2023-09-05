using VideoChatApp.FrontEnd.Services.Enums;

namespace videochamada.frontend.Models;

public class HistoricoSituacaoAtendimentoModel
{
    public HistoricoSituacaoAtendimentoModel(string idAtendimento, SituacaoAtendimentoEnum situacaoAtendimento)
    {
        IdAtendimento = idAtendimento;
        Situacao = situacaoAtendimento;
        DataRegistro = DateTime.Now;
    }
    
    public HistoricoSituacaoAtendimentoModel(string idAtendimento, SituacaoAtendimentoEnum situacaoAtendimento, string textoHistorico)
    {
        IdAtendimento = idAtendimento;
        Situacao = situacaoAtendimento;
        DataRegistro = DateTime.Now;
        TextoHistorico = textoHistorico;
    }
    
    public string IdAtendimento { get; set; }
    public DateTime DataRegistro { get; set; }
    public SituacaoAtendimentoEnum Situacao { get; set; }
    public string TextoHistorico { get; set; }
}