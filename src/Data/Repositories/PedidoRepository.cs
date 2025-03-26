using GerenciadorPedido.Data.Context;
using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Interfaces.Repository;
using GerenciadorPedido.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace GerenciadorPedido.Data.Repositories
{
    public class PedidoRepository: Repository<Pedido>, IPedidoRepository
    {
        private readonly ILogger<PedidoRepository> _logger;

        public PedidoRepository(AppDbContext context, ILogger<PedidoRepository> logger) : base(context)
        {
            _logger = logger;
        }
        public async Task<Pedido?> ObterPedidoPorId(int id)
        {
            try
            {
                _logger.LogInformation("Buscando pedido com ID {PedidoId}", id);
                return await Db.Pedidos
                    .Include(p => p.Itens)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido com ID {PedidoId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Pedido>?> ObterPedidosPorStatus(PedidoStatus status)
        {
            try
            {
                _logger.LogInformation("Buscando pedidos com status {Status}", status);
                return await Db.Pedidos
                    .Include(p => p.Itens)
                    .Where(p => p.Status == status)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos com status {Status}", status);
                throw;
            }
        }
        public new async Task<IEnumerable<Pedido>> Buscar(Expression<Func<Pedido, bool>> predicate)
        {
            try
            {
                _logger.LogInformation("Buscando pedidos com critério de busca");
                return await Db.Pedidos
                    .Include(p => p.Itens)
                    .Where(predicate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos com o critério fornecido.");
                throw;
            }
        }
    }
}
