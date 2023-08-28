using VideoChatApp.FrontEnd.Services.Interfaces;

namespace videochamada.frontend.Helper;

public class ListenerServerClient : IListenerServerClient
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ListenerServerClient(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string IpMaquinaUsuario()
    {
        var ipMaquina = string.Empty;
        if (_httpContextAccessor == null)
            throw new Exception("IHttpContextAccessor está nulo.");
        try
        {
            ipMaquina = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var enderecosLocalhost = new[] { "::1", "127.0.0.1", "localhost" };
            if (string.IsNullOrEmpty(ipMaquina) || enderecosLocalhost.Contains(ipMaquina.ToLowerInvariant()))
                ipMaquina = "LocalHost";

            if (string.IsNullOrEmpty(ipMaquina) || enderecosLocalhost.Contains(ipMaquina.ToLowerInvariant()))
                ipMaquina = $"{Environment.MachineName}\\{Environment.UserName}";
        }
        catch
        {
            // ignored
        }
        return ipMaquina;
    }

    public bool IsDispositivoMobileUsuario()
    {
        var isMobile = false;
        try
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            if (!string.IsNullOrEmpty(userAgent))
            {
                isMobile = userAgent.Contains("Mobile") || userAgent.Contains("Android") ||
                           userAgent.Contains("iPhone");
            }
        }
        //Erro ao recuperar as informações de dispositivo Mobile cliente.
        catch
        {
            // ignored
        }
        return isMobile;
    }

    public string InfoDispositivoUsuario()
    {
        var infoDispositivo = string.Empty;
        try
        {
            infoDispositivo = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? infoDispositivo;
        }
        //Erro ao recuperar as informações do dispositivo cliente.
        catch
        {
            // ignored
        }
        return infoDispositivo;
    }
}