using System.Collections.Concurrent;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Services;

public class ServiceCliente : IServiceCliente
{
    private static readonly ConcurrentDictionary<string, ClienteModel> _clientes = new ConcurrentDictionary<string, ClienteModel>();
    
    public ClienteModel Registrar(ClienteRegistroModel cliente)
    {
        var clienteNovo = new ClienteModel();
        clienteNovo.Id = cliente.Id;
        clienteNovo.Nome = cliente.Nome;
        clienteNovo.Email = cliente.Email;
        clienteNovo.Telefone = cliente.Telefone;
        _clientes.TryAdd(clienteNovo.Id, clienteNovo);
        return clienteNovo;
    }

    public ClienteModel Obter(string id)
    {
        return _clientes.GetValueOrDefault(id);
    }
    
    public List<ClienteModel> Listar()
    {
        return _clientes.Values.ToList();
    }
    
}