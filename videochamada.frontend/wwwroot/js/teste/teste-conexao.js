
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/videochamadaHub")
    .withAutomaticReconnect()
    .build();

let idConnectionHub = "";
let idAtendimentoHub = "Atendimento-ABCDEFGHIJ";
let idClienteHub = "";
let idProfissionalHub = "";

ComunicacaoUsuarios = {
    
    InitComunicacao: function () {
        
        console.log("Inicializar comunicação remota entre usuários.");
        $(".button-enviar-mensagem").click(function () {
            var mensagem = $(".mensagem-texto").val();
            connection.invoke("EnviarMensagem", mensagem);
            $(".mensagem-texto").val("");
            $(".mensagem-texto").focus();
        });
        $(".button-enviar-arquivo").click(function () {
            var nome_arquivo = $(".nome-arquivo").val();
            connection.invoke("EnviarArquivo", nome_arquivo);
            $(".nome-arquivo").val("");
            $(".nome-arquivo").focus();
        });
        
        connection.start()
        .then(() => {
            ComunicacaoUsuario.AtivarComunicacaoUsuario();
            $(".idAtendimento").text(idAtendimentoHub);
            console.log("Conexão com o servidor de sinalização estabelecida.");
        })
        .catch((err) => {
            console.error(err.toString());
        });
        
        //Receber usuário conectado...
        connection.on("UsuarioConectado", (connectionId) => {
            idConnectionHub = connectionId;
            $(".idConnectionHub").text(idConnectionHub);
            console.log("Usuário conectado: " + idConnectionHub, idAtendimentoHub, idClienteHub);
        });
        
        //Identificar usuário desconectado...
        connection.on('UsuarioDesconectado', (connectionId) => {
            console.log("Usuário desconectado: " + connectionId);
            connection.close();
        });

        //Receber mensagem enviada
        connection.on('ReceberMensagem', (idUsuario, mensagem) => {
            console.log(idUsuario, " => ", mensagem);
            $(".mensagens-texto").append("<span class='d-block'>"+idUsuario+" ==> "+mensagem+"</span>");
        });

        //Receber arquivo recebido
        connection.on('ReceberArquivo', (idUsuario, nomeArquivo) => {
            console.log(idUsuario, " => ", nomeArquivo);
            $(".mensagens-texto").append("<span class='d-block text-danger'>"+idUsuario+" ==> "+nomeArquivo+"</span>");
        });

    },
    CarregarDispositivosAudio: function () {
        navigator.mediaDevices.enumerateDevices()
            .then(devices => {
                let audioDevices = devices.filter(device => device.kind === 'audioinput');
                audioDevices.forEach(device => {
                    let option = document.createElement('option');
                    option.value = device.deviceId;
                    option.text = device.label;
                    document.getElementById('audioDevice').appendChild(option);
                });
            })
            .catch(err => {
                console.log('Erro ao listar dispositivos:', err);
            });
    },
}
$(function () {
    ComunicacaoUsuarios.InitComunicacao();
    ComunicacaoUsuarios.CarregarDispositivosAudio();
});