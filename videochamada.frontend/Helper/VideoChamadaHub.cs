using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using videochamada.frontend.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace videochamada.frontend.Helper;

public class VideoChamadaHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        Console.WriteLine("OnConnectedAsync ID: " + connectionId);
        _users[connectionId] = connectionId;
        await Clients.Others.SendAsync("UserJoined", connectionId);
        await base.OnConnectedAsync();
    }

   /* 
    public async Task NewUser(string username)
    {
        var connectionId = Context.ConnectionId;
        Console.WriteLine($"NewUser: {username} Connection ID: {connectionId}.");
        var userInfo = new UserInfo() { UserName = username, ConnectionId = connectionId };
        _users[connectionId] = userInfo;
        await Clients.Others.SendAsync("UserJoined", JsonSerializer.Serialize(userInfo));
    }
    */
   
    public async Task SendSignal(string signal, string targetConnectionId)
    {
        Console.WriteLine("signal: " + signal);
        Console.WriteLine("targetConnectionId: " + targetConnectionId);
        await Clients.Client(targetConnectionId).SendAsync("ReceiveSignal", signal, Context.ConnectionId);
    }    
    
    
    /*
    */
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var connectionId = Context.ConnectionId;
        Console.WriteLine("Desconectar ID: " + connectionId);
        _users.TryRemove(connectionId, out _);
        await Clients.All.SendAsync("UserDisconnected", connectionId);
        await base.OnDisconnectedAsync(exception);
    }
    
}