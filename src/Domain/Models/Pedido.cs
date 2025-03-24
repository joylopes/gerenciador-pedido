using GerenciadorPedido.Domain.Enums;

namespace GerenciadorPedido.Domain.Models
{
    public class Pedido : Entity
    {
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        public IEnumerable<ItemPedido> Itens { get; set; }
        public PedidoStatus Status { get; set; }
    }
}
