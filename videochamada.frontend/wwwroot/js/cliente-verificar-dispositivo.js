var streamMedia = null;
var timeVerificacao = null;
var testantoMicrofone = false;
var testantoAudio = false;
ativarVerificacaoDispositivo();
function ativarVerificacaoDispositivo() {
    $(".icon-verificacao").show();
    $(".icon-off").hide();
    $(".icon-on").hide();
    $(".erro-dispositivo").hide();
    $(".area-microfone-teste").hide();
    $(".area-mensagem-teste").hide();
    timeVerificacao = setTimeout(verificacaoMediaPermissions, 3000);
}
function verificacaoMediaPermissions() {
    navigator.mediaDevices.getUserMedia({ video: true, audio: true })
        .then(stream => {
            streamMedia = stream;
            sucessoPermissaoDispositivos();
        })
        .catch(error => {
            erroPermissaoDispositivos();
        });
    clearTimeout(timeVerificacao);
}
function sucessoPermissaoDispositivos() {
    $(".icon-verificacao").hide();
    $(".icon-off").hide();
    $(".icon-on").show();
    $(".erro-dispositivo").hide();
    resetMensagemTesteDispositivo();
    $(".area-mensagem-teste").show();
}
function erroPermissaoDispositivos() {
    $(".icon-verificacao").hide();
    $(".icon-on").hide();
    $(".icon-off").show();
    $(".erro-dispositivo").show();
    resetMensagemTesteDispositivo();
    $(".area-mensagem-teste").show();
}
$(".button-verificacao").click(function () {
    ativarVerificacaoDispositivo();
});
function resetMensagemTesteDispositivo() {
    $(".mensagem-teste").removeClass("bg-primary");
    $(".mensagem-teste").removeClass("bg-danger");
    $(".mensagem-teste").addClass("bg-secondary");
    $(".mensagem-teste").text("Clique no ícone para testar o dispositivo...");
}
function mensagemTesteDispositivo(mensagem) {
    $(".mensagem-teste").removeClass("bg-secondary");
    $(".mensagem-teste").removeClass("bg-danger");
    $(".mensagem-teste").addClass("bg-primary");
    $(".mensagem-teste").text(mensagem);
}
function erroTesteDispositivo(mensagem) {
    $(".mensagem-teste").removeClass("bg-secondary");
    $(".mensagem-teste").addClass("bg-danger");
    $(".mensagem-teste").text(mensagem);
    let timeTesteDispositivo = setTimeout(function () {
        resetMensagemTesteDispositivo();
        $(".mensagem-teste").removeClass("bg-danger");
        $(".mensagem-teste").addClass("bg-secondary");
        clearTimeout(timeTesteDispositivo);
    }, 5000);
}
$(".teste-camera").click(function () {
    const videoElement = document.getElementById('localVideo');
    const modelTesteVideo = document.getElementById('modelTesteVideo');
    modelTesteVideo.addEventListener('hidden.bs.modal', function (event) {
        videoElement.srcObject = null;
    })
    try {
        if (streamMedia == null) {
            erroTesteDispositivo("Câmera sem permissão, inexistente ou utilizado por outro aplicativo.");
            return;
        }
        videoElement.srcObject = streamMedia;
        bootstrap.Modal.getOrCreateInstance(modelTesteVideo).show();
        $(".camera.icon-off").hide();
        $(".camera.icon-on").show();
    } catch (e) {
        erroTesteDispositivo("Câmera sem permissão, inexistente ou utilizado por outro aplicativo.");
        console.log('Erro no teste da câmera: ' + e);
    }
});
$(".teste-microfone").click(function () {
    try {
        if (streamMedia == null) {
            erroTesteDispositivo("Microfone sem permissão, inexistente ou desabilitado.");
            return;
        }
        if (testantoMicrofone===false) {
            mensagemTesteDispositivo("Microfone em teste, pode falar algo... clique para fechar.");
            $(".area-microfone-teste").show();
            const audioContext = new AudioContext();
            const analyser = audioContext.createAnalyser();
            const microphone = audioContext.createMediaStreamSource(streamMedia);
            const javascriptNode = audioContext.createScriptProcessor(2048, 1, 1);
            analyser.smoothingTimeConstant = 0.8;
            analyser.fftSize = 1024;
            microphone.connect(analyser);
            analyser.connect(javascriptNode);
            javascriptNode.connect(audioContext.destination);
            javascriptNode.onaudioprocess = function() {
                const array = new Uint8Array(analyser.frequencyBinCount);
                analyser.getByteFrequencyData(array);
                let values = 0;
                const length = array.length;
                for (let i = 0; i < length; i++)
                    values += array[i];
                const average = values / length;
                var element = document.getElementById("progressMicrofone");
                var height = 0;
                if (average > 0) {
                    height = parseInt((average * 1.45), 10);
                    element.style.height = height+'px';
                }
            };
            testantoMicrofone=true;
        } else {
            resetMensagemTesteDispositivo();
            $(".area-microfone-teste").hide();
            const audioContext = new AudioContext();
            const javascriptNode = audioContext.createScriptProcessor(2048, 1, 1);
            javascriptNode.onaudioprocess = null;
            testantoMicrofone=false;
        }
        $(".microfone.icon-off").hide();
        $(".microfone.icon-on").show();
    } catch (e) {
        testantoMicrofone=false;
        erroTesteDispositivo("Microfone sem permissão, inexistente ou desativado.");
        $(".area-microfone-teste").hide();
        console.log('Erro no teste do Microfone: ' + e);
    }
});
$(".teste-audio").click(function () {
    try {
        if (testantoAudio===true)
            return;
        testantoAudio=true;
        mensagemTesteDispositivo("Áudio em teste, escute a mensagem...");
        const audioElement = document.getElementById('localAudio');
        audioElement.play();
        let timeTesteAudio = setTimeout(function () {
            $(".audio.icon-off").hide();
            $(".audio.icon-on").show();
            testantoAudio = false;
            resetMensagemTesteDispositivo();
            clearTimeout(timeTesteAudio);
        }, 8000);
    } catch (e) {
        testantoAudio=false;
        erroTesteDispositivo("Erro no dispositivo de áudio.");
        console.log('Erro no teste do áudio: ' + e);
    }
});