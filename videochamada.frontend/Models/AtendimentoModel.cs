using VideoChatApp.FrontEnd.Services.Enums;

namespace videochamada.frontend.Models;

public class AtendimentoModel
{
    public string Id { get; set; }
    public string IdCliente { get; set; }
    public DateTime DataRegistro { get; set; }
    public DateTime? DataInicial { get; set; }
    public DateTime? DataFinal { get; set; }
    public SituacaoAtendimentoEnum Situacao { get; set; }
    public bool HasTermoUso { get; set; }
    
    //Geolocalização do Cliente no Atendimento
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string IpMaquinaCliente { get; set; }

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
}