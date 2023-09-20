
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/videochamadaHub")
    .withAutomaticReconnect()
    .build();

let idAtendimento = "123456789";
let idConnectionHub = "";

ComunicacaoUsuarios = {
    
    InitComunicacao: function () {
        
        console.log("Inicializar comunicação remota entre usuários.");

        connection.start()
        .then(() => {
            console.log("Conexão com o servidor de sinalização estabelecida.");
        })
        .catch((err) => {
            console.error(err.toString());
        });
        
        //Identificar usuário desconectado...
        connection.on('UsuarioDesconectado', (connectionId) => {
            console.log("Usuário desconectado: " + connectionId);
            connection.close();
        });
        
        //Receber usuário conectado...
        connection.on("UsuarioConectado", (signal, connectionId) => {
            idConnectionHub = connectionId; 
            connection.invoke("EnviarUsuarioAtendimento", idConnectionHub, idAtendimento);
            console.log("Usuário conectado: " + connectionId);
        });
        
    },
    EstabelecerComunicacao: function () {
        
        try {
            

            // Configura o objeto RTCPeerConnection
            let configuration = { 'iceServers': [{ 'urls': 'stun:stun.l.google.com:19302' }] };
            let peerConnection = new RTCPeerConnection(configuration);
            videoRemoto = document.getElementById("video-remoto");
            
            // Lidar com o ICE Candidate Events
            peerConnection.onicecandidate = function (event) {
                if (event.candidate) {
                    connection.invoke("SendIceCandidate", 
                        JSON.stringify({ 'iceCandidate': event.candidate }), idProfissional);
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
                    JSON.stringify({ 'answer': answer }), idProfissional);
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
                    JSON.stringify({ 'offer': offer }), idProfissional);
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