namespace videochamada.frontend.Models;

public class UsuarioLogadoModel
{
    public string IdUsuario { get; set; }
    public DateTime DataHoraLogin { get; set; }
    public DateTime DataHoraUltimaVerificacao { get; set; }
    public string IpMaquinaUsuario { get; set; }
}