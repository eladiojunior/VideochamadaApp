/* --------------------------------------------
 Responsável pela comunicação dos usuários do
 atendimento (Cliente e Profissional de Saúde)
 para Video, Mensagens Chat e Arquivos.
 -------------------------------------------- */
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/videochamadaHub")
    .withAutomaticReconnect()
    .build();

let idConnectionHub = "";
let idAtendimentoHub = "";
let idUsuarioHub = "";

let peerConnection;
let streamMediaLocal;
let videoLocal; 
let videoRemoto;

let hasVideoLocal = true;

ComunicacaoUsuarios = {

    InitDispositivoCameraMicrofone: function () {
        videoLocal = document.getElementById('video-local');
        //Variavel para pegar permissão de camera e microfone, definição de acordo com navegador.
        navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia;
        navigator.mediaDevices.getUserMedia({ video: true, audio: true })
            .then(stream => {
                streamMediaLocal = stream;
                ComunicacaoUsuarios.SucessoPermissaoDispositivo();
            })
            .catch(error => {
                ComunicacaoUsuarios.ErroPermissaoDispositivo(error);
            });
    },
    SucessoPermissaoDispositivo: function () {
        $(".video-local-aguardando").hide();
        $(".video-local-aguardando").removeClass("d-flex");
        try {
            if (streamMediaLocal == null) {
                ComunicacaoUsuarios.MensagemUsuario("Câmera sem permissão, inexistente ou utilizado por outro aplicativo.");
                return;
            }
            videoLocal.srcObject = streamMediaLocal;
            ComunicacaoUsuarios.InitComunicacaoVideoRemota();
        } catch (e) {
            ComunicacaoUsuarios.MensagemUsuario("Câmera sem permissão, inexistente ou utilizado por outro aplicativo.");
            console.log('Erro no dispositivo da câmera: ' + e);
        }
    },
    ErroPermissaoDispositivo: function (error) {
        console.log(error);
        $(".video-local-aguardando").html("<span class='material-symbols-outlined fs-2 text-danger'>video_camera_front_off</span><span class='d-block text-muted' style='font-size:10px;'>Erro ao acessar o dispositivo!</span>");
        $(".video-local-aguardando").addClass("d-flex");
        camera_video=false; //Desativar...
        ComunicacaoUsuarios.ControlarButtonCamera($("#camera-button"), camera_video);
    },
    ControlarDispositivoCamera: function (hasControle) {
        if (hasControle) {
            ComunicacaoUsuarios.InitDispositivoCameraMicrofone();
            return;
        }
        //Desativar câmera... local
        let videoTrack = streamMediaLocal.getVideoTracks()[0];
        if (videoTrack) {
            videoTrack.stop(); // Isso vai desativar a câmera
            streamMediaLocal.removeTrack(videoTrack); // Isso vai remover a faixa de vídeo do stream local
            // Atualizar RTCPeerConnection
            let videoSender = peerConnection.getSenders().find(s => s.track && s.track.kind === 'video');
            if (videoSender) {
                peerConnection.removeTrack(videoSender);
            }
        }
        hasVideoLocal = false;
        //videoLocal.srcObject = null;
        //streamMediaLocal = null;
        $(".video-local-aguardando").html("<span class='material-symbols-outlined fs-2 text-secondary'>video_camera_front_off</span>");
        $(".video-local-aguardando").addClass("d-flex");
        $(".video-local-aguardando").show();
    },
    InitControleVideo: function () {
        let mute_video = true;
        $("#mute-button").click(function (){
            mute_video = mute_video !== true;
            ComunicacaoUsuarios.ControlarButtonMicrofone($(this), mute_video);
        });
        let camera_video = true;
        $("#camera-button").click(function (){
            camera_video = camera_video !== true;
            ComunicacaoUsuarios.ControlarButtonCamera($(this), camera_video);
            ComunicacaoUsuarios.ControlarDispositivoCamera(camera_video);
        });
    },
    ControlarButtonCamera: function (obj_button_camera, has_controle) {
        obj_button_camera.removeClass("on");
        obj_button_camera.removeClass("off");
        obj_button_camera.addClass(has_controle?"on":"off");
        obj_button_camera.find("span.material-symbols-outlined").text(has_controle?"video_camera_front":"video_camera_front_off");
        ComunicacaoUsuarios.MensagemUsuario(`Câmera: ${has_controle?"ATIVADA":"DESATIVADA"} na videochamada.`);
    },
    ControlarButtonMicrofone: function (obj_button_microfone, has_controle) {
        obj_button_microfone.removeClass("on");
        obj_button_microfone.removeClass("off");
        obj_button_microfone.addClass(has_controle?"on":"off");
        obj_button_microfone.find("span.material-symbols-outlined").text(has_controle?"mic":"mic_off");
        if (videoLocal === null)
            return;
        let audioTrack = streamMediaLocal.getAudioTracks()[0];
        if (audioTrack) {
            if (has_controle) {
                //Para ativar novamente, caso tenha sido desativado
                audioTrack.enabled = true;
                streamMediaLocal.addTrack(audioTrack);
                let sender = peerConnection.addTrack(audioTrack, streamMediaLocal);
            } else {// Desativar o áudio local
                audioTrack.stop(); // Isso vai desativar o microfone
                streamMediaLocal.removeTrack(audioTrack); // Isso vai remover a faixa de áudio do stream local
                // Atualizar RTCPeerConnection
                let audioSender = peerConnection.getSenders().find(s => s.track && s.track.kind === 'audio');
                if (audioSender) {
                    peerConnection.removeTrack(audioSender); // Isso vai parar de enviar a faixa de áudio
                }
            }
        }
        ComunicacaoUsuarios.MensagemUsuario(`Microfone: ${has_controle?"ATIVADO":"DESATIVADO"} na videochamada.`);
    },
    MensagemUsuario: function (msg) {
        $(".mensagem-usuario").text(msg);
        window.setTimeout(function (){
            $(".mensagem-usuario").text("Você está em uma videochamada neste atendimento.");
        }, 3000);
    },
    
    InitComunicacao: function () {
        
        console.log("Inicializar comunicação remota entre usuários.");
        idAtendimentoHub = $("#idAtendimento").val();
        idUsuarioHub = $("#idUsuario").val();
        
        connection.start()
        .then(() => {
            connection.invoke("ConectarAtendimento", idAtendimentoHub, idUsuarioHub);
            console.log("Conexão com o servidor de sinalização estabelecida.");
        })
        .catch((err) => {
            console.error(err.toString());
        });
        
        //Receber usuário conectado...
        connection.on("UsuarioConectado", (connectionId) => {
            idConnectionHub = connectionId;
        });
        
        //Identificar usuário desconectado...
        connection.on('UsuarioDesconectado', (connectionId) => {
            if (connectionId === idUsuarioHub) {
                EmAtendimentoUsuario.AlertaAtendimentoEncerrado();
            }
            connection.close();
        });
    },
    SairAtendimento: function () {
        connection.invoke("DesconectarAtendimento", idAtendimentoHub, idUsuarioHub);
    },
    InitComunicacaoVideoRemota: function () {
        console.log("Inicializar comunicação remota de video.");
        if (ComunicacaoUsuarios.EstabelecerComunicacaoVideoRemota())
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
        
    },
    EstabelecerComunicacaoVideoRemota: function () {

        try {

            // Configura o objeto RTCPeerConnection
            let configuration = { 'iceServers': [{ 'urls': 'stun:stun.l.google.com:19302' }] };
            peerConnection = new RTCPeerConnection(configuration);
            videoRemoto = document.getElementById("video-remoto");

            // Lidar com o ICE Candidate Events
            peerConnection.onicecandidate = function (event) {
                if (event.candidate) {
                    connection.invoke("SendIceCandidate",
                        JSON.stringify({ 'iceCandidate': event.candidate }), idAtendimentoHub);
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
                    JSON.stringify({ 'answer': answer }), idAtendimentoHub);
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
                connection.invoke("SendOffer",
                    JSON.stringify({ 'offer': offer }), idAtendimentoHub);
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
    ComunicacaoUsuarios.InitControleVideo();
    ComunicacaoUsuarios.InitDispositivoCameraMicrofone();
    ComunicacaoUsuarios.InitComunicacao();
});