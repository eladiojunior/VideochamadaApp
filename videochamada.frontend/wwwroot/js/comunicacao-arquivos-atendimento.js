ArquivoAtendimento = {
    InitControles: function () {
        
        $(".button-init-enviar-arquivo").click(function () {
            ArquivoAtendimento.ModelUploadArquivo(true);
        });
        $(".button-enviar-arquivo").click(function () {
            ArquivoAtendimento.EnviarArquivo();
        });
        ArquivoAtendimento.InitRemoverArquivo();

        //Receber arquivo recebido
        connection.on('ReceberArquivo', (idUsuario, nomeArquivo) => {
            ComunicacaoUsuarios.MensagemUsuario("Novo arquivo recebido ["+nomeArquivo+"]... verifique.");
        });
        
    },
    InitRemoverArquivo: function () {
        $(".remover-arquivo-atendimento").click(function () {
            let idAtendimento = $("#idAtendimento").val();
            let idArquivo = $(this).data("id-arquivo");
            ArquivoAtendimento.RemoverArquivo(idAtendimento, idArquivo);
        });        
    },
    ModelUploadArquivo: function (has_controle) {
        const modalEnviarArquivoAtendimento = bootstrap.Modal.getOrCreateInstance(document.getElementById('modalEnviarArquivoAtendimento'));
        if (has_controle) {
            ArquivoAtendimento.ResetMensagemUsuario();
            $("#formFile").val('');
            $(".button-enviar-arquivo").prop('disabled', false);
            modalEnviarArquivoAtendimento.show();
        } else {
            $(".button-enviar-arquivo").prop('disabled', true);
            modalEnviarArquivoAtendimento.hide();
        }
    },
    EnviarArquivo: function () {
        
        var objFileInput = $("#formFile");
        if (ArquivoAtendimento.VerificarArquivo(objFileInput,
            function (msg) {
                ArquivoAtendimento.MensagemUsuario(msg, true);
            })) 
        {//Arquivo validado... enviar para o servidor...
            
            ArquivoAtendimento.ResetMensagemUsuario();
            
            var idUsuario = $("#idUsuario").val();
            var idAtendimento = $("#idAtendimento").val();
            
            ArquivoAtendimento.AreaProgressUpload(true);
            
            const formData = new FormData();
            formData.append("idUsuario", idUsuario);
            formData.append("idAtendimento", idAtendimento);
            formData.append("arquivo", objFileInput[0].files[0]);
            $.ajax({
                async: true,
                type: "POST",
                url: _contexto + "Atendimento/EnviarArquivosAtendimento",
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
                timeout: 60000,
                success: function (result) {
                    if (result.hasErro) {
                        ArquivoAtendimento.MensagemUsuario(result.erros[0], true);
                        ArquivoAtendimento.AreaProgressUpload(false);
                        return;
                    }
                    ArquivoAtendimento.AreaProgressUpload(false);
                    ArquivoAtendimento.ModelUploadArquivo(false);
                    connection.invoke("EnviarArquivo", result.model.nomeOriginal);
                    ArquivoAtendimento.AtualizarListaArquivos();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.error("EnviarArquivoAtendimento: " + errorThrown);
                },
                xhr: function () {
                    const filexhr = $.ajaxSettings.xhr();
                    if (filexhr.upload) {
                        filexhr.upload.addEventListener('progress', ArquivoAtendimento.ProgressUpload, false);
                    }
                    return filexhr;
                },
            });
        }
    },
    AreaProgressUpload: function (has_controle) {
        const area_upload = $(".area-upload-arquivo");
        const area_progress_upload = $(".area-progress-upload-arquivo");
        if (has_controle) {
            area_upload.hide();
            area_progress_upload.find(".progress-bar").css("width", "0%");
            area_progress_upload.find(".progress-bar").text("0%");
            area_progress_upload.show();
        } else {
            area_upload.show();
            area_progress_upload.hide();
        }
    },
    ProgressUpload: function (event) {
        let percent = 0;
        let position = event.loaded || event.position;
        let total = event.total;
        if (event.lengthComputable) {
            percent = Math.ceil(position / total * 100);
        }
        const area_progress_upload = $(".area-progress-upload-arquivo");
        area_progress_upload.find(".progress-bar").css("width", percent + "%");
        area_progress_upload.find(".progress-bar").text(percent + "%");
    },
    VerificarArquivo: function (fileInput, callback_erro) {
        
        if (!fileInput)
            return false;
        
        if (fileInput[0].files.length === 0) {
            callback_erro(`Nenhum arquivo informado, selecione um arquivo conforme regras`);
            return false;
        }
        
        const file = fileInput[0].files[0]; //Pegar o primeiro... se precisar de mais mude a regra...
        const fileSize = file.size / 1024 / 1024; // Tamanho do arquivo em MB
        const fileType = file.type; // Tipo de arquivo (exemplo: 'image/png')
        //Verificar o tamanho do arquivo (menor que 2 MB)
        if (fileSize > 2) {
            callback_erro(`Arquivo ${file.name} é maior que o permitido, selecione um arquivo com tamanho máximo de 2MB.`);
            return false;
        }
        // Verificar o tipo de arquivo (permitir apenas imagens e PDFs, por exemplo)
        if (!['image/jpeg', 'image/png', 'image/jpg', 'image/gif', 'application/pdf'].includes(fileType)) {
            callback_erro(`Arquivo ${file.name} com tipo não permitido, selecione um arquivo do tipo imagem (jpg/jpeg, gif ou png) e Documento (pdf).`);
            return false;
        }

        return true;
        
    },
    RemoverArquivo: function (idAtendimento, idArquivo) {
        $.ajax({
            cache: false,
            type: "POST",
            url: _contexto + "Atendimento/RemoverArquivoAtendimento",
            data: {
                'idAtendimento':idAtendimento, 
                'idArquivo': idArquivo
            },
            success: function (result) {
                if (result.hasErro) {
                    ComunicacaoUsuarios.MensagemUsuario(result.erros[0]);
                    return;
                }
                ComunicacaoUsuarios.MensagemUsuario(result.mensagem);
                ArquivoAtendimento.AtualizarListaArquivos();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("RemoverArquivo: " + errorThrown);
            }
        });
    },
    AtualizarListaArquivos: function () {
        var idAtendimento = $("#idAtendimento").val();
        console.log("Atualizar a lista de arquivos..." + idAtendimento);
        $.ajax({
            cache: false,
            type: "GET",
            url: _contexto + "Atendimento/ListarArquivosAtendimento",
            data: {'idAtendimento': idAtendimento},
            success: function (result) {
                if (result.hasErro) {
                    ComunicacaoUsuarios.MensagemUsuario(result.erros[0]);
                    return;
                }
                $(".lista-arquivos-atendimento").html(result.model);
                ArquivoAtendimento.InitRemoverArquivo();
                ComunicacaoUsuarios.MensagemUsuario(result.mensagem);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.error("AtualizarListaArquivos: " + errorThrown);
            }
        });
    },
    ResetMensagemUsuario: function () {
        var obj_mensagem = $(".modal-mensagem-arquivo");
        if (obj_mensagem === undefined)
            return;
        obj_mensagem.hide();
        obj_mensagem.html("");
    },
    MensagemUsuario: function (mensagem, hasErro) {
        var obj_mensagem = $(".modal-mensagem-arquivo");
        if (obj_mensagem === undefined || mensagem === undefined || mensagem === "") 
            return;
        if (hasErro === undefined) hasErro = false;
        obj_mensagem.addClass(hasErro?"alert-danger":"alert-info");
        obj_mensagem.html(mensagem);
        obj_mensagem.show();
    }
    
}
$(function () {
    ArquivoAtendimento.InitControles();
});

