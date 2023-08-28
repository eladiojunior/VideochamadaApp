namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IListenerServerClient
{
    string IpMaquinaUsuario();
    bool IsDispositivoMobileUsuario();
    string InfoDispositivoUsuario();
}