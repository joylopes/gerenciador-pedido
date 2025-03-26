using GerenciadorPedido.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GerenciadorPedido.Api.DTOs
{
    public class ItemPedidoDTO
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [JsonPropertyName("produtoId")]
        public int ProdutoId { get; set; }

        [JsonPropertyName("quantidade")]
        public int Quantidade { get; set; }

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }

        public ItemPedido MapeiaParaEntidade()
        {
            return new ItemPedido
            {
                ProdutoId = ProdutoId,
                Quantidade = Quantidade,
                Valor = Valor
            };
        }
        public ItemPedidoDTO MapeiaParaDTO(ItemPedido entidade)
        {
            return new ItemPedidoDTO
            {
                ProdutoId = entidade.ProdutoId,
                Quantidade = entidade.Quantidade,
                Valor = entidade.Valor
            };
        }
    }
}
