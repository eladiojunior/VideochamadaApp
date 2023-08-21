namespace videochamada.frontend.Models;

public class ClienteModel
{
    public string Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    
    public List<AtendimentoModel> Atendimentos { get; set; }
}