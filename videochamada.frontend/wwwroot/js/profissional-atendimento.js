let timeVerificarCliente;
let hasVerificarProximoCliente = true;
AreaAtendimentoProfissional = {
    InitControleAtendimento: function () {
        $("#btnOnline").click(function () {
            const labalCheck = $(".btnOnline");
            labalCheck.removeClass("btn-outline-secondary");
            labalCheck.removeClass("btn-outline-success");
            const isChecked = $(this).prop('checked');
            AreaAtendimentoProfissional
                .AtualizarSituacaoAtendimentoProfissional(isChecked, 
                    function (hasErro, mensagemErro) {
                    if (hasErro) {
                        console.error(mensagemErro);
                        return;
                    }
                    if (isChecked) {
                        labalCheck.addClass("btn-outline-success");
                        labalCheck.text("Atendendo (Online)");
                        AreaAtendimentoProfissional.ControleVerificacaoProximoClienteAtendimetno(true);
                        console.log("Profissional de Saúde está atendendo (Online).");
                    } else {
                        labalCheck.addClass("btn-outline-secondary");
                        labalCheck.text("Não estou atendendo");
                        console.log("Profissional de Saúde NÃO está atendendo (Offline).");
                        AreaAtendimentoProfissional.ControleVerificacaoProximoClienteAtendimetno(false);
                    }
                });
        });
        AreaAtendimentoProfissional.InitVerificacaoProximoClienteAtendimento();
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
            console.log("Verificar próximo cliente da fila...");
            AreaAtendimentoProfissional.VerificacaoProximoClienteAtendimento();
        }, 5000);
    },
    ControleVerificacaoProximoClienteAtendimetno: function (hasVerificar) {
        hasVerificarProximoCliente = hasVerificar;
    },
    VerificacaoProximoClienteAtendimento: function () {
        
        let idProfissional = $("#idProfissional").val();
        if (!idProfissional) 
            return;
        if (!hasVerificarProximoCliente)
            return;
        
        console.log("Para a verificação da fila até o retorno.");
        AreaAtendimentoProfissional.ControleVerificacaoProximoClienteAtendimetno(false);
        
        $.ajax({
            cache: false,
            type: "GET",
            url: _contexto + "Atendimento/RecuperarProximoClienteFilaAtendimento",
            data: { idProfissional: idProfissional },
            dataType: "json",
            success: function (result) {
                if (!result.hasErro) {
                    AreaAtendimentoProfissional.AvisarAtendimentoDeCliente();
                    AreaAtendimentoProfissional.RedirecionarParaAtendimento(result.model.id);
                    return;
                }
                AreaAtendimentoProfissional.ControleVerificacaoProximoClienteAtendimetno(true);    
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("VerificacaoProximoClienteAtendimento: " + errorThrown);
                AreaAtendimentoProfissional.ControleVerificacaoProximoClienteAtendimetno(true);
            }
        });
    },
    RetomarVerificacaoProximoCliente: function () {
        console.log("Retomar verificação de próximo cliente da fila.");
        AreaAtendimentoProfissional.ControleVerificacaoProximoClienteAtendimetno(true);
        const keyTab = $(this).data("bs-target");
        if (keyTab === '#emAndamento')
            AreaAtendimentoProfissional.CarregarAtendimentosProfissional(false);
        else if (keyTab === '#realizados')
            AreaAtendimentoProfissional.CarregarAtendimentosProfissional(true);
    },
    RedirecionarParaAtendimento: function (idAtendimento) {
        //Abrir modal com aguarde!
        AreaAtendimentoProfissional.ControleVerificacaoProximoClienteAtendimetno(false);
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
                    {
                        $(".atendimentos-realizados").html(result.model);
                        AreaAtendimentoProfissional.InitAcoesListaAtendimentoRealizado();
                    }
                    else 
                    {
                        $(".atendimentos-emAndamento").html(result.model);
                        AreaAtendimentoProfissional.InitAcoesListaAtendimentoEmAndamento();
                    }
                }
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("CarregarAtendimentosProfissional: " + errorThrown);
            }
        });
    },
    InitAcoesListaAtendimentoEmAndamento: function () {
        
    },
    InitAcoesListaAtendimentoRealizado: function () {

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
    }
}
$(function () {
    AreaAtendimentoProfissional.InitControleAtendimento();
    AreaAtendimentoProfissional.InitVerificacaoDashboard();
    AreaAtendimentoProfissional.InitControleTabsAtendimento();
});