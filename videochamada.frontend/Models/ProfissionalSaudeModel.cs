namespace videochamada.frontend.Models;

public class ProfissionalSaudeModel
{
    public string Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string Especialidade { get; set; }
    public string SenhaAcesso { get; set; }
    public bool Online { get; set; }
    public bool EmAtendimento { get; set; }
    
}