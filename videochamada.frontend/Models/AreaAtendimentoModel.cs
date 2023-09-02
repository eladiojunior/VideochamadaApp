namespace videochamada.frontend.Models;

public class AreaAtendimentoModel
{
    public ProfissionalSaudeModel ProfissionalSaude { get; set; }
    public int QtdProfissionaisOnline { get; set; }
    public int QtdClientesNaFila { get; set; }
    public int QtdClientesEmAtendimento { get; set; }
    public List<AtendimentoModel> Atendimentos { get; set; }
}