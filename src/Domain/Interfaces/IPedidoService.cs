using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Models;

namespace GerenciadorPedido.Domain.Interfaces
{
    public interface IPedidoService : IDisposable
    {
        Task<Pedido> Adicionar(Pedido pedido);
        Task<IEnumerable<Pedido>> ObterPorStatus(PedidoStatus status);
        Task<Pedido?> ObterPorId(int id);
    }
}
