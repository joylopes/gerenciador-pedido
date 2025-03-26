using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Interfaces;
using GerenciadorPedido.Domain.Interfaces.Repository;
using GerenciadorPedido.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GerenciadorPedido.Domain.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(IPedidoRepository repository, IConfiguration configuration, ILogger<PedidoService> logger)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Pedido> Adicionar(Pedido pedido)
        {
            try
            {
                _logger.LogInformation("Validando duplicidade do pedido {PedidoId} para o cliente {ClienteId}", pedido.PedidoId, pedido.ClienteId);
                ValidarDuplicidade(pedido);

                _logger.LogInformation("Adicionando pedido {PedidoId} para o cliente {ClienteId}", pedido.PedidoId, pedido.ClienteId);
                return await _repository.Adicionar(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar o pedido {PedidoId} para o cliente {ClienteId}", pedido.PedidoId, pedido.ClienteId);
                throw;
            }
        }

        public async Task<IEnumerable<Pedido>> ObterPorStatus(PedidoStatus status)
        {
            try
            {
                _logger.LogInformation("Obtendo pedidos com status {Status}", status);
                var pedidos = await _repository.ObterPedidosPorStatus(status) ?? [];

                var result = pedidos.Select(p => new Pedido
                {
                    Id = p.Id,
                    PedidoId = p.PedidoId,
                    ClienteId = p.ClienteId,
                    Imposto = CalcularImposto(p),
                    Itens = p.Itens,
                    Status = p.Status
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedidos com status {Status}", status);
                throw;
            }
        }

        public async Task<Pedido?> ObterPorId(int id)
        {
            try
            {
                _logger.LogInformation("Obtendo pedido com ID {Id}", id);
                var pedido = await _repository.ObterPedidoPorId(id);

                if (pedido is null) return null;

                return new Pedido
                {
                    Id = pedido.Id,
                    PedidoId = pedido.PedidoId,
                    ClienteId = pedido.ClienteId,
                    Imposto = CalcularImposto(pedido),
                    Itens = pedido.Itens,
                    Status = pedido.Status
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedido com ID {Id}", id);
                throw;
            }
        }

        public void Dispose() => _repository?.Dispose();

        #region Private Methods
        private void ValidarDuplicidade(Pedido pedido)
        {
            try
            {
                var pedidos = _repository.Buscar(p =>
                   p.ClienteId == pedido.ClienteId &&
                   p.PedidoId == pedido.PedidoId).Result;

                if (!pedidos.Any()) return;

                var pedidoDuplicado = pedidos.Any(p => p.Itens.Count() == pedido.Itens.Count() &&
                    p.Itens.All(i => pedido.Itens.Any(pi => pi.ProdutoId == i.ProdutoId && pi.Quantidade == i.Quantidade)));

                if (pedidoDuplicado)
                {
                    throw new InvalidOperationException("Já existe um pedido para este cliente.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar duplicidade do pedido {PedidoId} para o cliente {ClienteId}", pedido.PedidoId, pedido.ClienteId);
                throw;
            }
        }
        private static decimal CalcularImpostoVigente(Pedido pedido)
        {
            return pedido.Itens.Sum(i => i.Valor) * 0.3m;
        }

        private static decimal CalcularImpostoReformaTributaria(Pedido pedido)
        {
            return pedido.Itens.Sum(i => i.Valor) * 0.2m;
        }

        private decimal CalcularImposto(Pedido pedido)
        {
            try
            {
                var usarCalculoImpostoRT = _configuration["FeatureFlags:UsarCalculoImpostoRT"];
                _logger.LogInformation("Calculando imposto para pedido {PedidoId}, regra tributária: {UsarCalculoImpostoRT}", pedido.PedidoId, usarCalculoImpostoRT);

                return bool.TryParse(usarCalculoImpostoRT, out _)
                    ? CalcularImpostoReformaTributaria(pedido)
                    : CalcularImpostoVigente(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular imposto para o pedido {PedidoId}", pedido.PedidoId);
                throw;
            }
        }
        #endregion
    }
}
