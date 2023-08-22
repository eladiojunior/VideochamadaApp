using System.ComponentModel.DataAnnotations;

namespace videochamada.frontend.Models;

public class AvaliacaoAtendimentoModel
{
    public string IdAtendimento { get; set; }
    
    public string IdCliente { get; set; }
    
    [Required(ErrorMessage = "Selecione uma nota para avaliar o atendimento.")]
    [Range(minimum:1, maximum:5, ErrorMessage = "Selecione uma nota de 1 a 5 para avaliar o atendimento.")]
    public int Nota { get; set; }
    
    public string? Comentario { get; set; }
    
    public bool HasDesistencia { get; set; }
}