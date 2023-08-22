namespace videochamada.frontend.Models;

public class AtendimentoModel
{
    public string Id { get; set; }
    public string IdCliente { get; set; }
    public DateTime DataInicial { get; set; }
    public DateTime? DataFinal { get; set; }
    public string Situacao { get; set; }
    public int Nota { get; set; }
    public string Comentario { get; set; }
}