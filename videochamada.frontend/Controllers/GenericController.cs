using Microsoft.AspNetCore.Mvc;

namespace VideoChatApp.FrontEnd.Controllers;

public class GenericController : Controller
{
    private const string KeyIdClienteSession = "KEY_ID_CLIENTE_SESSION";
    internal const string KeyMensagemErros = "KEY_MENSAGEM_ERRO";
    private List<string> _erros;
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

    internal void ExibirErro(string mensagem)
    {
        if (_erros == null) 
            _erros = new List<string>();
        _erros.Add(mensagem);
        TempData[KeyMensagemErros] = _erros;
    }
    internal bool HasErros()
    {
        return (_erros!=null && _erros.Count!=0);
    }
    internal void ExibirAlerta(string mensagem)
    {
        if (!string.IsNullOrEmpty(mensagem))
            TempData[KeyMensagemAlerta] = mensagem;
    }

}