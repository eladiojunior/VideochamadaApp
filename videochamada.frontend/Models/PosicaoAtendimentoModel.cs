namespace videochamada.frontend.Models;

public class PosicaoAtendimentoModel
{
    public string IdCliente { get; set; }
    public int PosicaoNaFila { get; set; }
    public int QtdClientesFila { get; set; }
    public int QtdProfissionaisOnline { get; set; }
}