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
            window.scroll({top: 0, behavior: "smooth" });
        });
    },
    InitDispositivoCameraMicrofone: function () {
        videoLocal = document.getElementById('video-local');
        //Variavel para pegar permissão de camera e microfone, definição de acordo com navegador.
        navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia;
        navigator.mediaDevices.getUserMedia({ video: true, audio: true })
            .then(stream => {
                streamMediaLocal = stream;
                EmAtendimento.SucessoPermissaoDispositivo();
            })
            .catch(error => {
                EmAtendimento.ErroPermissaoDispositivo(error);
            });
    },
    SucessoPermissaoDispositivo: function () {
        $(".video-local-aguardando").hide();
        $(".video-local-aguardando").removeClass("d-flex");
        try {
            if (streamMediaLocal == null) {
                EmAtendimento.MensagemUsuario("Câmera sem permissão, inexistente ou utilizado por outro aplicativo.");
                return;
            }
            videoLocal.srcObject = streamMediaLocal;
            ComunicacaoUsuarios.InitComunicacaoVideoRemota();
        } catch (e) {
            EmAtendimento.MensagemUsuario("Câmera sem permissão, inexistente ou utilizado por outro aplicativo.");
            console.log('Erro no dispositivo da câmera: ' + e);
        }
    },
    ErroPermissaoDispositivo: function (error) {
        console.log(error);
        $(".video-local-aguardando").html("<span class='material-symbols-outlined fs-2 text-danger'>video_camera_front_off</span><span class='d-block text-muted' style='font-size:10px;'>Erro ao acessar o dispositivo!</span>");
        $(".video-local-aguardando").addClass("d-flex");
        camera_video=false; //Desativar...
        EmAtendimento.ControlarButtonCamera($("#camera-button"), camera_video);
    },
    ControlarDispositivoCamera: function (hasControle) {
        if (hasControle) {
            EmAtendimento.InitDispositivoCameraMicrofone();
            return;
        }
        //Desativar camera...
        videoLocal.srcObject = null;
        streamMediaLocal = null;
        $(".video-local-aguardando").html("<span class='material-symbols-outlined fs-2 text-secondary'>video_camera_front_off</span>");
        $(".video-local-aguardando").addClass("d-flex");
        $(".video-local-aguardando").show();
    },
    InitControleVideo: function () {
        let mute_video = true;
        $("#mute-button").click(function (){
            mute_video = mute_video !== true;
            EmAtendimento.ControlarButtonMicrofone($(this), mute_video);
        });
        let camera_video = true;
        $("#camera-button").click(function (){
            camera_video = camera_video !== true;
            EmAtendimento.ControlarButtonCamera($(this), camera_video);
            EmAtendimento.ControlarDispositivoCamera(camera_video);
        });
    },
    ControlarButtonCamera: function (obj_button_camera, has_controle) {
        obj_button_camera.removeClass("on");
        obj_button_camera.removeClass("off");
        obj_button_camera.addClass(has_controle?"on":"off");
        obj_button_camera.find("span.material-symbols-outlined").text(has_controle?"video_camera_front":"video_camera_front_off");
        EmAtendimento.MensagemUsuario(`Câmera: ${has_controle?"ATIVADA":"DESATIVADA"} na videochamada.`);
    },
    ControlarButtonMicrofone: function (obj_button_microfone, has_controle) {
        obj_button_microfone.removeClass("on");
        obj_button_microfone.removeClass("off");
        obj_button_microfone.addClass(has_controle?"on":"off");
        obj_button_microfone.find("span.material-symbols-outlined").text(has_controle?"mic":"mic_off");
        if (videoLocal)
            videoLocal.muted = !has_controle;
        EmAtendimento.MensagemUsuario(`Microfone: ${has_controle?"ATIVADO":"DESATIVADO"} na videochamada.`);
    },
    MensagemUsuario: function (msg) {
        $(".mensagem-usuario").text(msg);
        window.setTimeout(function (){
            $(".mensagem-usuario").text("Você está em uma videochamada neste atendimento.");
        }, 3000);
    }
}
$(function () {
    EmAtendimento.InitChatAtendimento();
    EmAtendimento.InitControleVideo();
    EmAtendimento.InitDispositivoCameraMicrofone();
});