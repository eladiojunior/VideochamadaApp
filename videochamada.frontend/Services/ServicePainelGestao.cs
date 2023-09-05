using System.Collections.Concurrent;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Exceptions;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Services;

public class ServicePainelGestao : IServicePainelGestao
{
    private readonly IListenerServerClient _listenerServerClient;
    private static readonly ConcurrentDictionary<string, UsuarioGestorModel> _usuariosGestor = new ConcurrentDictionary<string, UsuarioGestorModel>();
    private static readonly ConcurrentDictionary<string, UsuarioLogadoModel> _usuariosGestorLogados = new ConcurrentDictionary<string, UsuarioLogadoModel>();

    public ServicePainelGestao(IListenerServerClient listenerServerClient)
    {
        _listenerServerClient = listenerServerClient;
       
        //Criar um usuário padrão para acesso ao painel...
        var idUsuarioGestor = ServiceHelper.GerarId();
        _usuariosGestor.TryAdd(idUsuarioGestor, new UsuarioGestorModel()
        {
            Id = idUsuarioGestor,
            Nome = "Eladio Júnior - Administrador",
            Email = "eladiojunior@gmail.com",
            Senha = "Admin123456"
        });
        
    }
    
    public void LogoffUsuarioGestor(string idUsuarioGestor)
    {
        var usuarioGestor = ObterUsuarioGestor(idUsuarioGestor);
        if (usuarioGestor == null)
            return;
        //Remover usuário dos logados...
        _usuariosGestorLogados.Remove(idUsuarioGestor, out var usuario);
    }

    private void RegistrarUsuarioLogado(string idUsuarioGestor)
    {
        var usuario = new UsuarioLogadoModel();
        usuario.IdUsuario = idUsuarioGestor;
        usuario.DataHoraLogin = DateTime.Now;
        usuario.DataHoraUltimaVerificacao = DateTime.Now;
        usuario.IpMaquinaUsuario = _listenerServerClient.IpMaquinaUsuario();
        _usuariosGestorLogados.TryAdd(idUsuarioGestor, usuario);
    }
    
    public UsuarioGestorModel AutenticarUsuarioGestor(string email, string senha)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            return null;

        const string mensagemSemAcesso = "Não foi possível realizar o acesso, verifique os dados informados.";
        
        var usuario = ObterUsuarioGestorPorEmail(email);
        if (usuario == null)
            throw new ServiceException(mensagemSemAcesso);
        if (!usuario.Senha.Equals(senha))
            throw new ServiceException(mensagemSemAcesso);

        //Registrar login usuario...
        RegistrarUsuarioLogado(usuario.Id);
        
        return usuario;
    }

    public UsuarioGestorModel ObterUsuarioGestor(string idUsuario)
    {
        if (string.IsNullOrEmpty(idUsuario))
            return null;
        return _usuariosGestor.GetValueOrDefault(idUsuario);
    }

    public UsuarioGestorModel ObterUsuarioGestorPorEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return null;
        return _usuariosGestor.Values.FirstOrDefault(w => w.Email.Equals(email));
    }
}