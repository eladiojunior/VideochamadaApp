ComunicacaoUsuario = {
    AtivarComunicacaoUsuario: function () {
        idProfissionalHub = "Profissional-987654321"
        connection.invoke("ConectarAtendimento", idAtendimentoHub, idProfissionalHub);
        $(".idProfissional").text(idProfissionalHub);
    }
}