using System.Globalization;
using VideoChatApp.FrontEnd.Services.Enums;

namespace VideoChatApp.FrontEnd.Services;

public class ServiceHelper
{
    public static string GerarId()
    {
        return Guid.NewGuid().ToString();
    }

    public static int CalcularIdade(DateTime dataNascimento)
    {
        var dataCorrente = DateTime.Now.Date;
        return dataNascimento.Year - dataCorrente.Year - 1 +
               (dataNascimento.Month > dataCorrente.Month ||
               (dataNascimento.Month == dataCorrente.Month && dataNascimento.Day >= dataCorrente.Day) ? 1 : 0);
    }

    public static bool VerificarDataValida(string dataString, string formatoValidar = "dd/MM/yyyy")
    {
        if (string.IsNullOrEmpty(dataString))
            return false;
        var hasConvertido = DateTime
            .TryParseExact(dataString, formatoValidar,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dataConvertida);
        return hasConvertido;
    }

    public static DateTime? ConverterStringToDateTime(string dataString, string formatoValidar = "dd/MM/yyyy")
    {
        if (string.IsNullOrEmpty(dataString))
            return null;
        var hasConvertido = DateTime.TryParseExact(dataString, formatoValidar,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dataConvertida);
        return dataConvertida;
    }

    public static bool VerificarSexoValido(string sexoString)
    {
        if (string.IsNullOrEmpty(sexoString))
            return false;
        var infoValidas = new[] { "masculino", "feminino" };
        return infoValidas.Contains(sexoString.ToLower());
    }

    public static SexoBiologicoEnum ObterSexoEnum(string sexoString)
    {
        if (string.IsNullOrEmpty(sexoString))
            return SexoBiologicoEnum.NaoDefinido;
        var valorMasculinoValido = new[] { "m", "masculino", "1" };
        var valorFemininoValido = new[] { "f", "feminino", "2" };
        if (valorMasculinoValido.Contains(sexoString.ToLower()))
            return SexoBiologicoEnum.Masculino;
        if (valorFemininoValido.Contains(sexoString.ToLower()))
            return SexoBiologicoEnum.Feminino;
        return SexoBiologicoEnum.NaoDefinido;
    }
}