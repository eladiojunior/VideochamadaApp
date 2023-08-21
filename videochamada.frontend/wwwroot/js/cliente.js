const webm9MimeCodec = 'video/webm;codecs="vp9"'
const segmentLimit = 20000;
const localVideo = document.getElementById('local-video');

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/videochamadaHub")
    .build();

let mediaRecorder = null
connection.start()
    .then(() => {
        console.log("Conexão com o servidor de sinalização estabelecida.");
    })
    .catch((err) => {
        console.error(err.toString());
    });




connection.on("ReceiveSignal", (signal, connectionId) => {
    console.log(`Conectado, signal: ${signal}, com o ID: ${connectionId}`);
});

let targetPeerId;
connection.on("UserJoined", (userInfo) => {
    const jsonUser = JSON.parse(userInfo);
    console.log(jsonUser);
    let connectionId = jsonUser.ConnectionId;
    let userName = jsonUser.UserName;
    console.log(`Novo usuário ${userName} conectado com o ID: ${connectionId}`);
    targetPeerId = connectionId;
});

connection.on('UserDisconnected', () => {
    connection.close();
    const localVideo = document.getElementById('local-video');
    const remoteVideo = document.getElementById('remote-video');
    if (localVideo.srcObject) {
        localVideo.srcObject.getTracks().forEach(track => track.stop());
        localVideo.srcObject = null;
    }
    if (remoteVideo.srcObject) {
        remoteVideo.srcObject.getTracks().forEach(track => track.stop());
        remoteVideo.srcObject = null;
    }
});



// Vamos considerar que as seguintes variáveis já foram definidas no topo do seu JS:
const configuration = {
    iceServers: [{ urls: 'stun:stun.l.google.com:19302' }]
};
let peerConnection  = new RTCPeerConnection(configuration);

peerConnection.onicecandidate = (event) => {
    if (event.candidate) {
        // Enviar o candidato ICE para o outro par através do servidor de sinalização
        connection.invoke("SendSignal",
            JSON.stringify({ 'ice-candidate': event.candidate }), targetPeerId);
    }
};
peerConnection.ontrack = (event) => {
    // Adicione o fluxo de mídia recebido ao elemento de vídeo remoto
    const remoteVideo = document.getElementById("remote-video");
    if (remoteVideo.srcObject !== event.streams[0]) {
        remoteVideo.srcObject = event.streams[0];
    }
};

// Variavel para pegar permissão de camera e microfone, definição de acordo com navegador.
navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia;

let localStream;
navigator.mediaDevices.getUserMedia({ video: true, audio: true })
    .then(stream => {
        localStream = stream;
        const localVideoElement = document.getElementById('local-video');
        localVideoElement.srcObject = localStream;
    })
    .catch(error => {
        console.error('Erro ao capturar mídia.', error);
    });
if (localStream != null) {
    localStream.getTracks().forEach((track) => {
        peerConnection.addTrack(track, localStream);
    });
}

const offer = peerConnection.createOffer();
peerConnection.setLocalDescription(offer);

// Enviar a oferta para o outro par através do servidor de sinalização
connection.invoke("SendSignal",
    JSON.stringify({ 'offer': offer }), targetPeerId);

connection.on("ReceiveSignal", async (message, senderId) => {
    const signal = JSON.parse(message);

    if (signal['ice-candidate']) {
        const iceCandidate = new RTCIceCandidate(signal['ice-candidate']);
        peerConnection.addIceCandidate(iceCandidate);
    } else if (signal.offer) {
        const remoteOffer = new RTCSessionDescription(signal.offer);
        peerConnection.setRemoteDescription(remoteOffer);

        const answer = await peerConnection.createAnswer();
        peerConnection.setLocalDescription(answer);

        // Enviar a resposta para o outro par através do servidor de sinalização
        connection.invoke("SendSignal",
            JSON.stringify({ 'answer': answer }), senderId);
    } else if (signal.answer) {
        const remoteAnswer = new RTCSessionDescription(signal.answer);
        peerConnection.setRemoteDescription(remoteAnswer);
    }
});