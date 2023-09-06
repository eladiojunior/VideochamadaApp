using System.Collections.Concurrent;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Enums;
using VideoChatApp.FrontEnd.Services.Exceptions;
using VideoChatApp.FrontEnd.Services.Interfaces;
namespace VideoChatApp.FrontEnd.Services;

public class ServiceCliente : IServiceCliente
{
    private static readonly ConcurrentDictionary<string, ClienteModel> _clientes = new ConcurrentDictionary<string, ClienteModel>();
    
    public ClienteModel RegistrarCliente(ClienteRegistroModel cliente)
    {
        
        if (cliente == null)
            throw new ServiceException("Informações do Cliente não informado.");
        if (string.IsNullOrEmpty(cliente.Nome))
            throw new ServiceException("Nome do Cliente não informado, obrigatório.");
        if (string.IsNullOrEmpty(cliente.DataNascimento))
            throw new ServiceException("Data de nascimento do Cliente não informado, obrigatório.");
        if (!ServiceHelper.VerificarDataValida(cliente.DataNascimento))
            throw new ServiceException("Data de nascimento do Cliente inválida, formato: DD/MM/YYYY.");
        if (string.IsNullOrEmpty(cliente.Sexo))
            throw new ServiceException("Sexo do Cliente não informado, obrigatório.");
        if (!ServiceHelper.VerificarSexoValido(cliente.Sexo))
            throw new ServiceException("Sexo do Cliente inválido, informe (Masculino ou Feminino).");
        if (string.IsNullOrEmpty(cliente.Email))
            throw new ServiceException("E-mail do Cliente não informado, obrigatório.");
        if (string.IsNullOrEmpty(cliente.Telefone))
            throw new ServiceException("Telefone do Cliente não informado, obrigatório.");

        var clienteExiste = ObterClientePorEmail(cliente.Email);
        if (clienteExiste != null)
            throw new ServiceException("Cliente já registrado com o e-mail informado.");
        
        var clienteNovo = new ClienteModel();
        clienteNovo.Id = ServiceHelper.GerarId();
        clienteNovo.Nome = cliente.Nome;
        var dtNascimento = ServiceHelper.ConverterStringToDateTime(cliente.DataNascimento);
        if (dtNascimento.HasValue)
            clienteNovo.DataNascimento = dtNascimento.Value;
        clienteNovo.Sexo = ServiceHelper.ObterSexoEnum(cliente.Sexo);
        clienteNovo.Email = cliente.Email.ToLower();
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

    public ClienteModel ObterClientePorEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return null;
        var clienteModel = _clientes.Values.FirstOrDefault(w => w.Email.Equals(email));
        return clienteModel;
    }

    public int QtdClientesRegistrados()
    {
        return _clientes.Values.Count();
    }
}