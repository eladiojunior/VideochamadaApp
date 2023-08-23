using System.Collections.Concurrent;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Interfaces;
namespace VideoChatApp.FrontEnd.Services;

public class ServiceCliente : IServiceCliente
{
    private static readonly ConcurrentDictionary<string, ClienteModel> _clientes = new ConcurrentDictionary<string, ClienteModel>();
    
    public ClienteModel RegistrarCliente(ClienteRegistroModel cliente)
    {
        var clienteNovo = new ClienteModel();
        clienteNovo.Id = ServiceHelper.GerarId();
        clienteNovo.Nome = cliente.Nome;
        clienteNovo.Email = cliente.Email;
        clienteNovo.Telefone = cliente.Telefone;
        _clientes.TryAdd(clienteNovo.Id, clienteNovo);
        return clienteNovo;
    }

    public ClienteModel ObterCliente(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;
        return _clientes.GetValueOrDefault(id);
    }
    
    public List<ClienteModel> ListarCliente()
    {
        return _clientes.Values.ToList();
    }

}