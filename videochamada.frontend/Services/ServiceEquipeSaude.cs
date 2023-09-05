using System.Collections.Concurrent;
using videochamada.frontend.Models;
using VideoChatApp.FrontEnd.Services.Exceptions;
using VideoChatApp.FrontEnd.Services.Interfaces;

namespace VideoChatApp.FrontEnd.Services;

public class ServiceEquipeSaude : IServiceEquipeSaude
{
    private IListenerServerClient _listenerServerClient;
    private static readonly ConcurrentDictionary<string, ProfissionalSaudeModel> _profissionais = new ConcurrentDictionary<string, ProfissionalSaudeModel>();
    private static readonly ConcurrentDictionary<string, UsuarioLogadoModel> _profissionaisLogados = new ConcurrentDictionary<string, UsuarioLogadoModel>();

    public ServiceEquipeSaude(IListenerServerClient listenerServerClient)
    {
        _listenerServerClient = _listenerServerClient;
    }
    
    public ProfissionalSaudeModel RegistrarProfissionalSaude(ProfissionalSaudeRegistroModel profissionalSaude)
    {
        if (profissionalSaude == null)
            throw new ServiceException("Informações do profissional de saúde obrigatórias.");
        if (string.IsNullOrEmpty(profissionalSaude.Nome))
            throw new ServiceException("Nome do profissional de saúde não informado.");
        if (string.IsNullOrEmpty(profissionalSaude.Email))
            throw new ServiceException("E-mail do profissional de saúde não informado.");
        if (string.IsNullOrEmpty(profissionalSaude.Telefone))
            throw new ServiceException("Telefone de contato do profissional de saúde não informado.");
        if (string.IsNullOrEmpty(profissionalSaude.Especialidade))
            throw new ServiceException("Especialidade do profissional de saúde não informada.");
        if (string.IsNullOrEmpty(profissionalSaude.SenhaAcesso))
            throw new ServiceException("Senha de acesso do profissional de saúde não informada.");
        if (string.IsNullOrEmpty(profissionalSaude.ConfirmacaoSenhaAcesso))
            throw new ServiceException("Confirmação de senha de acesso não informada.");
        if (!profissionalSaude.SenhaAcesso.Equals(profissionalSaude.ConfirmacaoSenhaAcesso))
            throw new ServiceException("Confirmação da senha de acesso não confere, verifique.");
        
        //Verificar se profissional existe pelo e-mail.
        var profissionalSaudeExistente = ObterProfissionalSaudePorEmail(profissionalSaude.Email.Trim());
        if (profissionalSaudeExistente != null)
            throw new ServiceException("Profissional de saúde já registrado.");
        
        var profissionalSaudeNovo = new ProfissionalSaudeModel();
        
        profissionalSaudeNovo.Id = ServiceHelper.GerarId();
        profissionalSaudeNovo.Nome = profissionalSaude.Nome;
        profissionalSaudeNovo.Email = profissionalSaude.Email.Trim().ToLower();
        profissionalSaudeNovo.Telefone = profissionalSaude.Telefone;
        profissionalSaudeNovo.Especialidade = profissionalSaude.Especialidade;
        profissionalSaudeNovo.SenhaAcesso = profissionalSaude.SenhaAcesso;
        profissionalSaudeNovo.Online = false;
            
        _profissionais.TryAdd(profissionalSaudeNovo.Id, profissionalSaudeNovo);
        
        return profissionalSaudeNovo;
    }

    public ProfissionalSaudeModel AutenticarProfissionalSaude(string email, string senha)
    {
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            return null;

        const string mensagemSemAcesso = "Não foi possível realizar o acesso, verifique os dados informados.";
        
        var profissional = ObterProfissionalSaudePorEmail(email);
        if (profissional == null)
            throw new ServiceException(mensagemSemAcesso);
        if (!profissional.SenhaAcesso.Equals(senha))
            throw new ServiceException(mensagemSemAcesso);

        profissional.EmAtendimento = false;
        profissional.Online = false;
        
        //Registrar login usuario...
        RegistrarUsuarioLogado(profissional.Id);
        
        return profissional;
        
    }

    public void LogoffProfissionalSaude(string idProfissional)
    {
        var profissional = ObterProfissionalSaude(idProfissional);
        if (profissional == null)
            return;
        //Remover usuário dos logados...
        _profissionaisLogados.Remove(idProfissional, out var usuario);
    }

    private void RegistrarUsuarioLogado(string idProfissional)
    {
        var usuario = new UsuarioLogadoModel();
        usuario.IdUsuario = idProfissional;
        usuario.DataHoraLogin = DateTime.Now;
        usuario.DataHoraUltimaVerificacao = DateTime.Now;
        usuario.IpMaquinaUsuario = _listenerServerClient.IpMaquinaUsuario();
        _profissionaisLogados.TryAdd(idProfissional, usuario);
    }

    public void AtualizarSituacaoProfissionalAendimento(string idProfissional, bool hasSituacaoAtendimento)
    {
        var profissional = ObterProfissionalSaude(idProfissional);
        profissional.Online = hasSituacaoAtendimento;
    }

    public int QtdProfissionalSaudeLogados()
    {
        return _profissionaisLogados.Count();
    }

    public int QtdProfissionalSaudeEmAtendimento()
    {
        return _profissionais.Values.Count(c => c.EmAtendimento == true);
    }

    private ProfissionalSaudeModel ObterProfissionalSaudePorEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return null;
        return _profissionais.Values.FirstOrDefault(w => w.Email.Equals(email));
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