using System.Collections.Concurrent;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Interfaces;
using System.Linq;
namespace VideoChatApp.FrontEnd.Services;

public class ServiceCliente : IServiceCliente
{
    private static readonly ConcurrentDictionary<string, ClienteModel> _clientes = new ConcurrentDictionary<string, ClienteModel>();
    
    public ClienteModel RegistrarCliente(ClienteRegistroModel cliente)
    {
        var clienteNovo = new ClienteModel();
        clienteNovo.Id = cliente.Id;
        clienteNovo.Nome = cliente.Nome;
        clienteNovo.Email = cliente.Email;
        clienteNovo.Telefone = cliente.Telefone;
        _clientes.TryAdd(clienteNovo.Id, clienteNovo);
        return clienteNovo;
    }

    public ClienteModel ObterCliente(string id)
    {
        return _clientes.GetValueOrDefault(id);
    }
    
    public List<ClienteModel> Listar()
    {
        return _clientes.Values.ToList();
    }

    public ClienteAtendimentoModel NovoAtendimento(ClienteModel cliente)
    {
        var clienteModel = ObterCliente(cliente.Id);
        if (clienteModel == null)
            return null;
        
        var atendimentoCliente = new ClienteAtendimentoModel();
        atendimentoCliente.IdCliente = cliente.Id;
        atendimentoCliente.Cliente = clienteModel;
        var idAtendimento = Guid.NewGuid().ToString(); //Gerar ID
        atendimentoCliente.IdAtendimento = idAtendimento;
        var atendimento = new AtendimentoModel();
        atendimento.Id = idAtendimento;
        atendimento.IdCliente = clienteModel.Id;
        atendimento.Situacao = "EmAndamento";
        atendimento.DataInicial = DateTime.Now;
        atendimento.DataFinal = null;
        clienteModel.Atendimentos ??= new List<AtendimentoModel>();
        clienteModel.Atendimentos.Add(atendimento);
        atendimentoCliente.Atendimento = atendimento;
        
        return atendimentoCliente;
    }

    public ClienteAtendimentoModel ObterAtendimentoAberto(string idCliente)
    {
        var clienteModel = ObterCliente(idCliente);
        if (clienteModel == null)
            return null;
        var atendimentoAberto = clienteModel.Atendimentos.FirstOrDefault(c => c.DataFinal == null);
        if (atendimentoAberto == null)
            return null;
        
        var atendimentoCliente = new ClienteAtendimentoModel();
        atendimentoCliente.IdCliente = clienteModel.Id;
        atendimentoCliente.Cliente = clienteModel;
        var idAtendimento = atendimentoAberto.Id;
        atendimentoCliente.IdAtendimento = idAtendimento;
        atendimentoCliente.Atendimento = atendimentoAberto;
        return atendimentoCliente;
            
    }

    public void EncerrarAtendimento(AvaliacaoAtendimentoModel atendimento)
    {
        
        var clienteModel = ObterCliente(atendimento.IdCliente);
        if (clienteModel == null)
            return;

        var atendimentoModel = clienteModel.Atendimentos.FirstOrDefault(w => w.Id == atendimento.IdAtendimento);
        if (atendimentoModel == null)
            return;
        
        atendimentoModel.DataFinal = DateTime.Now;
        atendimentoModel.Situacao = atendimento.HasDesistencia ? "Desistência" : "Finalizado";
        atendimentoModel.Nota = atendimento.Nota;
        atendimentoModel.Comentario = atendimento.Comentario;
        
    }
    
}