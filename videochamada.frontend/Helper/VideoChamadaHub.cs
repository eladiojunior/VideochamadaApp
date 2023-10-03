using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Enums;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace videochamada.frontend.Helper;

public class VideoChamadaHub : Hub
{
    private static readonly ConcurrentDictionary<string, UsuarioHubModel> _usuarios = new ConcurrentDictionary<string, UsuarioHubModel>();
    private IServiceAtendimento _serviceAtendimento;

    public VideoChamadaHub(IServiceAtendimento serviceAtendimento)
    {
        _serviceAtendimento = serviceAtendimento;
    }
    
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        await Clients.Client(connectionId).SendAsync("UsuarioConectado", connectionId);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var connectionId = Context.ConnectionId;
        var usuarioHub = ObterUsuarioHub(connectionId);
        if (usuarioHub != null)
        {
            RemoverUsuarioHub(connectionId);
            await Groups.RemoveFromGroupAsync(connectionId, usuarioHub.IdAtendimento);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task ConectarAtendimento(string idAtendimento, string idUsuario)
    {
        var usuario = ObterUsuario(idAtendimento);
        if (usuario != null)
        {
            if (!usuario.HasExisteIdUsuario(idUsuario))
            {
                usuario.AddUsuario(Context.ConnectionId, idUsuario);
                await Groups.AddToGroupAsync(Context.ConnectionId, idAtendimento);
            }
        }
        else
        {
            var usuarioHub = new UsuarioHubModel();
            usuarioHub.IdAtendimento = idAtendimento;
            usuarioHub.AddUsuario(Context.ConnectionId, idUsuario);
            _usuarios.TryAdd(idAtendimento, usuarioHub);
            await Groups.AddToGroupAsync(Context.ConnectionId, idAtendimento); 
        }
    }

    public async Task DesconectarAtendimento(string idAtendimento, string idUsuario)
    {
        var usuarioHub = ObterUsuario(idAtendimento);
        if (usuarioHub != null)
        {
            _serviceAtendimento.EncerrarAtendimento(idAtendimento, SituacaoAtendimentoEnum.Finalizado);
            var idOutroUsuario = usuarioHub.ObterUsuarioDiferenteDe(Context.ConnectionId);
            await Clients.OthersInGroup(usuarioHub.IdAtendimento).SendAsync("UsuarioDesconectado", idOutroUsuario);
        }
    }
    public async Task EnviarMensagem(string mensagem)
    {
        var connectionId = Context.ConnectionId;
        var usuarioHub = ObterUsuarioHub(connectionId);
        if (usuarioHub != null)
        {
            var idUsuario = usuarioHub.ObterUsuario(connectionId);
            var mensagemChat = _serviceAtendimento.RegistrarMensagemChatAtendimento(usuarioHub.IdAtendimento, idUsuario, mensagem).Result;
            if (mensagemChat == null)
                return;
            var codOrigemEnvio = mensagemChat.UsuarioUsuarioOrigem.ObterCodigoEnum().ToString();
            await Clients.Group(usuarioHub.IdAtendimento).SendAsync("ReceberMensagem", idUsuario, codOrigemEnvio, mensagem);
        }
    }
    
    public async Task EnviarArquivo(string nomeArquivo)
    {
        var connectionId = Context.ConnectionId;
        var usuarioHub = ObterUsuarioHub(connectionId);
        if (usuarioHub != null)
        {
            var idUsuario = usuarioHub.ObterUsuario(connectionId);
            await Clients.OthersInGroup(usuarioHub.IdAtendimento).SendAsync("ReceberArquivo", idUsuario, "incluido", nomeArquivo);
        }
    }
    
    public async Task RemoverArquivo(string idArquivo)
    {
        var connectionId = Context.ConnectionId;
        var usuarioHub = ObterUsuarioHub(connectionId);
        if (usuarioHub != null)
        {
            var idUsuario = usuarioHub.ObterUsuario(connectionId);
            await Clients.OthersInGroup(usuarioHub.IdAtendimento).SendAsync("ReceberArquivo", idUsuario, "removido", idArquivo);
        }
    }
    
    private UsuarioHubModel ObterUsuarioHub(string connectionId)
    {
        return _usuarios.Values.FirstOrDefault(w => w.HasExisteIdUsuarioHub(connectionId));
    }
    private UsuarioHubModel ObterUsuario(string idAtendimento)
    {
        return _usuarios.GetValueOrDefault(idAtendimento)!;
    }
    private void RemoverUsuarioHub(string connectionId)
    {
        var usuarioHub = ObterUsuarioHub(connectionId);
        if (usuarioHub == null) return; 
        //Remover do grupo de atendimento...
        usuarioHub.RemoverUsuarioHub(connectionId);
        if (usuarioHub.QtdUsuarios() == 0)
        {//Remover da lista de Hubs.
            _usuarios.Remove(usuarioHub.IdAtendimento, out var item);
        }
    }
    
    //Métodos para controle do VideRemoto    
    public async Task ControleAudioVideoAtendimento(string equipamento, bool hasControle)
    {
        var connectionId = Context.ConnectionId;
        var usuarioHub = ObterUsuarioHub(connectionId);
        if (usuarioHub != null)
            await Clients.OthersInGroup(usuarioHub.IdAtendimento).SendAsync("ReceiveAudioVideoAtendimento", equipamento, hasControle);
    }
    
    public async Task SendOffer(string offer, string connectionId)
    {
        await Clients.OthersInGroup(connectionId).SendAsync("ReceiveOffer", offer);
        //await Clients.Others.SendAsync("ReceiveOffer", offer);
    }
    public async Task SendAnswer(string answer, string connectionId)
    {
        await Clients.OthersInGroup(connectionId).SendAsync("ReceiveAnswer", answer);
        //await Clients.Others.SendAsync("ReceiveAnswer", answer);
    }
    public async Task SendIceCandidate(string iceCandidate, string connectionId)
    {
        await Clients.OthersInGroup(connectionId).SendAsync("ReceiveIceCandidate", iceCandidate);
        //await Clients.Others.SendAsync("ReceiveIceCandidate", iceCandidate);
    }
    
}