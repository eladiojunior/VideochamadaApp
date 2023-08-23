using videochamada.frontend.Models;

namespace VideoChatApp.FrontEnd.Services.Interfaces;

public interface IServiceEquipeSaude
{
    ProfissionalSaudeModel RegistrarEquipeSaude(ProfissionalSaudeRegistroModel profissionalSaude);
    ProfissionalSaudeModel ObterProfissionalSaude(string id);
    List<ProfissionalSaudeModel> ListarProfissionalSaude();
    List<ProfissionalSaudeModel> ListarProfissionalSaudeOnline();
    int QtdProfissionalSaudeOnline();
}