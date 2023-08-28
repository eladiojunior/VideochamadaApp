if (!navigator.geolocation) {
    console.error("Navegador (browser) não suporte a recuperação de geolocalização.");
} else {
    navigator.geolocation.getCurrentPosition(success, error);
}
function success(position) {
    const latitude = position.coords.latitude;
    const longitude = position.coords.longitude;
    $("input[name='Latitude']").val(latitude);
    $("input[name='Longitude']").val(longitude);
    console.log(`Geolocalização recuperada [ ${latitude}° x ${longitude}° ]`);
}
function error(e) {
    console.error("Não foi possível recuperar sua geolocalização.");
}