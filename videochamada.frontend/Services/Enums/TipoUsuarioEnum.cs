using System.ComponentModel;

namespace VideoChatApp.FrontEnd.Services.Enums;

public enum TipoUsuarioEnum
{
    [Description("Cliente")]
    Cliente=1,
    
    [Description("Profissional de Saúde")]
    ProfissionalSaude=2,
    
    [Description("Usuário Gestor")]
    UsuarioGestor=3
}