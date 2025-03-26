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
        
        public async Task<Pedido?> ObterPorId(int id)
        {
            var pedido = await _repository.ObterPedidoPorId(id);

            if(pedido is null) return null;

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

        public void Dispose() => _repository?.Dispose();

        #region Private Methods
        private void ValidarDuplicidade(Pedido pedido)
        {
            var pedidos = _repository.Buscar(p =>
               p.ClienteId == pedido.ClienteId &&
               p.PedidoId == pedido.PedidoId).Result;

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
            var UsarCalculoImpostoRT = _configuration["FeatureFlags:UsarCalculoImpostoRT"];
            return bool.TryParse(UsarCalculoImpostoRT, out _) ? CalcularImpostoReformaTributaria(pedido) : CalcularImpostoVigente(pedido);
        }
        #endregion
    }
}
