using videochamada.frontend.Models;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServiceAtendimento
{
    AtendimentoModel CriarAtendimento(string idCliente);
    AtendimentoModel ObterAtendimentoAberto(string idCliente);
    
    AtendimentoModel EntrarFilaAtendimento(AtendimentoModel atendimento);
    int PosicaoFilaAtendimento(string idCliente);
    void EncerrarAtendimento(AvaliacaoAtendimentoModel atendimento);
    AtendimentoModel IniciarAtendimentoProfissionalSaude(AtendimentoModel atendimento, ProfissionalSaudeModel profissional);
    int QtdClienteFilaAtendimento();
    List<AtendimentoModel> ListarAtendimentosCliente(string idCliente);
}