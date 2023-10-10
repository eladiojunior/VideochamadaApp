HistoricoSaudeCliente = {
    InitHistorico: function () {
        window.setInterval(function () {
            var texto = $("#textoMotivoAtendimento").val();
            if (texto!=="")
                HistoricoSaudeCliente.GravarTextoMotivoAtendimento(texto);
        }, 5000);
    },
    GravarTextoMotivoAtendimento: function (textoMotivoAtendimento) {
        $.ajax({
            cache: false,
            type: "POST",
            url: _contexto + "Atendimento/GravarTextoMotivoAtendimento",
            data: {
                "idAtendimento" : idAtendimentoHub,
                "textoMotivoAtendimento" : textoMotivoAtendimento
            },
            dataType: "json",
            success: function (result) {
                if (result.hasErro) {
                    ComunicacaoUsuarios.MensagemUsuario(result.erros[0]);
                    return;
                }
                const dataHoraGravacao = result.model.dataHoraGravacao;
                $(".mensagem-motivo-atendimento").text("Motivo gravado com sucesso, última atualização "+dataHoraGravacao+".");
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("GravarTextoMotivoAtendimento: " + errorThrown);
            }
        });
    }
}
$(function () {
    HistoricoSaudeCliente.InitHistorico();
});