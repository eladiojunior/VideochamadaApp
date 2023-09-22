namespace videochamada.frontend.Models;

public class ArquivoAtendimentoEnviarModel
{
    public string IdUsuario { get; set; }
    public string IdAtendimento { get; set; }
    public IFormFile Arquivo { get; set; }
}