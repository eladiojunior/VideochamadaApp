EmAtendimentoUsuario = {
    InitAtendimento: function () {
        $(".button-finalizar-atendimento").click(function () {
            ComunicacaoUsuarios.SairAtendimento();
            window.location.href = _contexto + "Atendimento/SairDoAtendimento";
        });
    },
    AlertaAtendimentoEncerrado: function () {
        alert("Profissional de Saúde saiu do atendimento.");
        window.location.href = _contexto + "Atendimento/SairDoAtendimento";
    }
}
$(function () {
    EmAtendimentoUsuario.InitAtendimento();
});