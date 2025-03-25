using GerenciadorPedido.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciadorPedido.Domain.Models
{
    public class Pedido : Entity
    {
        public Pedido()
        {
            Status = PedidoStatus.Criado;
            Itens = [];
        }
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        [NotMapped]
        public decimal Imposto { get; set; }
        public IEnumerable<ItemPedido> Itens { get; set; }
        public PedidoStatus Status { get; set; }
    }
}
