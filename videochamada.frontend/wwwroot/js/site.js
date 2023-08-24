
// Controle de Mascara de Telefone
// ------------------------------------------------------------------------------
const $input = document.querySelector('[data-mask="phone"]');
if ($input)
    $input.addEventListener('input', handleInput, false);
function handleInput (e) {
    e.target.value = phoneMask(e.target.value)
}
function phoneMask (phone) {
    return phone.replace(/\D/g, '')
        .replace(/^(\d)/, '($1')
        .replace(/^(\(\d{2})(\d)/, '$1) $2').replace(/(\d{5})(\d{1,5})/, '$1-$2')
        .replace(/(-\d{4})\d+?$/, '$1');
}
// ------------------------------------------------------------------------------