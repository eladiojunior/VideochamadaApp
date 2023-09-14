EmAtendimento = {
    InitChatAtendimento: function () {
        
        //Posicionar no final da lista do chat...
        const divAreaChat = $(".chat-container");
        divAreaChat.scrollTop(divAreaChat.offset().top);
        
        //Ação do buttton de enviar...
        $(".button-enviar-mensagem-chat").click(function () {
            EmAtendimento.EnviarMensagemChat();
        });
        $(".input-mensagem-chat").keydown(function (event) {
            if (event.keyCode === 13) //ENTER
                EmAtendimento.EnviarMensagemChat();
        });
        $(".button-posicao-chat").click(function () {
            //Posicionar no final da lista do chat...
            const divAreaChat = $(".chat-container");
            divAreaChat.scrollTop(divAreaChat.offset().top);
            const fieldMensagem = $(".input-mensagem-chat");
            fieldMensagem.focus();
        });
        $(".button-posicao-video").click(function () {
            //Posicionar no início da tela...
            window.scroll({top: 0, behavior: "smooth" });
        });
    },
    InitComunicacaoVideochamada: function () {
        console.log("Iniciar os componentes para videochamada.");
    },
    EnviarMensagemChat: function () {
        
        const fieldMensagem = $(".input-mensagem-chat");
        const mensagem = fieldMensagem.val();
        console.log("Enviar mensagem... [" + mensagem + "]");
        if (mensagem === null || mensagem === "") {
            fieldMensagem.focus();
            return;
        }
        
        const divAreaChat = $(".chat-container");
        divAreaChat.append("<div class='message'><img class='img-chat-sender' src='/imgs/chat-sender.png' alt='Enviado' title='Você'><div class='message-content message-content-sender'>"+mensagem+"</div></div>");
        divAreaChat.scrollTop(divAreaChat.offset().top);

        fieldMensagem.val("");
        fieldMensagem.focus();

    },
    InitControleVideo: function () {
        let mute_video = true;
        $("#mute-button").click(function (){
            $(this).removeClass("on");
            $(this).removeClass("off");
            mute_video = mute_video !== true;
            $(this).addClass(mute_video?"on":"off");
            $(this).find("span.material-symbols-outlined").text(mute_video?"mic":"mic_off");
            EmAtendimento.MensagemCliente(`Você ${mute_video?"ATIVOU":"DESATIVOU"} seu microfone na videochamada.`);
        });
        let camera_video = true;
        $("#camera-button").click(function (){
            $(this).removeClass("on");
            $(this).removeClass("off");
            camera_video = camera_video !== true;
            $(this).addClass(camera_video?"on":"off");
            $(this).find("span.material-symbols-outlined").text(camera_video?"video_camera_front":"video_camera_front_off");
            EmAtendimento.MensagemCliente(`Você ${camera_video?"ATIVOU":"DESATIVOU"} sua câmera na videochamada.`);
        });
    },
    MensagemCliente: function (msg) {
        $(".mensagem-usuario").text(msg);
        window.setTimeout(function (){
            $(".mensagem-usuario").text("Você está em uma videochamada com um profissional de saúde.");
        }, 3000);
    }
}
$(function () {
    EmAtendimento.InitChatAtendimento();
    EmAtendimento.InitControleVideo();
    EmAtendimento.InitComunicacaoVideochamada();
});

