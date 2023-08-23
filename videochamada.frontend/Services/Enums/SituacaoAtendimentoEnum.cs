using System.ComponentModel;

namespace VideoChatApp.FrontEnd.Services.Enums;

public enum SituacaoAtendimentoEnum
{
    
    [Description("Registrado")]
    Registrado=1,
    
    [Description("Fila Atendimento")]
    FilaAtendimento=2,

    [Description("Em Atendimento")]
    EmAtendimento=3,

    [Description("Finalizado")]
    Finalizado=4,

    [Description("Desistência")]
    Desistencia=5,
    
    [Description("Cancelado")]
    Cancelado=9

}