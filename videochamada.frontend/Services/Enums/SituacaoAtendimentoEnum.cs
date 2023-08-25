using System.ComponentModel;

namespace VideoChatApp.FrontEnd.Services.Enums;

public enum SituacaoAtendimentoEnum
{
    
    [Description("Registrado")]
    Registrado=1,
    
    [Description("Verificação de Dispositivo")]
    VerificacaoDispositivo=2,
    
    [Description("Fila Atendimento")]
    FilaAtendimento=3,

    [Description("Em Atendimento")]
    EmAtendimento=4,

    [Description("Finalizado")]
    Finalizado=5,

    [Description("Desistência")]
    Desistencia=6,
    
    [Description("Cancelado")]
    Cancelado=9

}