namespace videochamada.frontend.Models;

public class MensagemChatModel
{
    public int Tipo { get; set; }
    public string Conteudo { get; set; }
    public string Destinatario { get; set; }
    public string Remetente { get; set; }
    public DateTime DataHoraEnvio { get; set; }
    public DateTime? DataHoraRebimento { get; set; }
    public DateTime? DataHoraLeitura { get; set; }
    public bool HasRecebida => DataHoraRebimento.HasValue;
    public bool HasLida => DataHoraLeitura.HasValue;
    //Geolocalização da mensagem
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string IpMaquinaUsuario { get; set; }
}