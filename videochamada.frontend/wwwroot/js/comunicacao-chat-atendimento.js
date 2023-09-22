ComunicacaoChat = {
    
    InitChatAtendimento: function () {
        
        //Posicionar no final da lista do chat...
        const divAreaChat = $(".chat-container");
        divAreaChat.scrollTop(divAreaChat.offset().top);
        
        //Ação do buttton de enviar...
        $(".button-enviar-mensagem-chat").click(function () {
            ComunicacaoChat.EnviarMensagemChat();
        });
        $(".input-mensagem-chat").keydown(function (event) {
            if (event.keyCode === 13) //ENTER
                ComunicacaoChat.EnviarMensagemChat();
        });
        $(".button-posicao-chat").click(function () {
            //Posicionar no final da lista do chat...
            const divAreaChat = $(".chat-container");
            divAreaChat.scrollTop(divAreaChat.offset().top);
            const fieldMensagem = $(".input-mensagem-chat");
            fieldMensagem.focus();
        });
        $(".button-posicao-video").click(function () {
            window.scroll({top: 0, behavior: "smooth" });
        });

        //Receber mensagem enviada
        connection.on('ReceberMensagem', (idUsuario, mensagem) => {
            const hasSender = (idUsuarioHub === idUsuario);
            const hasAvatar = (ultimoIdUsuario !== idUsuario);
            let strMensagem = "<div class='message'>";
            if (hasSender) {
                if (hasAvatar)
                    strMensagem += "<img class='img-chat img-chat-sender' src='/imgs/chat-sender.png' alt='Mensagem' title='Você'/>";
                else
                    strMensagem += "<div class='img-chat img-chat-sender'></div>";
                strMensagem += "<div class='message-content message-content-sender'>"+mensagem+"</div>";
            } else {
                strMensagem += "<div class='message-content message-content-receiver'>"+mensagem+"</div>";
                if (hasAvatar)
                    strMensagem += "<img class='img-chat img-chat-receiver' src='/imgs/chat-receiver.png' alt='Mensagem' title='Outro usuário'/>";
                else
                    strMensagem += "<div class='img-chat img-chat-receiver'></div>";
            }
            strMensagem += "</div>";
            const divAreaChat = $(".chat-container");
            divAreaChat.append(strMensagem);
            divAreaChat.scrollTop(divAreaChat.offset().top);
            ultimoIdUsuario = idUsuario;
        });
        
    },
    EnviarMensagemChat: function () {
        const fieldMensagem = $(".input-mensagem-chat");
        const mensagem = fieldMensagem.val();
        if (mensagem === null || mensagem === "") {
            fieldMensagem.focus();
            return;
        }
        //Enviar SingalR
        connection.invoke("EnviarMensagem", mensagem);
        fieldMensagem.val("");
        fieldMensagem.focus();
    }
}
$(function () {
    ComunicacaoChat.InitChatAtendimento();
});