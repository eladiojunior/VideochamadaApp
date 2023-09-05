using videochamada.frontend.Models;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServiceCliente
{
    ClienteModel RegistrarCliente(ClienteRegistroModel cliente);
    ClienteModel ObterCliente(string id);
    List<ClienteModel> ListarCliente();
    ClienteModel ObterClientePorEmail(string email);
    int QtdClientesRegistrados();
}