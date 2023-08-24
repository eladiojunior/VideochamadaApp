namespace VideoChatApp.FrontEnd.Services.Exceptions;

public class ServiceException : Exception
{
    public ServiceException(string mensagem) : base(mensagem) { }
    public ServiceException(string mensagem, Exception innerException) : base(mensagem, innerException) { }
}