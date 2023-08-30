using Microsoft.AspNetCore.Mvc;

namespace VideoChatApp.FrontEnd.Controllers;

public class GenericController : Controller
{
    private List<string> _erros;
    private const string KeyIdClienteSession = "KEY_ID_CLIENTE_SESSION";
    private const string KeyIdProfissionalSaudeSession = "KEY_ID_PROFISSIONAL_SAUDE_SESSION";
    
    internal const string KeyMensagemErros = "KEY_MENSAGEM_ERRO";
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

    internal string? ObterIdCliente()
    {
        var idCliente = HttpContext.Session.GetString(KeyIdClienteSession);
        return string.IsNullOrEmpty(idCliente) ? null : idCliente;
    }
    
    internal void GravarIdCliente(string idCliente)
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

    internal string? ObterIdProfissionalSaude()
    {
        var idProfissionalSaude = HttpContext.Session.GetString(KeyIdProfissionalSaudeSession);
        return string.IsNullOrEmpty(idProfissionalSaude) ? null : idProfissionalSaude;
    }
    
    internal void GravarIdProfissionalSaude(string idProfissionalSaude)
    {
        HttpContext.Session.SetString(KeyIdProfissionalSaudeSession, idProfissionalSaude);
    }

    internal bool HasProfissionalSaudeLogado()
    {
        var idProfissional = ObterIdProfissionalSaude();
        return !string.IsNullOrEmpty(idProfissional);
    }

    internal void LogoffProfissionalSaude()
    {
        HttpContext.Session.Remove(KeyIdProfissionalSaudeSession);
    }

}