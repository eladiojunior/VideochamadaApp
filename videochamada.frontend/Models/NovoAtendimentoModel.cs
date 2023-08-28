using System.ComponentModel.DataAnnotations;

namespace videochamada.frontend.Models;

public class NovoAtendimentoModel
{
    public string IdCliente { get; set; }
    public bool HasTermoUso { get; set; }
    
    //Geolocalização do Cliente no Atendimento
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}