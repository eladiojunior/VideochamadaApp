EmAtendimentoUsuario = {
    InitAtendimento: function () {
        $(".button-finalizar-atendimento").click(function () {
            EmAtendimentoUsuario.FinalizarAtendimento();
        });
    },
    FinalizarAtendimento: function () {
        $.ajax({
            cache: false,
            type: "POST",
            url: _contexto + "EquipeSaude/FinalizarAtendimento",
            data: {"idAtendimento" : idAtendimentoHub},
            dataType: "json",
            success: function (result) {
                if (result.hasErro) {
                    ComunicacaoUsuarios.MensagemUsuario(result.erros[0]);
                    return;
                }
                ComunicacaoUsuarios.SairAtendimento();
                window.close();
                
                const window_opened = window.opener;
                console.log(window_opened);
                if (window_opened)
                    window_opened.AreaAtendimentoProfissional.RetomarVerificacaoProximoCliente();
                
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("FinalizarAtendimento: " + errorThrown);
            }
        });
    },
    AlertaAtendimentoEncerrado: function () {
        alert("Cliente saiu do atendimento.");
        window.close();
    }
}
$(function () {
    EmAtendimentoUsuario.InitAtendimento();
});