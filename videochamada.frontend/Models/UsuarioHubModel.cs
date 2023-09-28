namespace videochamada.frontend.Models;

public class UsuarioHubModel
{
    private Dictionary<string, string> usuarios = new Dictionary<string, string>();
   
    public string IdAtendimento { get; set; }

    public void AddUsuario(string idUsuarioHub, string idUsuario) {
        usuarios.Add(idUsuarioHub, idUsuario);
    }

    public bool HasExisteIdUsuario(string idUsuario)
    {
        return usuarios.Values.Count(c => c.Contains(idUsuario)) != 0;
    }
    public bool HasExisteIdUsuarioHub(string idUsuarioHub)
    {
        return usuarios.Keys.Contains(idUsuarioHub);
    }

    public void RemoverUsuarioHub(string idUsuarioHub)
    {
        usuarios.Remove(idUsuarioHub);
    }

    public int QtdUsuarios()
    {
        return usuarios.Count;
    }

    public string ObterUsuario(string idUsuarioHub)
    {
        return usuarios[idUsuarioHub];
    }

    public string ObterUsuarioDiferenteDe(string connectionId)
    {
        return usuarios.FirstOrDefault(w => !w.Key.Contains(connectionId)).Value;
    }
}