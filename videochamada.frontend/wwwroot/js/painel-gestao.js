AreaPainelGestao = {
    InitVerificacaoDashboard: function () {
        //Verificar as informações do Painel de Gestão
        window.setInterval(function () {
            AreaPainelGestao.VerificarInformacoesPainelGestao();
        }, 5000);
    },
    VerificarInformacoesPainelGestao: function () {
        $.ajax({
            cache: false,
            type: "GET",
            url: _contexto + "PainelGestao/InfoPainelGestao",
            dataType: "json",
            success: function (result) {
                if (result.hasErro) {
                    console.error(result.mensagem);
                    return;
                }
                AreaPainelGestao.AtualizarPainelGestao(result.model);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("VerificarFilaAtendimento: " + errorThrown);
            }
        });  
    },
    AtualizarPainelGestao: function (model) {
        $(".ultima-atualizacao").text(model.dataHoraAtualizacaoPainelString);
        $(".qtd-clientes-registrados").text(model.qtdClientesRegistrados);
        $(".qtd-clientes-fila-atendimento").text(model.qtdClientesFilaAtendimento);
        $(".qtd-clientes-em-atendimento").text(model.qtdClientesEmAtendimento);
        $(".qtd-profissional-logados").text(model.qtdProfissionaisLogados);
        $(".qtd-profissional-online").text(model.qtdProfissionaisOnline);
        $(".qtd-profissional-em-atendimento").text(model.qtdProfissionaisEmAtendimento);
        $(".datahora-atendimentos").text(model.dataHoraAtendimentosString1);
        $(".datahora-atendimentos-sm").text(model.dataHoraAtendimentosString2);
        $(".qtd-atendimentos-cancelados").text(model.qtdAtendimentosCancelados);
        $(".qtd-atendimentos-desistencias").text(model.qtdAtendimentosDesistencias);
        $(".qtd-atendimentos-finalizados").text(model.qtdAtendimentosFinalizados);
        $(".tempo-medio-atendimentos").text(model.tempoMedioAtendimentos);
        $(".tempo-medio-nafila").text(model.tempoMedioNaFilaAtendimento);
        $(".avaliacao-media-atendimentos").text(model.avaliacaoMediaAtendimentos);
    },
}
$(function () {
    AreaPainelGestao.InitVerificacaoDashboard();
});