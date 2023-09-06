using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace videochamada.frontend.Models;

public class ClienteRegistroModel
{
    public string? Id { get; set; }
    
    [Required(ErrorMessage = "Nome do cliente obrigatório.")]
    [DisplayName("Nome do Cliente")]
    public string Nome { get; set; }
    
    [Required(ErrorMessage = "Data de Nascimento do cliente obrigatório.")]
    [DisplayName("Data Nascimento do Cliente")]
    public string DataNascimento { get; set; }

    [Required(ErrorMessage = "Sexo (biológico) do cliente obrigatório.")]
    [DisplayName("Sexo (biológico) do Cliente")]
    public string Sexo { get; set; }

    [Required(ErrorMessage = "E-mail do cliente obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail não informado ou inválido.")]
    [DisplayName("E-mail do Cliente")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Telefone do cliente obrigatório.")]
    [Phone(ErrorMessage = "Telefone não informado ou inválido.")]
    [DisplayName("Telefone do Cliente")]
    public string Telefone { get; set; }
    
}