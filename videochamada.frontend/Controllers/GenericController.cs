using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using videochamada.frontend.Models;

namespace VideoChatApp.FrontEnd.Controllers;

public class GenericController : Controller
{
    private const string KeyIdClienteSession = "KEY_ID_CLIENTE_SESSION";
    internal const string KeyMensagemAlerta = "KEY_MENSAGEM_ALERTA";
    
    /// <summary>
    ///     Cria um retorno Json de Sucesso (Result = true) com mensagem para o usuário (opcional).
    /// </summary>
    /// <param name="mensagemAlerta">Mensagem de alerta que deve ser apresentada ao usuário.</param>
    /// <returns></returns>
    internal JsonResult JsonResultSucesso(string mensagemAlerta = "")
    {
        return Json(new { HasErro = false, Mensagem = mensagemAlerta });
    }

    /// <summary>
    ///     Cria um retorno Json de Sucesso (Result = true) com Model e mensagem para o usuário (opcional).
    /// </summary>
    /// <param name="model">Informações do Model para renderizar a view.</param>
    /// <param name="mensagemAlerta">Mensagem de alerta que deve ser apresentada ao usuário.</param>
    /// <returns></returns>
    internal JsonResult JsonResultSucesso(object model, string mensagemAlerta = "")
    {
        return Json(new { HasErro = false, Model = model, Mensagem = mensagemAlerta });
    }

    internal string? ObterIdClienteSession()
    {
        var idCliente = HttpContext.Session.GetString(KeyIdClienteSession);
        return string.IsNullOrEmpty(idCliente) ? null : idCliente;
    }
    
    internal void GravarIdClienteSession(string idCliente)
    {
        HttpContext.Session.SetString(KeyIdClienteSession, idCliente);
    }

    private bool _hasAlerta = false;
    internal void ExibirAlerta(string mensagem)
    {
        TempData[KeyMensagemAlerta] = mensagem;
        _hasAlerta = true;
    }

    internal bool HasAlerta()
    {
        return _hasAlerta;
    }
}