using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace VideoChatApp.FrontEnd.Controllers;

public class GenericController : Controller
{
    private List<string> _erros;
    private const string KeyIdClienteSession = "KEY_ID_CLIENTE_SESSION";
    private const string KeyIdProfissionalSaudeSession = "KEY_ID_PROFISSIONAL_SAUDE_SESSION";
    private const string KeyIdUsuarioGestorSession = "KEY_ID_USUARIO_GESTOR_SESSION";
    
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

    /// <summary>
    ///     Cria um retorno Json de Erro (Result = false) sem Model e mensagem com o erro para o usuário.
    /// </summary>
    /// <param name="mensagemAlerta">Mensagem de erro que deve ser apresentada ao usuário.</param>
    /// <returns></returns>
    internal JsonResult JsonResultErro(string mensagemErro)
    {
        return Json(new { HasErro = true, Mensagem = mensagemErro });
    }
    
    /// <summary>
    ///     Renderizar a View em String para respostas Json;
    /// </summary>
    /// <param name="viewName">Nome da View a ser renderizada.</param>
    /// <param name="model">Informações da Model para carga.</param>
    internal object RenderRazorViewToString(string viewName, object model)
    {
        var actionContext = ControllerContext as ActionContext;
        var serviceProvider = ControllerContext.HttpContext.RequestServices;
        var razorViewEngine = serviceProvider.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
        var tempDataProvider = serviceProvider.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

        using var sw = new StringWriter();
        var viewResult = razorViewEngine.FindView(actionContext, viewName, false);
        if (viewResult?.View == null)
            throw new ArgumentException($"{viewName} does not match any available view");
        var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model };
        var viewContext = new ViewContext(actionContext, viewResult.View,  viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            sw, new HtmlHelperOptions()
        );
        viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
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
    internal void GravarIdUsuarioGestor(string idUsuarioGestor)
    {
        HttpContext.Session.SetString(KeyIdUsuarioGestorSession, idUsuarioGestor);
    }
    internal bool HasUsuarioGestorLogado()
    {
        var idUsuarioGestor = ObterIdUsuarioGestor();
        return !string.IsNullOrEmpty(idUsuarioGestor);
    }
    internal void LogoffUsuarioGestorSession()
    {
        HttpContext.Session.Remove(KeyIdUsuarioGestorSession);
    }

    internal string? ObterIdUsuarioGestor()
    {
        var idUsuarioGestor = HttpContext.Session.GetString(KeyIdProfissionalSaudeSession);
        return string.IsNullOrEmpty(idUsuarioGestor) ? null : idUsuarioGestor;
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
    internal void LogoffProfissionalSaudeSession()
    {
        HttpContext.Session.Remove(KeyIdProfissionalSaudeSession);
    }

}