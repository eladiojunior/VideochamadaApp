using VideoChatApp.FrontEnd.Services;
using VideoChatApp.FrontEnd.Services.Enums;

namespace videochamada.frontend.Models;

public class ClienteModel
{
    public string Id { get; set; }
    public string Nome { get; set; }
    public SexoBiologicoEnum Sexo { get; set; }
    public DateTime DataNascimento { get; set; }
    public int Idade => ServiceHelper.CalcularIdade(DataNascimento);
    public string Email { get; set; }
    public string Telefone { get; set; }
    public List<AtendimentoModel> Atendimentos { get; set; }
}