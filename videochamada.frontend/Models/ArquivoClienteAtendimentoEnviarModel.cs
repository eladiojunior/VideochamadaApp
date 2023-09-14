namespace videochamada.frontend.Models;

public class ArquivoClienteAtendimentoEnviarModel
{
    public string IdCliente { get; set; }
    public string IdAtendimento { get; set; }
    public IFormFile Arquivo { get; set; }
}