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
        private readonly IValidator<Pedido> _pedidoValidator;
        private readonly ILogger<PedidoController> _logger;

        public PedidoController(IPedidoService pedidoService, IValidator<Pedido> pedidoValidator, ILogger<PedidoController> logger)
        {
            _pedidoService = pedidoService;
            _pedidoValidator = pedidoValidator;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PedidoDTO>> ObterPedidoPorId(int id)
        {
            try
            {
                _logger.LogInformation("Buscando pedido com ID {Id}", id);
                var pedido = await _pedidoService.ObterPorId(id);

                if (pedido is null)
                {
                    _logger.LogWarning("Pedido com ID {Id} não encontrado", id);
                    return NotFound($"Pedido com ID {id} não encontrado");
                }

                return Ok(new PedidoDTO().MapeiaParaDTO(pedido));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedido com ID {Id}", id);
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDTO>?>> ObterPedidoPorStatus([FromQuery, Required] string status)
        {
            try
            {
                if (!Enum.TryParse<PedidoStatus>(status, true, out var statusEnum))
                {
                    _logger.LogWarning("Status inválido fornecido: {Status}", status);
                    return BadRequest(@$"Status inválido fornecido: {status}");
                }

                _logger.LogInformation("Buscando pedidos com status {Status}", statusEnum);
                var pedidos = await _pedidoService.ObterPorStatus(statusEnum);

                if (pedidos is null || !pedidos.Any())
                {
                    _logger.LogWarning("Nenhum pedido encontrado para o status {Status}", statusEnum);
                    return NotFound(@$"Nenhum pedido foi encontrado para o status {status} informado");
                }

                return Ok(pedidos.Select(p => new PedidoDTO().MapeiaParaDTO(p)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedidos com status {Status}", status);
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PedidoCriadoDTO>> AdicionarPedido([FromBody] PedidoDTO pedidoDTO)
        {
            try
            {
                var pedido = pedidoDTO.MapeiaParaEntidade();
                var validationResult = _pedidoValidator.Validate(pedido);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Pedido inválido enviado: {Erros}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                _logger.LogInformation("Adicionando novo pedido para o cliente {ClienteId}", pedido.ClienteId);
                var result = await _pedidoService.Adicionar(pedido);

                var uri = Url.Link("AdicionarPedido", new { id = pedido.Id });
                return CreatedAtAction(nameof(AdicionarPedido), new PedidoCriadoDTO().MapeiaParaDTO(result));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro ao adicionar pedido para o cliente {ClienteId}", pedidoDTO.ClienteId);
                return Conflict(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar pedido para o cliente {ClienteId}", pedidoDTO.ClienteId);
                return StatusCode(500, "Erro interno no servidor.");
            }
        }
    }
}
