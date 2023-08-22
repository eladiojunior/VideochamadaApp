var streamMedia = null;
var timeVerificacao = null;
var testantoCamera = false;
var testantoMicrofone = false;
var testantoAudio = false;
ativarVerificacaoDispositivo();
function ativarVerificacaoDispositivo() {
    $(".icon-verificacao").show();
    $(".icon-off").hide();
    $(".icon-on").hide();
    $(".buttons-test").hide();
    $(".erro-dispositivo").hide();
    timeVerificacao = setTimeout(verificacaoMediaPermissions, 3000);
}
function verificacaoMediaPermissions() {
    // Request media stream
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
    $(".buttons-test").show();
}
function erroPermissaoDispositivos() {
    $(".icon-verificacao").hide();
    $(".icon-on").hide();
    $(".icon-off").show();
    $(".erro-dispositivo").show();
    $(".buttons-test").show();
}
$(".button-verificacao").click(function () {
    ativarVerificacaoDispositivo();
});
$(".button-test-camara").click(function () {
    try {
        const videoElement = document.getElementById('localVideo');
        if (testantoCamera===false) {
            $(this).text("Parar Teste");
            $(".area-camera-teste").show();
            videoElement.srcObject = streamMedia;
            testantoCamera=true;
        } else {
            $(this).text("Testar Câmera");
            $(".area-camera-teste").hide();
            videoElement.srcObject = null;
            testantoCamera=false;
        }
        //Teste com sucesso...
        $(".camera .icon-off").hide();
        $(".camera .icon-on").show();
    } catch (e) {
        $(this).text("Testar Câmera");
        $(".area-camera-teste").hide();
        testantoCamera=false;
        console.log('Erro no teste da Câmera: ' + e);
    }
});
$(".button-test-microfone").click(function () {
    try {
        if (testantoMicrofone===false) {
            $(this).text("Parar Teste");
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
            $(this).text("Testar Microfone");
            $(".area-microfone-teste").hide();
            const audioContext = new AudioContext();
            const javascriptNode = audioContext.createScriptProcessor(2048, 1, 1);
            javascriptNode.onaudioprocess = null;
            testantoMicrofone=false;
        }
        $(".microfone .icon-off").hide();
        $(".microfone .icon-on").show();
    } catch (e) {
        testantoMicrofone=false;
        console.log('Erro no teste do Microfone: ' + e);
    }
});
$(".button-test-audio").click(function () {
    try {
        if (testantoAudio===true)
            return;
        testantoAudio=true;
        $(this).text("Testando...");
        const audioElement = document.getElementById('testAudio');
        audioElement.play();
        $(".audio .icon-off").hide();
        $(".audio .icon-on").show();
        testantoAudio=false;
        $(this).text("Testar Áudio");
    } catch (e) {
        testantoAudio=false;
        $(this).text("Testar Áudio");
        console.log('Erro no teste da Áudio: ' + e);
    }
});