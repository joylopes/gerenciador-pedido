using GerenciadorPedido.Domain.Models;

namespace GerenciadorPedido.Domain.Interfaces.Repository
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        Task<Pedido?> ObterPedidoItensAsync(int id);
        Task<Pedido?> ObterPedidoItensAsync(string status);
    }
}
