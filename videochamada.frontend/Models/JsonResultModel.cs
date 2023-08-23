namespace videochamada.frontend.Models;

public class JsonResultModel
{
    public JsonResultModel(bool hasErro, object data, string mensagem)
    {
        HasErro = hasErro;
        Data = data;
        Mensagem = mensagem;
    }
    public JsonResultModel(object data)
    {
        HasErro = false;
        Data = data;
        Mensagem = "";
    }
    
    public bool HasErro { get; set; }
    public string Mensagem { get; set; }
    public object Data { get; set; }
}