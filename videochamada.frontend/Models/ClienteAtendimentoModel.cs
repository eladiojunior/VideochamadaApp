namespace videochamada.frontend.Models;

public class ClienteAtendimentoModel
{
    public string IdCliente { get; set; }
    public ClienteModel Cliente { get; set; }
    public string IdAtendimento { get; set; }
    public AtendimentoModel Atendimento { get; set; }
}