ComunicacaoUsuario = {
    AtivarComunicacaoUsuario: function () {
        idClienteHub = "Cliente-123456789";
        connection.invoke("ConectarAtendimento", idAtendimentoHub, idClienteHub);
        $(".idCliente").text(idClienteHub);
    }
}