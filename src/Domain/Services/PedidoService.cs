using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Interfaces;
using GerenciadorPedido.Domain.Interfaces.Repository;
using GerenciadorPedido.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Runtime.ConstrainedExecution;

namespace GerenciadorPedido.Domain.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _repository;
        private readonly IConfiguration _configuration;

        public PedidoService(IPedidoRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<Pedido> Adicionar(Pedido pedido)
        {
            ValidarDuplicidade(pedido);

            return await _repository.Adicionar(pedido);
        }

        public async Task<IEnumerable<Pedido>> ObterPorStatus(PedidoStatus status)
        {
            var pedidos = await _repository.ObterPedidosPorStatus(status) ?? [];

            return pedidos.ToList().Select(p => new Pedido
            {
                PedidoId = p.PedidoId,
                ClienteId = p.ClienteId,
                Imposto = CalcularImposto(p),
                Itens = p.Itens,
                Status = p.Status
            });
        }

        public void Dispose() => _repository?.Dispose();

        #region Private Methods
        private void ValidarDuplicidade(Pedido pedido)
        {
            var pedidos = _repository.Buscar(p =>
               p.ClienteId == pedido.ClienteId &&
               p.PedidoId == pedido.PedidoId &&
               p.Status == pedido.Status).Result;

            if (!pedidos.Any())
            {
                return;
            }

            var pedidoDuplicado = pedidos.Any(p => p.Itens.Count() == pedido.Itens.Count() &&
                p.Itens.All(i => pedido.Itens.Any(pi => pi.ProdutoId == i.ProdutoId && pi.Quantidade == i.Quantidade)));

            if (pedidoDuplicado)
            {
                throw new Exception("Já existe um pedido para este cliente.");
            }
        }
        private decimal CalcularImpostoVigente(Pedido pedido)
        {
            return pedido.Itens.Sum(i => i.Valor) * 0.3m;
        }

        private decimal CalcularImpostoReformaTributaria(Pedido pedido)
        {
            return pedido.Itens.Sum(i => i.Valor) * 0.2m;
        }

        private decimal CalcularImposto(Pedido pedido)
        {
            var UsarCalculoImpostoRT = _configuration["FeatureFlags:UsarCalculoImpostoRT"];
            return bool.TryParse(UsarCalculoImpostoRT, out _) ? CalcularImpostoReformaTributaria(pedido) : CalcularImpostoVigente(pedido);
        }
        #endregion
    }
}
