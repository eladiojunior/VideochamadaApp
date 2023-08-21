using videochamada.frontend.Models;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServiceCliente
{
    ClienteModel Registrar(ClienteRegistroModel cliente);
    ClienteModel Obter(string id);
    List<ClienteModel> Listar();
}