using videochamada.frontend.Models;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServiceUsuario
{
    UsuarioAcessoModel RegistrarUsuarioAcesso(UsuarioAcessoModel usuario);
    UsuarioAcessoModel ObterUsuarioAcesso(string idUsuario);
    UsuarioAcessoModel ObterUsuarioAcessoPorEmail(string idUsuario);
    List<UsuarioAcessoModel> ListarUsuarioAcesso();
    
}