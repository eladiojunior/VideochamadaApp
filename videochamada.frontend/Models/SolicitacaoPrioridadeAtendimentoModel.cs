namespace videochamada.frontend.Models;

public class SolicitacaoPrioridadeAtendimentoModel
{
    public string IdCliente { get; set; }
    public string IdAtendimento { get; set; }
    public List<PerguntaRespostaModel> Perguntas { get; set; }
}