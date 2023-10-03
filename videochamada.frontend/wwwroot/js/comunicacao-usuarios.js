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

// Definição da qualidade da conexão do usuário.
//1=Alta qualidade
//2=Média qualidade
//3=Alta qualidade
let qualidadeConexao = 3; //Padrão baixa

ComunicacaoUsuarios = {

    InitDispositivoCameraMicrofone: function () {

        //Definição da qualidade de transmissão do video...
        const ALTA_QUALIDADE_VIDEO = { video: { width: { min: 1280 }, height: { min: 720 }, frameRate: { min: 30 } }};
        const MEDIA_QUALIDADE_VIDEO = { video: { width: { min: 640 }, height: { min: 480 }, frameRate: { min: 24, ideal: 30 } }};
        const BAIXA_QUALIDADE_VIDEO = { video: { width: { min: 480 }, height: { min: 360 }, frameRate: { min: 15, ideal: 24 } }};
        
        videoLocal = document.getElementById('video-local');
        
        const qualidadeVideoStream= (qualidadeConexao === 1 ? ALTA_QUALIDADE_VIDEO : (qualidadeConexao === 2 ? MEDIA_QUALIDADE_VIDEO: BAIXA_QUALIDADE_VIDEO));
        //Variavel para pegar permissão de camera e microfone, definição de acordo com navegador.
        navigator.getUserMedia = (navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia);
        navigator.mediaDevices.getUserMedia({
            qualidadeVideoStream, audio: true 
        })
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
        $(".video-local-aguardando").html("<span class='material-icons fs-2 text-danger'>videocam_off</span><span class='d-block text-muted' style='font-size:10px;'>Erro ao acessar o dispositivo!</span>");
        $(".video-local-aguardando").addClass("d-flex");
        camera_video=false; //Desativar...
        ComunicacaoUsuarios.ControlarButtonCamera($("#camera-button"), camera_video);
    },
    AtualizarQualidadeVideo: function () {
        const qualidadeVideoStream = (qualidadeConexao === 1 ? ALTA_QUALIDADE_VIDEO : (qualidadeConexao === 2 ? MEDIA_QUALIDADE_VIDEO: BAIXA_QUALIDADE_VIDEO));
        navigator.mediaDevices.getUserMedia({ qualidadeVideoStream, audio: true });
        console.log("Atualizar a qualidade do vídeo: " + qualidadeVideoStream);
    },
    ControlarDispositivoCamera: function (hasControle) {
        if (hasControle) {
            ComunicacaoUsuarios.InitDispositivoCameraMicrofone();
            connection.invoke("ControleAudioVideoAtendimento", "video", true);
            return;
        }
        //Desativar câmera... local
        let videoTrack = streamMediaLocal.getVideoTracks()[0];
        if (videoTrack) {
            videoTrack.stop(); // Isso vai desativar a câmera
            streamMediaLocal.removeTrack(videoTrack); // Isso vai remover a faixa de vídeo do stream local
            let videoSender = peerConnection.getSenders().find(s => s.track && s.track.kind === 'video');
            if (videoSender) {
                peerConnection.removeTrack(videoSender);
            }
        }
        hasVideoLocal = false;
        connection.invoke("ControleAudioVideoAtendimento", "video", false);
        $(".video-local-aguardando").html("<span class='material-icons fs-2 text-secondary'>videocam_off</span>");
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
        obj_button_camera.find("span.material-icons").text(has_controle?"videocam":"videocam_off");
        ComunicacaoUsuarios.MensagemUsuario(`Câmera: ${has_controle?"ATIVADA":"DESATIVADA"} na videochamada.`);
    },
    ControlarButtonMicrofone: function (obj_button_microfone, has_controle) {
        obj_button_microfone.removeClass("on");
        obj_button_microfone.removeClass("off");
        obj_button_microfone.addClass(has_controle?"on":"off");
        obj_button_microfone.find("span.material-icons").text(has_controle?"mic":"mic_off");
        if (videoLocal === null)
            return;
        let audioTrack = streamMediaLocal.getAudioTracks()[0];
        if (audioTrack) {
            if (has_controle) {
                //Para ativar novamente, caso tenha sido desativado
                audioTrack.enabled = true;
                streamMediaLocal.getTracks().forEach(track => {
                    peerConnection.addTrack(track, streamMediaLocal);
                });
            } else {// Desativar o áudio local
                audioTrack.stop(); // Isso vai desativar o microfone
                streamMediaLocal.removeTrack(audioTrack); // Isso vai remover a faixa de áudio do stream local
                // Atualizar RTCPeerConnection
                let audioSender = peerConnection.getSenders().find(s => s.track && s.track.kind === 'audio');
                if (audioSender) {
                    peerConnection.removeTrack(audioSender); // Isso vai parar de enviar a faixa de áudio
                }
            }
            connection.invoke("ControleAudioVideoAtendimento", "audio", has_controle);
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
        
        //Receber infomações do dispositivo Audio ou Video.
        connection.on("ReceiveAudioVideoAtendimento", (dispositivo, has_controle) => {
            if (dispositivo === 'video') {
                ComunicacaoUsuarios.MensagemUsuario(`Usuário conectado: ${has_controle?"ativou":"desativou"} seu vídeo.`);
                ComunicacaoUsuarios.DispositivoRemotoVideo(has_controle);
            } else if (dispositivo === 'audio') {
                ComunicacaoUsuarios.MensagemUsuario(`Usuário conectado: ${has_controle?"ativou":"desativou"} seu áudio.`);
            }
        });
        
    },
    DispositivoRemotoVideo: function (has_controle) {
        if (has_controle) 
        {
            $(".video-remoto-aguardando").hide();
            $(".video-remoto-aguardando").removeClass("d-flex");
        } 
        else 
        {
            videoRemoto.srcObject = null;
            $(".video-remoto-aguardando").html("<span class='material-icons fs-2 text-secondary'>videocam_off</span>");
            $(".video-remoto-aguardando").addClass("d-flex");
            $(".video-remoto-aguardando").show();
        }
    },
    SairAtendimento: function () {
        connection.invoke("DesconectarAtendimento", idAtendimentoHub, idUsuarioHub);
    },
    InitComunicacaoVideoRemota: function () {
        console.log("Inicializar comunicação remota de video.");
        const hasConnectado = ComunicacaoUsuarios.EstabelecerComunicacaoVideoRemota();
        ComunicacaoUsuarios.DispositivoRemotoVideo(hasConnectado);
    },
    EstabelecerComunicacaoVideoRemota: function () {

        const BAIXA_LATENCIA = 100; //0ms a 100ms: Baixa Latência (Qualidade Alta)
        const MEDIA_LATENCIA = 200; //101ms a 200ms: Latência Média (Qualidade Média)
        const ALTA_LATENCIA = 201; //201ms acima: Alta Latência (Qualidade Baixa)
        
        try {
            
            // Configura o objeto RTCPeerConnection
            let configuration = { 'iceServers': [{ 'urls': 'stun:stun.l.google.com:19302' }] };
            peerConnection = new RTCPeerConnection(configuration);
            peerConnection.getStats(null).then(stats => {
                stats.forEach(report => {
                    if (report.type === 'candidate-pair' && report.state === 'succeeded') {
                        if (report.currentRoundTripTime <= BAIXA_LATENCIA) {
                            console.log(report.currentRoundTripTime + ' -> 0ms a 100ms: Baixa Latência (Qualidade Alta)');
                            if (qualidadeConexao !== 1)
                                ComunicacaoUsuarios.AtualizarQualidadeVideo();
                            qualidadeConexao = 1; //Alta qualidade de vídeo.
                        } else if (report.currentRoundTripTime <= MEDIA_LATENCIA) {
                            console.log(report.currentRoundTripTime + ' -> 101ms a 200ms: Latência Média (Qualidade Média)');
                            if (qualidadeConexao !== 2)
                                ComunicacaoUsuarios.AtualizarQualidadeVideo();
                            qualidadeConexao = 2; //Média qualidade de vídeo.
                        } else if (report.currentRoundTripTime >= ALTA_LATENCIA) {
                            console.log(report.currentRoundTripTime + ' -> 201ms acima: Alta Latência (Qualidade Baixa)');
                            if (qualidadeConexao !== 3)
                                ComunicacaoUsuarios.AtualizarQualidadeVideo();
                            qualidadeConexao = 3; //Baixa qualidade de vídeo.
                        }
                        if (report.packetsLost > 0) {
                            console.log('Identificamos perda de pacotes na comunicação.');
                        }
                    }
                });
            });
            
            videoRemoto = document.getElementById("video-remoto");
            
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