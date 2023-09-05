using videochamada.frontend.Models;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServiceEquipeSaude
{
    ProfissionalSaudeModel RegistrarProfissionalSaude(ProfissionalSaudeRegistroModel profissionalSaude);
    ProfissionalSaudeModel ObterProfissionalSaude(string id);
    List<ProfissionalSaudeModel> ListarProfissionalSaude();
    List<ProfissionalSaudeModel> ListarProfissionalSaudeOnline();
    int QtdProfissionalSaudeOnline();
    void AtualizarSituacaoProfissionalAendimento(string idProfissional, bool hasSituacaoAtendimento);
    int QtdProfissionalSaudeLogados();
    int QtdProfissionalSaudeEmAtendimento();
    
    //Acesso de profissional de Saúde
    ProfissionalSaudeModel AutenticarProfissionalSaude(string email, string senha);
    void LogoffProfissionalSaude(string idProfissional);
}