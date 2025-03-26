using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Models;
using System.Text.Json.Serialization;

namespace GerenciadorPedido.Api.DTOs
{
    public class PedidoCriadoDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        public PedidoCriadoDTO MapeiaParaDTO(Pedido entidade)
        {
            return new PedidoCriadoDTO
            {
                Id = entidade.Id,
                Status = entidade.Status.ToString(),
            };
        }
    }
}
