using System.Collections.Concurrent;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Services;

public class ServiceEquipeSaude : IServiceEquipeSaude
{
    private static readonly ConcurrentDictionary<string, ProfissionalSaudeModel> _profissionais = new ConcurrentDictionary<string, ProfissionalSaudeModel>();
    
    public ProfissionalSaudeModel RegistrarEquipeSaude(ProfissionalSaudeRegistroModel profissionalSaude)
    {
        var profissionalSaudeNovo = new ProfissionalSaudeModel();
        profissionalSaudeNovo.Id = ServiceHelper.GerarId();
        profissionalSaudeNovo.Nome = profissionalSaude.Nome;
        profissionalSaudeNovo.Email = profissionalSaude.Email;
        profissionalSaudeNovo.Telefone = profissionalSaude.Telefone;
        profissionalSaudeNovo.Especialidade = profissionalSaude.Especialidade;
        _profissionais.TryAdd(profissionalSaudeNovo.Id, profissionalSaudeNovo);
        return profissionalSaudeNovo;
    }

    public ProfissionalSaudeModel ObterProfissionalSaude(string id)
    {
        throw new NotImplementedException();
    }

    public List<ProfissionalSaudeModel> ListarProfissionalSaude()
    {
        throw new NotImplementedException();
    }

    public List<ProfissionalSaudeModel> ListarProfissionalSaudeOnline()
    {
        throw new NotImplementedException();
    }

    public int QtdProfissionalSaudeOnline()
    {
        throw new NotImplementedException();
    }
}