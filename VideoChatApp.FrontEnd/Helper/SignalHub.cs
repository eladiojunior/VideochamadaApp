using Microsoft.AspNetCore.SignalR;

namespace VideoChatApp.FrontEnd.Helper;

public class SignalHub : Hub
{
    public async Task RegisterPeer(string peerId)
    {
        // Você pode usar grupos para gerenciar salas ou conexões individuais.
        // Neste exemplo simples, estamos apenas registrando um usuário em um grupo com seu ID de peer.
        Console.WriteLine("ID: " + peerId);
        await Groups.AddToGroupAsync(Context.ConnectionId, peerId);
        
    }

    public async Task SendOffer(string targetPeerId, string offer)
    {
        // Envia a oferta SDP para o peer de destino.
        await Clients.Group(targetPeerId).SendAsync("ReceiveOffer", Context.ConnectionId, offer);
    }

    public async Task SendAnswer(string targetPeerId, string answer)
    {
        // Envia a resposta SDP para o peer que enviou a oferta.
        await Clients.Group(targetPeerId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
    }

    public async Task SendIceCandidate(string targetPeerId, string iceCandidate)
    {
        // Envia os ICE candidates para o peer de destino para ajudar na negociação da conexão.
        await Clients.Group(targetPeerId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, iceCandidate);
    }
    
}