using videochamada.frontend.Models;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServiceEquipeSaude
{
    ProfissionalSaudeModel RegistrarProfissionalSaude(ProfissionalSaudeRegistroModel profissionalSaude);
    ProfissionalSaudeModel ObterProfissionalSaude(string id);
    List<ProfissionalSaudeModel> ListarProfissionalSaude();
    List<ProfissionalSaudeModel> ListarProfissionalSaudeOnline();
    int QtdProfissionalSaudeOnline();
    ProfissionalSaudeModel AutenticarProfissionalSaude(string email, string senha);
}