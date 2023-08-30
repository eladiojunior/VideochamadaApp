let nivelSuperForte = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[$*&@#])(?:([0-9a-zA-Z$*&@#])(?!\1)){8,}$/;
let nivelForte = /(?=^.{8,}$)((?=.*\d)(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$/;
let nivelMedia = /(?=^.{8,}$)((?=.*\d)(?=.*\W+))(?![.\n])(?=.*[a-z]).*$/;
let nivelSimples = /(?=^.{8,}$)(?=.*\d)(?![.\n])(?=.*[a-z]).*$/;

VerificarSenha = {
    InitVerificacaoSenha: function () {
        $("#SenhaAcesso").keyup(function () {
            var nivelSenha = $(".nivel-senha");
            nivelSenha.removeClass("bg-secondary");
            nivelSenha.removeClass("bg-success");
            nivelSenha.removeClass("bg-primary");
            nivelSenha.removeClass("bg-warning");
            nivelSenha.removeClass("bg-danger");
            nivelSenha.removeClass("bg-info");
            nivelSenha.removeClass("text-white");
            var senha = $(this).val();
            if (senha === '') {
                nivelSenha.addClass("bg-secondary");
                nivelSenha.addClass("text-white");
                nivelSenha.text("...");
                return;
            }
            
            if (nivelSuperForte.test(senha)) {
                nivelSenha.addClass("bg-success");
                nivelSenha.addClass("text-white");
                nivelSenha.text("Excelente, senha muito forte!");
                return;
            }
            if (nivelForte.test(senha)) {
                nivelSenha.addClass("bg-primary");
                nivelSenha.addClass("text-white");
                nivelSenha.text("Ótimo, senha forte.");
                return;
            }
            if (nivelMedia.test(senha)) {
                nivelSenha.addClass("bg-info");
                nivelSenha.text("Tá bom, mais poderia melhorar, senha média.");
                return;
            }
            if (nivelSimples.test(senha)) {
                nivelSenha.addClass("bg-warning");
                nivelSenha.text("Bem fraquinha, senha simples.");
                return;
            }
            //Nenhuma senha definida
            nivelSenha.addClass("bg-danger");
            nivelSenha.addClass("text-white");
            nivelSenha.text("Achei muito fraca essa senha... cuidado!");
        });
    }
}
$(function () {
    VerificarSenha.InitVerificacaoSenha();
});
        
/*
deve conter no mínimo 8 caracter com números, letras e caracteres especiais.
/^
  (?=.*\d)              // deve conter ao menos um dígito
  (?=.*[a-z])           // deve conter ao menos uma letra minúscula
  (?=.*[A-Z])           // deve conter ao menos uma letra maiúscula
  (?=.*[$*&@#])         // deve conter ao menos um caractere especial
  [0-9a-zA-Z$*&@#]{8,}  // deve conter ao menos 8 dos caracteres mencionados
$/

 */