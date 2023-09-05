using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Services;

public class ServicePainelGestao : IServicePainelGestao
{
    public UsuarioGestorModel AutenticarUsuarioGestor(string email, string senha)
    {
        throw new NotImplementedException();
    }

    public UsuarioGestorModel ObterUsuarioGestor(string idUsuario)
    {
        throw new NotImplementedException();
    }

    public UsuarioGestorModel ObterUsuarioGestorPorEmail(string email)
    {
        throw new NotImplementedException();
    }
}