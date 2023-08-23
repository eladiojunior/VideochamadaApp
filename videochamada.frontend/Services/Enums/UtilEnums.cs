using System.ComponentModel;

namespace VideoChatApp.FrontEnd.Services.Enums;
public static class UtilEnums
{
    /// <summary>
    ///     Recupera a lista de enums &gt;E&lt; conforme o tipo informado
    /// </summary>
    /// <typeparam name="E">tipo do enum que será recuperado a lista.</typeparam>
    /// <returns></returns>
    public static List<E> ListarEnums<E>()
    {
        return Enum.GetValues(typeof (E)).OfType<E>().ToList();
    }

    /// <summary>
    ///     Recupera o código (flag) atribuido ao enum.
    /// </summary>
    /// <param name="enumValue">Enum, para extração do código.</param>
    /// <returns></returns>
    public static int ObterCodigoEnum(this Enum enumValue)
    {
        return enumValue.GetHashCode();
    }

    /// <summary>
    ///     Recupera a instância de um enum &gt;E&lt; conforme o tipo informado.
    /// </summary>
    /// <typeparam name="E">Tipo do enum a ser retornado.</typeparam>
    /// <param name="enumCodigo">Código (flag) do enum para recuperação da instância.</param>
    /// <returns></returns>
    public static E ObterEnumPorCodigo<E>(int enumCodigo)
    {
        return (E) Enum.Parse(typeof (E), Convert.ToString(enumCodigo));
    }

    /// <summary>
    ///     Recupera o texto do enum;
    /// </summary>
    /// <param name="enumValue">Enum, para extração do texto.</param>
    /// <returns></returns>
    public static string ObterTextoEnum(this Enum enumValue)
    {
        var result = "";
        var memInfo = enumValue.GetType().GetMember(enumValue.ToString());
        if (memInfo != null && memInfo.Length != 0)
        {
            var attributes = memInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);
            result = ((DescriptionAttribute) attributes[0]).Description;
        }
        return result;
    }

    /// <summary>
    ///     Recupera o texto do enum &gt;E&lt; conforme o tipo informado.
    /// </summary>
    /// <typeparam name="E">Tipo do enum a ser retornado.</typeparam>
    /// <param name="enumCodigo">Código (flag) do enum para recuperação do texto.</param>
    /// <returns></returns>
    public static string ObterTextoEnumPorCodigo<E>(int enumCodigo)
    {
        return (ObterEnumPorCodigo<E>(enumCodigo) as Enum).ObterTextoEnum();
    }

    /// <summary>
    ///     Valida se o código do tipo enum é valido para um objeto enum informado;
    /// </summary>
    /// <param name="enumTipo">Enum, para validação do código tipo existente.</param>
    /// <param name="enumCodigo">Código do tipo enum para validação.</param>
    /// <returns></returns>
    public static bool ValidarCodigoTipoEnum(this Enum enumValue, int enumCodigo)
    {
        var isTipoDefinidoEnum = false;
        //Verificar se o código do tipo de ambiente do sistema está definido no Enum.
        foreach (var tipo in Enum.GetValues(enumValue.GetType()))
        {
            isTipoDefinidoEnum = (int) tipo == enumCodigo;
            if (isTipoDefinidoEnum) break;
        }
        return isTipoDefinidoEnum;
    }
}