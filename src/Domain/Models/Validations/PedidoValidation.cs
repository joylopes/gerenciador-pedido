using FluentValidation;

namespace GerenciadorPedido.Domain.Models.Validations
{
    public class PedidoValidation : AbstractValidator<Pedido>
    {
        public PedidoValidation()
        {
            RuleFor(o => o.ClienteId)
                .GreaterThan(0).WithMessage("{PropertyName} inválido, {PropertyName}: {ComparisonValue}.");
            RuleFor(o => o.PedidoId)
                .GreaterThan(0).WithMessage("{PropertyName} inválido, {PropertyName}: {ComparisonValue}.");

            RuleForEach(o => o.Itens)
                .SetValidator(new ItemValidation());
        }
    }
}
