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

    public bool HasRetomarAtendimento
    {
        get
        {
            var listaSituacaoAberto = new[]
            {
                SituacaoAtendimentoEnum.Registrado, 
                SituacaoAtendimentoEnum.EmAtendimento,
                SituacaoAtendimentoEnum.FilaAtendimento
            };
            return listaSituacaoAberto.Contains(Situacao);
        }
    }

    public int Nota { get; set; }
    public string ComentarioNota { get; set; }
    
    public ProfissionalSaudeModel ProfissionalSaude { get; set; }
}