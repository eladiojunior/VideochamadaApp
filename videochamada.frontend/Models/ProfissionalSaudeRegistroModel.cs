using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace videochamada.frontend.Models;

public class ProfissionalSaudeRegistroModel
{
    public string? Id { get; set; }
    
    [Required]
    [StringLength(100)]
    [DisplayName("Nome do Profissional")]
    public string Nome { get; set; }
    
    [Required]
    [EmailAddress(ErrorMessage = "E-mail não informado ou inválido.")]
    [DisplayName("E-mail do Profissional")]
    public string Email { get; set; }
    
    [Required]
    [Phone(ErrorMessage = "Telefone não informado ou inválido.")]
    [DisplayName("Telefone do Profissional")]
    public string Telefone { get; set; }

    [Required]
    [StringLength(100)]
    [DisplayName("Especialidade do Profissional")]
    public string Especialidade { get; set; }

}