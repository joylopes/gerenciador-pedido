using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Interfaces;
using GerenciadorPedido.Domain.Interfaces.Repository;
using GerenciadorPedido.Domain.Models;

namespace GerenciadorPedido.Domain.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _repository;

        public PedidoService(IPedidoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Pedido> Adicionar(Pedido pedido)
        {
            return await _repository.Adicionar(pedido);
        }

        public async Task<IEnumerable<Pedido>> ObterPorStatus(PedidoStatus status)
        {
            return await _repository.Buscar(p => p.Status == status);
        }

        public void Dispose() => _repository?.Dispose();
    }
}
