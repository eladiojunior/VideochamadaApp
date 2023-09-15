const avisarApartirPosicao = 3; //A partir da posição 3, avisar o cliente com o um alerta! 
FilaAtendimento = {
    InitAtualizacaoPublicidade: function () {
        //Atualizar as publicidades da página
        var carouselPublicidade = document.querySelector('#carouselPublicidade');
        var carousel = new bootstrap.Carousel(carouselPublicidade, {
            interval: 10000,
            wrap: false
        });
    },
    InitVerificacaoFila: function () {
        //Verificar a fila de atendimento e profissionais Online
        window.setInterval(function () {
            FilaAtendimento.VerificarFilaAtendimento();
        }, 5000);
    },
    VerificarFilaAtendimento: function () {
        $.ajax({
            cache: false,
            type: "GET",
            url: _contexto + "Atendimento/PosicaoFilaAtendimento",
            dataType: "json",
            success: function (result) {
                if (!result.hasErro) {
                    var posicaoAtualFila = parseInt($(".text-posicao-fila").text());
                    if (!posicaoAtualFila) posicaoAtualFila = 0;
                    var posicaoNovaFila = parseInt(result.model.posicaoNaFila);
                    $(".text-posicao-fila").text(posicaoNovaFila);
                    $(".text-qtd-profissionais").text(result.model.qtdProfissionaisOnline);
                    if (posicaoAtualFila!=0 && posicaoAtualFila!=posicaoNovaFila) {
                        FilaAtendimento.AvisarAtendimentoProximo(posicaoNovaFila);
                        if (posicaoNovaFila === 0) //Atender
                            FilaAtendimento.RedirecionarParaAtendimento();
                    }
                }
            }, 
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("VerificarFilaAtendimento: " + errorThrown);
            }
        });
    },
    RedirecionarParaAtendimento: function () {
        //Redirecionar cliente para o atendimento...
        var modalRedirect = document.getElementById('modalRedirect')
        modalRedirect.addEventListener('show.bs.modal', function (event) {
            window.setTimeout(function () {
                window.location.href = _contexto + "Atendimento/EmAtendimento";
            }, 5000);
        });
        modalRedirect.show();
    },
    AvisarAtendimentoProximo: function (posicaoFila) {
        if (posicaoFila > avisarApartirPosicao)
            return;
        const promise = document.getElementById('audioPosicaoFila').play();
        if (promise !== undefined) {
            promise.then(_ => {
                console.log("Avisar o cliente que está chegando sua vez na fila.");
            }).catch(error => {
                console.error("Erro ao avisar cliente da fila: " + error);
            });
        }
    }
}
$(function () {
    FilaAtendimento.InitAtualizacaoPublicidade();
    FilaAtendimento.InitVerificacaoFila();
});

