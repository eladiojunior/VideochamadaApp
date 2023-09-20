using Microsoft.AspNetCore.SignalR;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace videochamada.frontend.Helper;

public class VideoChamadaHub : Hub
{
    private IServiceAtendimento _serviceAtendimento = null;

    public VideoChamadaHub(IServiceAtendimento serviceAtendimento)
    {
        this._serviceAtendimento = serviceAtendimento;
    }

    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var connectionId = Context.ConnectionId;
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task SendOffer(string offer, string connectionId)
    {
        Console.WriteLine("SendOffer: " + connectionId);
        await Clients.Others.SendAsync("ReceiveOffer", offer);
    }

    public async Task SendAnswer(string answer, string connectionId)
    {
        Console.WriteLine("SendAnswer: " + connectionId);
        //await Clients.Client(connectionId).SendAsync("ReceiveAnswer", answer);
        await Clients.Others.SendAsync("ReceiveAnswer", answer);
    }

    public async Task SendIceCandidate(string iceCandidate, string connectionId)
    {
        Console.WriteLine("SendIceCandidate: " + connectionId);
        await Clients.Others.SendAsync("ReceiveIceCandidate", iceCandidate);
    }
    
}