namespace videochamada.frontend.Models;

public class UsuarioHubModel
{
    public UsuarioHubModel(string idConnectionHub, string idAtendimento)
    {
        IdConnectionHub = idConnectionHub;
        IdAtendimento = idAtendimento;
    }
    public string IdConnectionHub { get; set; }
    public string IdAtendimento { get; set; }
}