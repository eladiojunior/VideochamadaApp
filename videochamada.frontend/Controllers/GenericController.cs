using Microsoft.AspNetCore.Mvc;

namespace VideoChatApp.FrontEnd.Controllers;

public class GenericController : Controller
{
    internal string ObterIdUsuario()
    {
        var idUsuario = HttpContext.Session.GetString("ID_USUARIO");
        if (!string.IsNullOrEmpty(idUsuario))
            return idUsuario;
        
        var guidId = Guid.NewGuid();
        idUsuario = guidId.ToString();
        HttpContext.Session.SetString("ID_USUARIO", idUsuario);
        return idUsuario;
    }
    
}