using videochamada.frontend.Models;

namespace videochamada.frontend.Helper;

public class GerenciadorFilaCliente
{
    private static GerenciadorFilaCliente instancia = null;
    private Queue<ClienteModel> _fila = null;

    private GerenciadorFilaCliente()
    {
        _fila = new Queue<ClienteModel>();
    }

    public static GerenciadorFilaCliente Get()
    {
        return instancia ??= new GerenciadorFilaCliente();
    }
    
    public int EntrarNaFila(ClienteModel cliente)
    {
        _fila.Enqueue(cliente);
        return PosicaoNaFila(cliente.Id);
    }

    public ClienteModel AtenderCliente()
    {
        return _fila.Count == 0 ? null : _fila.Dequeue();
    }

    public List<ClienteModel> ListarClientesFila()
    {
        return _fila.ToList();
    }

    public int QuantidadeClientesFila()
    {
        return _fila.Count;
    }
    
    public int PosicaoNaFila(string idCliente)
    {
        var posicao = 0;
        foreach (var cliente in _fila)
        {
            if (cliente.Id == idCliente)
                return posicao;
            posicao++;
        }
        return posicao;  // Cliente não encontrado
    }

    public void RemoverDaFila(string idCliente)
    {
        var posicaoNaFila = PosicaoNaFila(idCliente);
        if (posicaoNaFila != 0)
        {//Está na fila... remover
            _fila.Where(w => w.Id.Equals(idCliente));
        }
    }
}