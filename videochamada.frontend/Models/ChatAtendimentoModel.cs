namespace videochamada.frontend.Models;

public class ChatAtendimentoModel
{
    public List<MensagemChatModel> Mensagens { get; set; }

    public int QtdMensagens => Mensagens.Count;

    public void AddMensagem(MensagemChatModel mensagem)
    {
        if (mensagem==null)
            return;
        if (Mensagens == null)
            Mensagens = new List<MensagemChatModel>();
        Mensagens.Add(mensagem);
    }
}