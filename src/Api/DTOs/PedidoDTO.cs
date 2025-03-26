using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GerenciadorPedido.Api.DTOs
{
    public class PedidoDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [JsonPropertyName("pedidoId")]
        public int PedidoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [JsonPropertyName("clienteId")]
        public int ClienteId { get; set; }

        [JsonPropertyName("imposto")]
        public decimal Imposto { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [JsonPropertyName("itens")]
        public IEnumerable<ItemPedidoDTO>? Itens { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        public Pedido MapeiaParaEntidade()
        {
            return new Pedido
            {
                PedidoId = PedidoId,
                ClienteId = ClienteId,
                Itens = Itens?.Select(i => i.MapeiaParaEntidade()).ToList()!
            };
        }
        public PedidoDTO MapeiaParaDTO(Pedido entidade)
        {
            return new PedidoDTO
            {
                Id = entidade.Id,
                PedidoId = entidade.PedidoId,
                ClienteId = entidade.ClienteId,
                Imposto = entidade.Imposto,
                Status = entidade.Status.ToString(),
                Itens = entidade.Itens.Select(i => new ItemPedidoDTO().MapeiaParaDTO(i))
            };
        }
    }
}
