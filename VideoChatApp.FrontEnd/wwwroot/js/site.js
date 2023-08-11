let localStream;

navigator.mediaDevices.getUserMedia({ video: true, audio: true })
    .then(stream => {
        localStream = stream;
        // Adicione o stream à sua video tag local.
        const localVideoElement = document.getElementById('local-video');
        localVideoElement.srcObject = localStream;
    })
    .catch(error => {
        console.error('Erro ao capturar mídia:', error);
    });

const connection = new signalR.HubConnectionBuilder().withUrl("/signalHub").build();
const peer = new Peer(undefined, { debug: 2 });

peer.on('open', id => {
    // Conecte-se ao servidor SignalR e envie o ID do peer ao conectar-se.
    connection.start().then(() => {
        connection.invoke("RegisterPeer", id);
    });
});

// Vamos considerar que as seguintes variáveis já foram definidas no topo do seu JS:
const configuration = { iceServers: [{ urls: 'stun:stun.l.google.com:19302' }] };
let peerConnection  = new RTCPeerConnection(configuration);

/*
// Adicione o stream local à conexão.
localStream.getTracks().forEach(track => {
    peerConnection.addTrack(track, localStream);
});

peerConnection.createOffer()
    .then(offer => peerConnection.setLocalDescription(offer))
    .then(() => {
        connection.invoke("SendOffer", targetPeerId, peerConnection.localDescription);
    });

connection.on("ReceiveOffer", (peerId, offer) => {
    const remoteOffer = new RTCSessionDescription(offer);
    peerConnection.setRemoteDescription(remoteOffer).then(() => {
        return peerConnection.createAnswer();
    }).then(answer => {
        return peerConnection.setLocalDescription(answer);
    }).then(() => {
        connection.invoke("SendAnswer", peerId, peerConnection.localDescription);
    });
});

connection.on("ReceiveAnswer", (peerId, answer) => {
    const remoteAnswer = new RTCSessionDescription(answer);
    peerConnection.setRemoteDescription(remoteAnswer);
});

peerConnection.onicecandidate = event => {
    if (event.candidate) {
        connection.invoke("SendIceCandidate", targetPeerId, event.candidate);
    }
};

connection.on("ReceiveIceCandidate", (peerId, iceCandidate) => {
    peerConnection.addIceCandidate(new RTCIceCandidate(iceCandidate));
});

peerConnection.ontrack = event => {
    const remoteVideoElement = document.getElementById('remote-video');
    if (!remoteVideoElement.srcObject && event.streams[0]) {
        remoteVideoElement.srcObject = event.streams[0];
    }
};
*/

// Função para iniciar chamada
function startCall() {
    
    // Obter o ID do usuário remoto do input
    let targetPeerId = document.getElementById('remote-peer-id').value;
    if (!targetPeerId) {
        alert("Por favor, insira o ID do usuário remoto.");
        return;
    }
    
    // Supondo que já tenha algum código para configurar a RTCPeerConnection
    peerConnection.createOffer()
        .then(offer => peerConnection.setLocalDescription(offer))
        .then(() => {
            // Suponha que 'connection' é sua conexão SignalR
            connection.invoke("SendOffer", targetPeerId, peerConnection.localDescription);
        });
}

// Função para terminar chamada
function endCall() {
    if (peerConnection) {
        peerConnection.close();
        peerConnection = null;
    }

    // Parar todos os tracks e limpar o video local
    if (localStream) {
        localStream.getTracks().forEach(track => track.stop());
        document.getElementById('local-video').srcObject = null;
    }
}

// Função para ajustar qualidade do vídeo
function setVideoQuality(quality) {
    let constraints;

    switch (quality) {
        case 'low':
            constraints = { video: { width: { max: 320 }, height: { max: 240 }, frameRate: 15 }, audio: true };
            break;
        case 'medium':
            constraints = { video: { width: { max: 640 }, height: { max: 480 }, frameRate: 30 }, audio: true };
            break;
        case 'high':
            constraints = { video: { width: { max: 1280 }, height: { max: 720 }, frameRate: 30 }, audio: true };
            break;
    }

    // Re-obter mídia e re-iniciar chamada se necessário
    navigator.mediaDevices.getUserMedia(constraints).then(stream => {
        localStream = stream;
        document.getElementById('local-video').srcObject = localStream;
        // Se estivermos em uma chamada, reinicie-a com a nova qualidade
        if (peerConnection) {
            startCall();
        }
    }).catch(error => {
        console.error('Erro ao ajustar qualidade:', error);
    });
}

// Adicionando event listeners para os controles
document.getElementById('start-call').addEventListener('click', startCall);
document.getElementById('end-call').addEventListener('click', endCall);
document.getElementById('video-quality').addEventListener('change', (event) => {
    setVideoQuality(event.target.value);
});
