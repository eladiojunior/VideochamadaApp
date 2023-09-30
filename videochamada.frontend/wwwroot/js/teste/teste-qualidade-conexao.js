QualidadeConexao = {
    InitTesteQualidade: function() {
        setInterval(QualidadeConexao.VerificarQualidade(), 10000);
    },
    VerificarQualidade: function () {
        let connection = (navigator.connection || navigator.mozConnection || navigator.webkitConnection);
        if (connection) {
            let typeConenectionSlow = ['slow-2g','2g','3g'];
            let connectionType = connection.effectiveType;
            $(".qualidade-conexao").text(connectionType);
            if (typeConenectionSlow.includes(connectionType)) {
                alert('Sua conexão está lenta e afetar a qualidade de sua videochamada.');
            }
        }
    }
}
$(function () {
    QualidadeConexao.InitTesteQualidade();
});