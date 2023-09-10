
// Controle de Mascara de Telefone
// ------------------------------------------------------------------------------
const $inputPhone = document.querySelector('[data-mask="phone"]');
if ($inputPhone) $inputPhone.addEventListener('input', handleInputPhone, false);
function handleInputPhone (e) {
    e.target.value = phoneMask(e.target.value)
}
function phoneMask (phone) {
    return phone.replace(/\D/g, '')
        .replace(/^(\d)/, '($1')
        .replace(/^(\(\d{2})(\d)/, '$1) $2')
        .replace(/(\d{5})(\d{1,5})/, '$1-$2')
        .replace(/(-\d{4})\d+?$/, '$1');
}
// ------------------------------------------------------------------------------


// Controle de Mascara de Data 'dd/mm/yyyy'
// ------------------------------------------------------------------------------
const $inputDate = document.querySelector('[data-mask="date"]');
if ($inputDate) $inputDate.addEventListener('input', handleInputDate, false);
function handleInputDate (e) {
    e.target.value = dateMask(e.target.value)
}
function dateMask (date) {
    return date.replace(/\D/g, '')
        .replace(/^(\d{2})/, '$1/')
        .replace(/^(\d{2}\/\d{2})/, '$1/')
        .replace(/(\/\d{4})\d+?$/, '$1');
}
// ------------------------------------------------------------------------------