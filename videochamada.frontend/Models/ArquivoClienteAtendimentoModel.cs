namespace videochamada.frontend.Models;

public class ArquivoClienteAtendimentoModel
{
    public string Id { get; set; }
    public string NomeOriginal { get; set; }
    public string NomeFisico { get; set; }
    public long TamanhoBytes { get; set; }
    public byte[] BytesArquivo { get; set; }
    public string TipoExtensao { get; set; }
    public DateTime DataHoraEnvio { get; set; }
}