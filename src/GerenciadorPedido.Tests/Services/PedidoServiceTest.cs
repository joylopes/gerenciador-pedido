﻿using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using GerenciadorPedido.Domain.Interfaces.Repository;
using GerenciadorPedido.Domain.Services;
using NSubstitute;
using Microsoft.Extensions.Logging;
using GerenciadorPedido.Domain.Models;
using Microsoft.Extensions.Configuration;
using GerenciadorPedido.Domain.Enums;

namespace GerenciadorPedido.Tests.Services
{
    public class PedidoServiceTests
    {
        private readonly PedidoService _service;
        private readonly IPedidoRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PedidoService> _logger;

        private readonly Faker<Pedido> _pedidoFaker;

        public PedidoServiceTests()
        {
            _repository = Substitute.For<IPedidoRepository>();
            _configuration = Substitute.For<IConfiguration>();
            _logger = Substitute.For<ILogger<PedidoService>>();
            _service = new PedidoService(_repository, _configuration, _logger);
            _pedidoFaker = GenerarPedidoFake();
        }

        #region Adicionar

        [Fact]
        public async Task Adicionar_DeveLancarException_SePedidoDuplicado()
        {
            // Arrange
            var pedido = _pedidoFaker.Generate();
            _repository.Buscar(Arg.Any<Expression<Func<Pedido, bool>>>())
                .Returns(Task.FromResult(new List<Pedido> { pedido }.AsEnumerable()));

            // Act
            var act = async () => await _service.Adicionar(pedido);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Já existe um pedido para este cliente.");
        }

        [Fact]
        public async Task Adicionar_DeveRetornarPedido_SeNaoForDuplicado()
        {
            // Arrange
            var pedido = _pedidoFaker.Generate();
            _repository.Buscar(Arg.Any<Expression<Func<Pedido, bool>>>())
                .Returns(Task.FromResult(new List<Pedido>().AsEnumerable()));

            _repository.Adicionar(pedido).Returns(Task.FromResult(pedido));

            // Act
            var result = await _service.Adicionar(pedido);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(pedido);
        }
        #endregion

        #region Obter Por Id

        [Fact]
        public async Task ObterPorId_DeveRetornarPedido_SeExistir()
        {
            // Arrange
            var pedido = _pedidoFaker.Generate();
            _repository.ObterPedidoPorId(pedido.Id)
                .Returns(Task.FromResult(pedido));

            // Act
            var result = await _service.ObterPorId(pedido.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(pedido, options => options.Excluding(p => p.Imposto));
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNull_SeNaoExistir()
        {
            // Arrange
            _repository.ObterPedidoPorId(Arg.Any<int>())
                .Returns(Task.FromResult<Pedido>(null));

            // Act
            var result = await _service.ObterPorId(9999);

            // Assert
            result.Should().BeNull();
        }
        #endregion

        #region Obter Por Status

        [Fact]
        public async Task ObterPorStatus_DeveRetornarPedidos_SeExistirem()
        {
            // Arrange
            var pedidos = _pedidoFaker.Generate(3);
            _repository.ObterPedidosPorStatus(PedidoStatus.Criado)
                .Returns(Task.FromResult(pedidos.AsEnumerable()));

            // Act
            var result = await _service.ObterPorStatus(PedidoStatus.Criado);

            // Assert
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(pedidos, options => options.Excluding(p => p.Imposto));
        }

        [Fact]
        public async Task ObterPorStatus_DeveRetornarListaVazia_SeNaoExistiremPedidos()
        {
            // Arrange
            _repository.ObterPedidosPorStatus(Arg.Any<PedidoStatus>())
                .Returns(Task.FromResult<IEnumerable<Pedido>>(new List<Pedido>()));

            // Act
            var result = await _service.ObterPorStatus(PedidoStatus.Pendente);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region Private Methods
        private Faker<Pedido> GenerarPedidoFake()
        {
            return new Faker<Pedido>()
                .RuleFor(p => p.Id, f => f.Random.Int(1, 1000))
                .RuleFor(p => p.PedidoId, f => f.Random.Int(1, 1000))
                .RuleFor(p => p.ClienteId, f => f.Random.Int(1, 100))
                .RuleFor(p => p.Itens, f => new List<ItemPedido>
                {
                    new() { ProdutoId = f.Random.Int(1, 100), Quantidade = f.Random.Int(1, 10), Valor = f.Random.Decimal(10, 500) }
                })
                .RuleFor(p => p.Status, f => f.PickRandom<PedidoStatus>());
        }
        #endregion
    }
}
