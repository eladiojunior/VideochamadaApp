using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Enums;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServiceAtendimento
{
    AtendimentoModel CriarAtendimento(NovoAtendimentoModel model);
    AtendimentoModel ObterAtendimentoAberto(string idCliente);
    AtendimentoModel EntrarFilaAtendimento(AtendimentoModel atendimento);
    int PosicaoFilaAtendimento(string idCliente);
    void EncerrarAtendimento(string idAtendimento, SituacaoAtendimentoEnum situacaoAtendimento);
    void AvaliarAtendimento(AvaliacaoAtendimentoModel avaliarAtendimento);
    int QtdClienteFilaAtendimento();
    int QtdClienteEmAtendimento();
    List<AtendimentoModel> ListarAtendimentosCliente(string idCliente);
    AtendimentoModel ObterAtendimento(string idAtendimento);
    void VerificarDispositivoParaAtendimento(string idAtendimento);
    AtendimentoModel IniciarAtendimentoProfissionalSaude(string idCliente, string idProfissional);
    List<AtendimentoModel> ListarAtendimentosProfissional(string idProfissional, bool hasRealizados);
    int QtdAtendimentos(SituacaoAtendimentoEnum filtroSituacao);
    TimeSpan TempoMedioAtendimentos(DateTime filtroDatahora);
    TimeSpan TempoMedioNaFilaAtendimento(DateTime filtroDatahora);
    int NotaMediaAtendimentos(DateTime? filtroDatahora);
    ArquivoAtendimentoModel ObterArquivoAtendimento(string idAtendimento, string idArquivo);
    void RemoverArquivoAtendimento(string idAtendimento, string idArquivo);
    List<ArquivoAtendimentoModel> ListarArquivosAtendimento(string idAtendimento);
    ChatAtendimentoModel ObterChatAtendimento(string idAtendimento);
    ArquivoAtendimentoModel RegistrarArquivoAtendimentoModel(string idAtendimento, ArquivoAtendimentoModel arquivo);
    ClienteModel ObterProximoClienteAtendimento();
    Task<MensagemChatModel> RegistrarMensagemChatAtendimento(string idAtendimento, string idUsuario, string mensagem);
    void FinalizarAtendimento(string idAtendimento);
    AtendimentoModel ObterAtendimentoAbertoPorProfissional(string idProfissional);
    void GravarTextoMotivoAtendimento(string idAtendimento, string textoMotivoAtendimento);
}