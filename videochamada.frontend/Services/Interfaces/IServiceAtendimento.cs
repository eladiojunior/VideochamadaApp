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
    int QtdClienteEmAtendimento();
    List<AtendimentoModel> ListarAtendimentosCliente(string idCliente);
    AtendimentoModel ObterAtendimento(string idAtendimento);
    void VerificarDispositivoParaAtendimento(string idAtendimento);
    AtendimentoModel IniciarAtendimentoProfissionalSaude(string idAtendimento, string idProfissional);
    List<AtendimentoModel> ListarAtendimentosProfissional(string idProfissional, bool hasRealizados);
    int QtdAtendimentos(SituacaoAtendimentoEnum filtroSituacao);
    TimeSpan TempoMedioAtendimentos(DateTime filtroDatahora);
    TimeSpan TempoMedioNaFilaAtendimento(DateTime filtroDatahora);
    int NotaMediaAtendimentos(DateTime? filtroDatahora);
    ArquivoClienteAtendimentoModel ObterArquivoAtendimento(string idAtendimento, string idArquivo);
    void RemoverArquivoAtendimento(string idAtendimento, string idArquivo);
    List<ArquivoClienteAtendimentoModel> ListarArquivosAtendimento(string idAtendimento);
    ChatAtendimentoModel ObterChatAtendimento(string idAtendimento);
    ArquivoClienteAtendimentoModel RegistrarArquivoAtendimentoModel(string idAtendimento, ArquivoClienteAtendimentoModel arquivo);
}