
let streamMediaLocal;
let videoLocal; //Area do cliente
let videoRemoto; //Area do Profissional

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/videochamadaHub")
    .build();

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
                EmAtendimento.MensagemCliente("Câmera sem permissão, inexistente ou utilizado por outro aplicativo.");
                return;
            }
            videoLocal.srcObject = streamMediaLocal;
            EmAtendimento.InitComunicacaoVideoRemota();
        } catch (e) {
            EmAtendimento.MensagemCliente("Câmera sem permissão, inexistente ou utilizado por outro aplicativo.");
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
        EmAtendimento.MensagemCliente(`Câmera: ${has_controle?"ATIVADA":"DESATIVADA"} na videochamada.`);
    },
    ControlarButtonMicrofone: function (obj_button_microfone, has_controle) {
        obj_button_microfone.removeClass("on");
        obj_button_microfone.removeClass("off");
        obj_button_microfone.addClass(has_controle?"on":"off");
        obj_button_microfone.find("span.material-symbols-outlined").text(has_controle?"mic":"mic_off");
        if (videoLocal)
            videoLocal.muted = !has_controle;
        EmAtendimento.MensagemCliente(`Microfone: ${has_controle?"ATIVADO":"DESATIVADO"} na videochamada.`);
    },
    MensagemCliente: function (msg) {
        $(".mensagem-usuario").text(msg);
        window.setTimeout(function (){
            $(".mensagem-usuario").text("Você está em uma videochamada com um profissional de saúde.");
        }, 3000);
    },
    
    InitComunicacaoVideoRemota: function () {
        
        console.log("Inicializar comunicação remota de video.");

        connection.start()
            .then(() => {
                console.log("Conexão com o servidor de sinalização estabelecida.");
                if (EmAtendimento.EstabelecerComunicacao()) 
                {//Connexao remota estabelecida...
                    $(".video-remoto-aguardando").hide();
                    $(".video-remoto-aguardando").removeClass("d-flex");
                } 
                else 
                {
                    //Desativar camera...
                    videoRemoto.srcObject = null;
                    $(".video-remoto-aguardando").html("<span class='material-symbols-outlined fs-2 text-secondary'>video_camera_front_off</span>");
                    $(".video-remoto-aguardando").addClass("d-flex");
                    $(".video-remoto-aguardando").show();
                }
            })
            .catch((err) => {
                console.error(err.toString());
            });
        
        connection.on('UserDisconnected', async (senderId) => {
            console.log("Desconectado: " + senderId);
            connection.close();
        });
        
    },
    EstabelecerComunicacao: function () {
        
        try {
            
            let idCliente = $("#idCliente").val();
            let idProfissional = $("#idProfissional").val();
            console.log("IdCliente: " + idCliente);
            console.log("IdProfissional: " + idProfissional);

            // Configura o objeto RTCPeerConnection
            let configuration = { 'iceServers': [{ 'urls': 'stun:stun.l.google.com:19302' }] };
            let peerConnection = new RTCPeerConnection(configuration);
            videoRemoto = document.getElementById("video-remoto");
            
            // Lidar com o ICE Candidate Events
            peerConnection.onicecandidate = function (event) {
                if (event.candidate) {
                    connection.invoke("SendIceCandidate", 
                        JSON.stringify({ 'iceCandidate': event.candidate, 'connectionId': idProfissional }));
                }
            };

            // Lidar com o vídeo remoto
            peerConnection.ontrack = function (event) {
                videoRemoto.srcObject = event.streams[0];
            };

            // Sinalização de eventos
            connection.on("ReceiveOffer", async (offer) => {

                const result = JSON.parse(offer);
                const remoteOffer = new RTCSessionDescription(result.offer);
                await peerConnection.setRemoteDescription(remoteOffer);
                const answer = await peerConnection.createAnswer();
                await peerConnection.setLocalDescription(answer);
                
                connection.invoke("SendAnswer", 
                    JSON.stringify({ 'answer': answer, 'connectionId': idProfissional }));
            });

            connection.on("ReceiveAnswer", async (answer) => {
                const result = JSON.parse(answer);
                const remoteAnswer = new RTCSessionDescription(result.answer);
                await peerConnection.setRemoteDescription(remoteAnswer);
            });
            
            connection.on("ReceiveIceCandidate", async (iceCandidate) => {
                const result = JSON.parse(iceCandidate);
                await peerConnection.addIceCandidate(result.iceCandidate);
            });

            async function makeOffer() {
                const offer = await peerConnection.createOffer();
                await peerConnection.setLocalDescription(offer);
                connection.invoke("SendOffer", JSON.stringify({ 'offer': offer, 'connectionId': idProfissional}));
            }

            // Adicione os streams locais (deve ser feito depois de obter acesso à câmera/microfone)
            if (streamMediaLocal != null) {
                let localStream = streamMediaLocal;
                localStream.getTracks().forEach(track => {
                    peerConnection.addTrack(track, localStream);
                });
            }
            
            makeOffer().then(result => {
                console.log("Iniciando negociação...");
                
            });
            
            return true;
            
        } catch (erro) {
            console.error(erro);
            return false;
        }
    }
}
$(function () {
    EmAtendimento.InitChatAtendimento();
    EmAtendimento.InitControleVideo();
    EmAtendimento.InitDispositivoCameraMicrofone();
});

