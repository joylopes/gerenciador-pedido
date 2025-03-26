using FluentValidation;
using GerenciadorPedido.Api.DTOs;
using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Interfaces;
using GerenciadorPedido.Domain.Interfaces.Repository;
using GerenciadorPedido.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GerenciadorPedido.Api.Controllers
{
    [Route("api/pedidos")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IValidator<Pedido> _pedidoValidator;

        public PedidoController(IPedidoService pedidoService, IPedidoRepository pedidoRepository, IValidator<Pedido> pedidoValidator)
        {
            _pedidoService = pedidoService;
            _pedidoRepository = pedidoRepository;
            _pedidoValidator = pedidoValidator;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PedidoDTO>> ObterPedidoPorId(int id)
        {
            var pedido = await _pedidoRepository.ObterPedidoPorId(id);

            return pedido is not null ? Ok(new PedidoDTO().MapeiaParaDTO(pedido)) : NotFound();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDTO>?>> ObterPedidoPorStatus([FromQuery, Required] string status)
        {
            if (Enum.TryParse<PedidoStatus>(status, true, out var statusEnum))
            {
                var pedidos = await _pedidoService.ObterPorStatus(statusEnum);

                if (pedidos is null || !pedidos.Any()) return NotFound("Nenhum pedido foi encontrado para o status informado");

                return Ok(pedidos.Select(p => new PedidoDTO().MapeiaParaDTO(p)));
            }

            return BadRequest("Status inválido.");
        }

        [HttpPost]
        public async Task<ActionResult<PedidoCriadoDTO>> AdicionarPedido([FromBody] PedidoDTO pedidoDTO)
        {
            var pedido = pedidoDTO.MapeiaParaEntidade();
            var validationResult = _pedidoValidator.Validate(pedido);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _pedidoService.Adicionar(pedido);

            var uri = Url.Link("AdicionarPedido", new { id = pedido.Id });
            return Created(uri, new PedidoCriadoDTO().MapeiaParaDTO(result));
        }
    }
}
