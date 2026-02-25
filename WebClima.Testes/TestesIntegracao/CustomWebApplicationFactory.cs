using Application.Interfaces;
using Application.DTOs;
using Infrastructure.Data.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace WebClima.Testes.TestesIntegracao
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o DbContext existente
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona DbContext em memória
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Substitui o IClimaService por um mock para não fazer chamadas reais à API externa
                var climaServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IClimaService));

                if (climaServiceDescriptor != null)
                {
                    services.Remove(climaServiceDescriptor);
                }

                // Cria mock do IClimaService
                var mockClimaService = new Mock<IClimaService>();

                mockClimaService
                    .Setup(s => s.ObterClimaPorCoordenadas(
                        It.IsAny<CoordenadasRequestDTO>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((CoordenadasRequestDTO coords, CancellationToken ct) =>
                    {
                        return new ClimaResponseDTO(
                            new CidadeResponseDTO("São Paulo", "Brasil"),
                            coords.Latitude,
                            coords.Longitude,
                            25.5,
                            20.0,
                            30.0,
                            "Ensolarado",
                            DateTime.UtcNow
                        );
                    });

                mockClimaService
                    .Setup(s => s.ObterClimaPorCidade(
                        It.IsAny<CidadeRequestDTO>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((CidadeRequestDTO cidade, CancellationToken ct) =>
                    {
                        return new ClimaResponseDTO(
                            new CidadeResponseDTO(cidade.Nome, "Brasil"),
                            -23.5505m,
                            -46.6333m,
                            25.5,
                            20.0,
                            30.0,
                            "Ensolarado",
                            DateTime.UtcNow
                        );
                    });

                services.AddSingleton(mockClimaService.Object);

                // Garante que o banco seja criado
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                db.Database.EnsureCreated();
            });
        }
    }
}