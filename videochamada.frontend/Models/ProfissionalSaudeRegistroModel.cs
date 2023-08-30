using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace videochamada.frontend.Models;

public class ProfissionalSaudeRegistroModel
{
    
    [Required(ErrorMessage = "Nome do profissional não informado")]
    [DisplayName("Nome do Profissional")]
    public string Nome { get; set; }
    
    [Required(ErrorMessage = "E-mail do profissional não informado")]
    [EmailAddress(ErrorMessage = "E-mail não informado ou inválido.")]
    [DisplayName("E-mail do Profissional")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Telefone não informado")]
    [Phone(ErrorMessage = "Telefone não informado ou inválido.")]
    [DisplayName("Telefone do Profissional")]
    public string Telefone { get; set; }

    [Required(ErrorMessage = "Especialidade/Área do profissional não informada")]
    [DisplayName("Especialidade/Área")]
    public string Especialidade { get; set; }

    [Required(ErrorMessage = "Senha de acesso  não informada")]
    [DisplayName("Senha")]
    public string SenhaAcesso { get; set; }
    
    [Required(ErrorMessage = "Confirmação de senha não informada")]
    [DisplayName("Confirmação")]
    public string ConfirmacaoSenhaAcesso { get; set; }

}