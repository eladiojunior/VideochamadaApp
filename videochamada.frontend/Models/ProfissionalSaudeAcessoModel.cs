using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace videochamada.frontend.Models;

public class ProfissionalSaudeAcessoModel
{
    
    [Required(ErrorMessage = "E-mail do profissional de saúde obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail não informado ou inválido.")]
    [DisplayName("E-mail do profissional")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Senha de acesso obrigatória.")]
    [DisplayName("Senha de acesso")]
    public string Senha { get; set; }
    
}