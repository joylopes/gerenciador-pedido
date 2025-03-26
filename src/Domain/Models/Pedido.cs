using GerenciadorPedido.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciadorPedido.Domain.Models
{
    public class Pedido : Entity
    {
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        [NotMapped]
        public decimal Imposto { get; set; }
        public PedidoStatus Status { get; set; }

        /* EF Relation */
        public IEnumerable<ItemPedido> Itens { get; set; } = null!;
    }
}
