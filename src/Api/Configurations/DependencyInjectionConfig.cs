using GerenciadorPedido.Data.Context;
using GerenciadorPedido.Data.Repositories;
using GerenciadorPedido.Domain.Interfaces;
using GerenciadorPedido.Domain.Interfaces.Repository;
using GerenciadorPedido.Domain.Services;

namespace GerenciadorPedido.Api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<AppDbContext>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();

            services.AddScoped<IPedidoService, PedidoService>();

            return services;
        }
    }
}
