namespace GerenciadorPedido.Domain.Models
{
    public class ItemPedido : Entity
    {
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }

        /* EF Relation */
        public Pedido Pedido { get; set; }
    }
}
