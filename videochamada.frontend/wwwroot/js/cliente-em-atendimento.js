EmAtendimentoUsuario = {
    InitAtendimento: function () {
        $(".button-finalizar-atendimento").click(function () {
            ComunicacaoUsuarios.SairAtendimento();
            window.location.href = _contexto + "Atendimento/SairDoAtendimento?idAtendimento=" + idAtendimentoHub;
        });
    },
    AlertaAtendimentoEncerrado: function () {
        alert("Profissional de Saúde saiu do atendimento.");
        window.location.href = _contexto + "Atendimento/SairDoAtendimento?idAtendimento=" + idAtendimentoHub;
    }
}
$(function () {
    EmAtendimentoUsuario.InitAtendimento();
});