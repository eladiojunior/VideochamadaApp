﻿using System.ComponentModel;

namespace VideoChatApp.FrontEnd.Services.Enums;

public enum UsuarioOrigemMensagemEnum
{
    [Description("Cliente")]
    Cliente=1,
    
    [Description("Profissional de Saúde")]
    ProfissionalSaude=2,
    
}