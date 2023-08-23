namespace VideoChatApp.FrontEnd.Services;

public class ServiceHelper
{
    public static string GerarId()
    {
        return Guid.NewGuid().ToString();
    }
}