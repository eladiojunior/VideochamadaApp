namespace videochamada.frontend.Models;

public class AreaPainelGestaoModel
{
    public UsuarioGestorModel UsuarioGestor { get; set; } 
    public DateTime DataHoraAtualizacaoPainel { get; set; }
    public string DataHoraAtualizacaoPainelString => DataHoraAtualizacaoPainel.ToString("dd/MM/yyyy HH:mm:ss");

    //Clientes
    public int QtdClientesRegistrados { get; set; }
    public int QtdClientesFilaAtendimento { get; set; }
    public int QtdClientesEmAtendimento { get; set; }
    
    //Profissionais
    public int QtdProfissionaisLogados { get; set; }
    public int QtdProfissionaisOnline { get; set; }
    public int QtdProfissionaisEmAtendimento { get; set; }

    //Atendimentos
    public DateTime DataHoraAtendimentos { get; set; }
    public string DataHoraAtendimentosString1 => DataHoraAtendimentos.ToString("dd/MM/yyyy '('dddd')'");
    public string DataHoraAtendimentosString2 => DataHoraAtendimentos.ToString("dd/MM/yyyy");
    
    public int QtdAtendimentosCancelados { get; set; }
    public int QtdAtendimentosDesistencias { get; set; }
    public int QtdAtendimentosFinalizados { get; set; }
    public string TempoMedioAtendimentos { get; set; }
    public string TempoMedioNaFilaAtendimento { get; set; }
    public string AvaliacaoMediaAtendimentos { get; set; }
}