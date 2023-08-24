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
        profissionalSaudeNovo.Online = false;
            
        _profissionais.TryAdd(profissionalSaudeNovo.Id, profissionalSaudeNovo);
        return profissionalSaudeNovo;
    }

    public ProfissionalSaudeModel ObterProfissionalSaude(string idProfissional)
    {
        if (string.IsNullOrEmpty(idProfissional))
            return null;
        return _profissionais.GetValueOrDefault(idProfissional);
    }

    public List<ProfissionalSaudeModel> ListarProfissionalSaude()
    {
        return _profissionais.Values.ToList();
    }

    public List<ProfissionalSaudeModel> ListarProfissionalSaudeOnline()
    {
        var listaOnline = _profissionais.Values.Where(w => w.Online == true);
        return listaOnline.ToList();
    }

    public int QtdProfissionalSaudeOnline()
    {
        var qtd = _profissionais.Values.Count(w => w.Online == true);
        return qtd;
    }
    
}