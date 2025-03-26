using FluentValidation;

namespace GerenciadorPedido.Domain.Models.Validations
{
    public class ItemValidation : AbstractValidator<ItemPedido>
    {
        public ItemValidation()
        {

            RuleFor(i => i.ProdutoId)
                .GreaterThan(0).WithMessage("O {PropertyName} deve ser maior que {ComparisonValue}");

            RuleFor(i => i.Valor)
                .GreaterThanOrEqualTo(0).WithMessage("O {PropertyName} deve ser maior ou igual a {ComparisonValue}");

            RuleFor(i => i.Quantidade)
                .GreaterThanOrEqualTo(0).WithMessage("A {PropertyName} deve ser maior ou igual {ComparisonValue}");
        }
    }
}
