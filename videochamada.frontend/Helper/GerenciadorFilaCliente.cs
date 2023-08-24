using System.Collections.Concurrent;
using videochamada.frontend.Models;

namespace videochamada.frontend.Helper;

public class GerenciadorFilaCliente
{
    private static GerenciadorFilaCliente instancia = null;
    private static ConcurrentQueue<ItemFila> _fila = null;

    private GerenciadorFilaCliente()
    {
        _fila = new ConcurrentQueue<ItemFila>();
    }

    public static GerenciadorFilaCliente Get()
    {
        if (instancia == null)
            instancia = new GerenciadorFilaCliente();
        return instancia;
    }
    
    public int EntrarNaFila(ClienteModel cliente)
    {
        _fila.Enqueue(new ItemFila(cliente));
        return PosicaoNaFila(cliente.Id);
    }

    public ClienteModel AtenderCliente()
    {
        if (_fila.Count == 0) return null;
        while (_fila.TryDequeue(out var itemFila))
            if (!itemFila.HasRemovido)
                return itemFila.Cliente;
        return null;
    }

    public List<ClienteModel> ListarClientesFila()
    {
        return _fila.Where(w => w.HasRemovido == false).Select(s => s.Cliente).ToList();
    }

    public int QuantidadeClientesFila()
    {
        return _fila.Count(w => w.HasRemovido == false);
    }
    
    public int PosicaoNaFila(string idCliente)
    {
        var posicao = 0;
        var hasExiste = false;
        var listaClientes = ListarClientesFila();
        foreach (var cliente in listaClientes)
        {
            posicao++;
            if (cliente.Id != idCliente) continue;
            hasExiste = true;
            break;
        }
        return hasExiste ? posicao : 0;
    }

    public void RemoverDaFila(string idCliente)
    {
        var posicaoNaFila = PosicaoNaFila(idCliente);
        if (posicaoNaFila != 0)
        {//Está na fila... remover
            var item = _fila.Where(w => w.Cliente.Id.Equals(idCliente)).FirstOrDefault();
            item.Remover();
        }
    }
}

class ItemFila
{
    public ItemFila(ClienteModel cliente)
    {
        Cliente = cliente;
    }
    private bool hasRemovido = false;
    public ClienteModel Cliente { get; set; }
    public bool HasRemovido => hasRemovido;
    internal void Remover()
    {
        hasRemovido = true;
    }
}