using GerenciadorPedido.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciadorPedido.Data.Mappings
{
    public class ItemPedidoMapping : IEntityTypeConfiguration<ItemPedido>
    {
        public void Configure(EntityTypeBuilder<ItemPedido> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.ProdutoId)
                .IsRequired();

            builder.HasOne(i => i.Pedido) 
                 .WithMany(p => p.Itens)
                 .HasForeignKey(i => i.PedidoId);

            builder.ToTable("ItensPedido");
        }
    }
}
