namespace videochamada.frontend.Models;

public class ProfissionalSaudeEmAtendimentoModel
{
    public string IdAtendimento { get; set; }
    public string IdCliente { get; set; }
    public string IdProfissionalSaude { get; set; }

    public ClienteModel Cliente { get; set; }
    public ProfissionalSaudeModel ProfissionalSaude { get; set; }
    public ChatAtendimentoModel ChatAtendimento { get; set; }
    
}