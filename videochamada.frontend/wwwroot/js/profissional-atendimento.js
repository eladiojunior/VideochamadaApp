let timeVerificarCliente;
AreaAtendimentoProfissional = {
    InitControleAtendimento: function () {
        $("#btnOnline").click(function () {
            const labalCheck = $(".btnOnline");
            labalCheck.removeClass("btn-outline-secondary");
            labalCheck.removeClass("btn-outline-success");
            const isChecked = $(this).prop('checked');
            AreaAtendimentoProfissional
                .AtualizarSituacaoAtendimentoProfissional(isChecked, function (hasErro, mensagemErro) {
                if (hasErro) {
                    console.error(mensagemErro);
                    return;
                }
                if (isChecked) {
                    labalCheck.addClass("btn-outline-success");
                    labalCheck.text("Atendendo (Online)");
                    AreaAtendimentoProfissional.InitVerificacaoProximoClienteAtendimento();
                    console.log("Profissional de Saúde está atendendo (Online).");
                } else {
                    labalCheck.addClass("btn-outline-secondary");
                    labalCheck.text("Não estou atendendo");
                    console.log("Profissional de Saúde NÃO está atendendo (Offline).");
                    clearTimeout(timeVerificarCliente);
                }
            });
        });
    },
    InitVerificacaoDashboard: function () {
        //Verificar a fila de atendimento e profissionais Online
        window.setInterval(function () {
            AreaAtendimentoProfissional.VerificarFilaAtendimento();
        }, 5000);
    },
    InitVerificacaoProximoClienteAtendimento: function () {
        //Time para verificação do próximo cliente da fila de atendimento.
        timeVerificarCliente = window.setInterval(function () {
            AreaAtendimentoProfissional.VerificacaoProximoClienteAtendimento();
        }, 5000);
    },
    VerificacaoProximoClienteAtendimento: function () {
        let idProfissional = $("#idProfissional").val();
        if (!idProfissional) return;
        
        console.log("Para a verificação da fila até o retorno.");
        clearTimeout(timeVerificarCliente);
        
        $.ajax({
            cache: false,
            type: "GET",
            url: _contexto + "Atendimento/RecuperarProximoClienteFilaAtendimento",
            data: { idProfissional: idProfissional },
            dataType: "json",
            success: function (result) {
                if (result.hasErro) {
                    console.log(result.erros);
                    AreaAtendimentoProfissional.InitVerificacaoProximoClienteAtendimento();
                    return;
                }
                AreaAtendimentoProfissional.AvisarAtendimentoDeCliente();
                alert("IdAtendimento: " + result.model.id);
                AreaAtendimentoProfissional.RedirecionarParaAtendimento(result.model.id);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("VerificacaoProximoClienteAtendimento: " + errorThrown);
            }
        });
        
        console.log("Verificar próximo cliente...");
        
    },
    RedirecionarParaAtendimento: function (idAtendimento) {
        //Abrir modal com aguarde!
        const modalRedirect = new bootstrap.Modal(document.getElementById('modalRedirect'), {});
        modalRedirect.show();
        //Redirecionar proficional para o atendimento do cliente...
        window.setTimeout(function () {
            window.open(_contexto + "EquipeSaude/EmAtendimento?idAtendimento="+idAtendimento, "_blank");
            modalRedirect.hide();
        }, 5000);
    },
    AvisarAtendimentoDeCliente: function () {
        const promise = document.getElementById('audioAtendimento').play();
        if (promise !== undefined) {
            promise.then(_ => {
                console.log("Avisar ao profissional que que chegou cliente para atender.");
            }).catch(error => {
                console.error("Erro ao avisar profissional do atendimento: " + error);
            });
        }
    },
    AtualizarSituacaoAtendimentoProfissional: function (hasOnline, callback_result) {
        $.ajax({
            cache: false,
            type: "POST",
            url: _contexto + "EquipeSaude/AtualizarSituacaoAtendimentoProfissional",
            data: { hasSituacaoAtendimento: hasOnline },
            dataType: "json",
            success: function (result) {
                if (!result.hasErro) {
                    AreaAtendimentoProfissional.AtualizarDastboardAtendimentos(
                        result.model.qtdProfissionaisOnline, result.model.qtdClientesFila, result.model.qtdClientesEmAtendimento);
                }
                callback_result(result.hasErro, result.erros);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                callback_result(true, errorThrown);
            }
        });
    },
    InitControleTabsAtendimento: function () {
        const tabs = $('#tabsAtendimentos button');
        tabs.click(function () {
            const keyTab = $(this).data("bs-target");
            if (keyTab === '#emAndamento')
                AreaAtendimentoProfissional.CarregarAtendimentosProfissional(false);
            else if (keyTab === '#realizados')
                AreaAtendimentoProfissional.CarregarAtendimentosProfissional(true);
        });  
    },
    CarregarAtendimentosProfissional: function (hasRealizados) {
        $.ajax({
            cache: false,
            type: "GET",
            url: _contexto + "EquipeSaude/CarregarAtendimentosProfissional",
            dataType: "json",
            data: {
                hasAtendimentosRealizados: hasRealizados
            },
            success: function (result) {
                if (!result.hasErro) {
                    if (hasRealizados)
                        $(".atendimentos-realizados").html(result.model);
                    else
                        $(".atendimentos-emAndamento").html(result.model);
                }
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("CarregarAtendimentosProfissional: " + errorThrown);
            }
        });
    },
    VerificarFilaAtendimento: function () {
        $.ajax({
            cache: false,
            type: "GET",
            url: _contexto + "EquipeSaude/SituacaoFilaAtendimento",
            dataType: "json",
            success: function (result) {
                if (!result.hasErro) {
                    AreaAtendimentoProfissional.AtualizarDastboardAtendimentos(
                        result.model.qtdProfissionaisOnline, result.model.qtdClientesFila, result.model.qtdClientesEmAtendimento);
                }
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("VerificarFilaAtendimento: " + errorThrown);
            }
        });  
    },
    AtualizarDastboardAtendimentos: function (qtdProfissionaisOnline, qtdClienteFilaAtendimento, qtdClienteEmAtendimento) {
        $(".qtd-profissionais-online").text(qtdProfissionaisOnline);
        $(".qtd-clientes-filaatendimento").text(qtdClienteFilaAtendimento);
        $(".qtd-clientes-ematendimento").text(qtdClienteEmAtendimento);
    },
}
$(function () {
    AreaAtendimentoProfissional.InitControleAtendimento();
    AreaAtendimentoProfissional.InitVerificacaoDashboard();
    AreaAtendimentoProfissional.InitControleTabsAtendimento();
});