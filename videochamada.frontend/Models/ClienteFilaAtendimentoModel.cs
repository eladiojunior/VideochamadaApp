namespace videochamada.frontend.Models;

public class ClienteFilaAtendimentoModel
{
    public ClienteModel Cliente { get; set; }
    public int PosicaoNaFila { get; set; }
    public int QtdProfissionaisOnline { get; set; }
}