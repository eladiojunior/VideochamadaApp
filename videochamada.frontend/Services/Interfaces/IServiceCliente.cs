using videochamada.frontend.Models;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServiceCliente
{
    ClienteModel RegistrarCliente(ClienteRegistroModel cliente);
    ClienteModel ObterCliente(string id);
    List<ClienteModel> Listar();
    ClienteAtendimentoModel NovoAtendimento(ClienteModel cliente);
    ClienteAtendimentoModel ObterAtendimentoAberto(string idCliente);
    void EncerrarAtendimento(AvaliacaoAtendimentoModel atendimento);
}