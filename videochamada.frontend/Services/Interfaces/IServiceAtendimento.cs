using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Enums;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServiceAtendimento
{
    AtendimentoModel CriarAtendimento(NovoAtendimentoModel model);
    AtendimentoModel ObterAtendimentoAberto(string idCliente);
    AtendimentoModel EntrarFilaAtendimento(AtendimentoModel atendimento);
    int PosicaoFilaAtendimento(string idCliente);
    void EncerrarAtendimento(AvaliacaoAtendimentoModel atendimento, SituacaoAtendimentoEnum situacaoAtendimento);
    int QtdClienteFilaAtendimento();
    List<AtendimentoModel> ListarAtendimentosCliente(string idCliente);
    AtendimentoModel ObterAtendimento(string idAtendimento);
    void VerificarDispositivoParaAtendimento(string idAtendimento);
    
    AtendimentoModel IniciarAtendimentoProfissionalSaude(AtendimentoModel atendimento, ProfissionalSaudeModel profissional);
}