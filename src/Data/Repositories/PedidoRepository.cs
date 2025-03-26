using GerenciadorPedido.Data.Context;
using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Interfaces.Repository;
using GerenciadorPedido.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GerenciadorPedido.Data.Repositories
{
    public class PedidoRepository(AppDbContext context) : Repository<Pedido>(context), IPedidoRepository
    {
        public async Task<Pedido?> ObterPedidoPorId(int id)
        {
            return await Db.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pedido>?> ObterPedidosPorStatus(PedidoStatus status)
        {
            return await Db.Pedidos
                .Include(p => p.Itens)
                .Where(p => p.Status == status)
                .ToListAsync();
        }
        public new async Task<IEnumerable<Pedido>> Buscar(Expression<Func<Pedido, bool>> predicate)
        {
            return await Db.Pedidos
                .Include(p => p.Itens)
                .Where(predicate)
                .ToListAsync();
        }
    }
}
