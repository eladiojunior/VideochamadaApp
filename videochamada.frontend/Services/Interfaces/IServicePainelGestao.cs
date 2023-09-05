using videochamada.frontend.Models;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServicePainelGestao
{
    UsuarioGestorModel AutenticarUsuarioGestor(string email, string senha);
    UsuarioGestorModel ObterUsuarioGestor(string id);
    UsuarioGestorModel ObterUsuarioGestorPorEmail(string email);
}