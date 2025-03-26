using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Models;

namespace GerenciadorPedido.Domain.Interfaces.Repository
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        Task<Pedido?> ObterPedidoPorId(int id);
        Task<IEnumerable<Pedido>?> ObterPedidosPorStatus(PedidoStatus status);
    }
}
