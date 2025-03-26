using GerenciadorPedido.Domain.Enums;
using GerenciadorPedido.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciadorPedido.Data.Mappings
{
    public class PedidoMapping : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => new { p.PedidoId, p.ClienteId}).IsUnique();
            builder.HasAlternateKey(p => p.PedidoId);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Status).HasDefaultValue(PedidoStatus.Criado);

            builder.HasMany(p => p.Itens)
                .WithOne(p => p.Pedido)
                .HasForeignKey(p => p.PedidoId)
                .IsRequired();

            builder.ToTable("Pedidos");
        }
    }
}
