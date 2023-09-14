ArquivoAtendimento = {
    Init: function () {
        $(".button-init-enviar-arquivo").click(function () {
            $("#modalEnviarArquivoAtendimento").show();
        });
        $(".button-enviar-arquivo").click(function () {
            ArquivoAtendimento.EnviarArquivo();
        });
    },
    EnviarArquivo: function () {
        console.log("Enviar arquivo...");
    }
}
$(function () {
    ArquivoAtendimento.Init();
});

