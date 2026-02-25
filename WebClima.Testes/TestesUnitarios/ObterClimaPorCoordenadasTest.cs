using Application.DTOs;
using Application.Interfaces;
using Application.UseCases;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace WebClima.Tests.TestesUnitarios
{
    public class ObterClimaPorCoordenadasTest
    {
        private readonly Mock<ILogger<ObterClimaPorCoordenadasUseCase>> _mockLogger;

        public ObterClimaPorCoordenadasTest()
        {
            _mockLogger = new Mock<ILogger<ObterClimaPorCoordenadasUseCase>>();
        }

        [Fact]
        public async Task Executar_DeveBuscarClimaPorCoordenadasObterOuCriarCidadeERegistrarClima_RetornarClima()
        {
            // Arrange
            var mockClimaService = new Mock<IClimaService>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRegistroClimaService = new Mock<IRegistroClimaService>();

            var coordenadasRequest = new CoordenadasRequestDTO(-23, -46);
            var cidadeResponse = new CidadeResponseDTO("São Paulo", "BR");
            var climaResponse = new ClimaResponseDTO(cidadeResponse, 12, 12, 25.5, 22.3, 27.3, "Nublado", DateTime.Now);

            var cidade = new Cidade("São Paulo", "BR");

            mockClimaService
                .Setup(s => s.ObterClimaPorCoordenadas(coordenadasRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(climaResponse);

            mockRegistroClimaService
                .Setup(s => s.ObterOuCriarCidade(climaResponse.Cidade, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cidade);

            mockRegistroClimaService
                .Setup(s => s.RegistrarClima(climaResponse, cidade, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var useCase = new ObterClimaPorCoordenadasUseCase(
                mockClimaService.Object,
                mockUnitOfWork.Object,
                mockRegistroClimaService.Object,
                _mockLogger.Object);

            // Act
            var resultado = await useCase.Executar(coordenadasRequest);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("São Paulo", resultado.Cidade.Nome);
            Assert.Equal(25.5, resultado.Temperatura);

            mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockClimaService.Verify(s => s.ObterClimaPorCoordenadas(coordenadasRequest, It.IsAny<CancellationToken>()), Times.Once);
            mockRegistroClimaService.Verify(s => s.ObterOuCriarCidade(climaResponse.Cidade, It.IsAny<CancellationToken>()), Times.Once);
            mockRegistroClimaService.Verify(s => s.RegistrarClima(climaResponse, cidade, It.IsAny<CancellationToken>()), Times.Once);
            mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task Executar_QuandoOcorreErro_DeveFazerRollBack()
        {
            var mockClimaService = new Mock<IClimaService>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRegistroClimaService = new Mock<IRegistroClimaService>();

            var coordenadasRequest = new CoordenadasRequestDTO(-23, -46);

            mockClimaService
                .Setup(s => s.ObterClimaPorCoordenadas(coordenadasRequest, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro ao obter clima"));

            var useCase = new ObterClimaPorCoordenadasUseCase(
                mockClimaService.Object,
                mockUnitOfWork.Object,
                mockRegistroClimaService.Object,
                _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => useCase.Executar(coordenadasRequest));

            mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
            mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
