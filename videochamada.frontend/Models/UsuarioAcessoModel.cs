using VideoChatApp.FrontEnd.Services.Enums;

namespace videochamada.frontend.Models;

public class UsuarioAcessoModel
{
    public string Id { get; set; }
    public TipoUsuarioEnum Tipo { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
}