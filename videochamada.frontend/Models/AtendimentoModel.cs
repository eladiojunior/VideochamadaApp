using VideoChatApp.FrontEnd.Services.Enums;

namespace videochamada.frontend.Models;

public class AtendimentoModel
{
    public string Id { get; set; }
    public ClienteModel Cliente { get; set; }
    public DateTime DataRegistro { get; set; }
    public DateTime? DataInicial { get; set; }
    public DateTime? DataFinal { get; set; }
    public SituacaoAtendimentoEnum Situacao { get; set; }
    public bool HasTermoUso { get; set; }
    
    //Geolocalização do Cliente no Atendimento
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string IpMaquinaUsuario { get; set; }

    public bool HasRetomarAtendimento
    {
        get
        {
            var listaSituacaoAberto = new[]
            {
                SituacaoAtendimentoEnum.Registrado, 
                SituacaoAtendimentoEnum.EmAtendimento,
                SituacaoAtendimentoEnum.VerificacaoDispositivo,
                SituacaoAtendimentoEnum.FilaAtendimento
            };
            return listaSituacaoAberto.Contains(Situacao);
        }
    }

    public int Nota { get; set; }
    public string ComentarioNota { get; set; }
    
    public ProfissionalSaudeModel ProfissionalSaude { get; set; }
    
    //Histórico de Situacao do Atendimento
    public List<HistoricoSituacaoAtendimentoModel> HistoricoSituacaoAtendimento { get; set; }
    
    //Lista de arquivos do cliente no atendimento
    public List<ArquivoAtendimentoModel> ArquivosAtendimento { get; set; }
    public ChatAtendimentoModel ChatAtendimento { get; set; }

    public String TempoAtendimento {
        get
        {
            if (!DataFinal.HasValue || !DataInicial.HasValue)
                return "00:00";
            var tempoAtendimento = (DataFinal.Value - DataInicial.Value);
            return tempoAtendimento.ToString(@"hh\:mm");
        }
    }
    internal void AddHistorico(HistoricoSituacaoAtendimentoModel historicoSituacaoAtendimento)
    {
        if (HistoricoSituacaoAtendimento == null)
            HistoricoSituacaoAtendimento = new List<HistoricoSituacaoAtendimentoModel>();
        HistoricoSituacaoAtendimento.Add(historicoSituacaoAtendimento);
    }
    
    internal void AddArquivo(ArquivoAtendimentoModel arquivoAtendimento)
    {
        if (ArquivosAtendimento == null)
            ArquivosAtendimento = new List<ArquivoAtendimentoModel>();
        ArquivosAtendimento.Add(arquivoAtendimento);
    }

}