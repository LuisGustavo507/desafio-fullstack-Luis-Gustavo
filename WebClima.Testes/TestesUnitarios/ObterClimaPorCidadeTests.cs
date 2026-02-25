using Application.DTOs;
using Application.Interfaces;
using Application.UseCases;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace WebClima.Tests.TestesUnitarios
{
    public class ObterClimaPorCidadeTests
    {
        private readonly Mock<ILogger<ObterClimaPorCidadeUseCase>> _mockLogger;

        public ObterClimaPorCidadeTests()
        {
            _mockLogger = new Mock<ILogger<ObterClimaPorCidadeUseCase>>();
        }

        [Fact]
        public async Task Executar_DeveBuscarClima_ObterOuRegistrarCidadeEClima()
        {
            // Arrange
            var mockClimaService = new Mock<IClimaService>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRegistroClimaService = new Mock<IRegistroClimaService>();

            var cidadeRequest = new CidadeRequestDTO("Sao Paulo");
            var cidadeResponse = new CidadeResponseDTO("São Paulo", "BR");
            var climaResponse = new ClimaResponseDTO(cidadeResponse, 12, 12, 25.5, 22.3, 27.3, "Nublado", DateTime.Now);

            var cidade = new Cidade("São paulo", "BR");

            mockClimaService
                .Setup(s => s.ObterClimaPorCidade(cidadeRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(climaResponse);

            mockRegistroClimaService
                .Setup(s => s.ObterOuCriarCidade(climaResponse.Cidade, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cidade);

            mockRegistroClimaService
                .Setup(s => s.RegistrarClima(climaResponse, cidade, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var useCase = new ObterClimaPorCidadeUseCase(
                mockClimaService.Object,
                mockUnitOfWork.Object,
                mockRegistroClimaService.Object,
                _mockLogger.Object);

            // Act
            var resultado = await useCase.Executar(cidadeRequest);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("São Paulo", resultado.Cidade.Nome);
            Assert.Equal(25.5, resultado.Temperatura);

            mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockClimaService.Verify(s => s.ObterClimaPorCidade(cidadeRequest, It.IsAny<CancellationToken>()), Times.Once);
            mockRegistroClimaService.Verify(s => s.ObterOuCriarCidade(climaResponse.Cidade, It.IsAny<CancellationToken>()), Times.Once);
            mockRegistroClimaService.Verify(s => s.RegistrarClima(climaResponse, cidade, It.IsAny<CancellationToken>()), Times.Once);
            mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task Executar_QuandoOcorreErro_DeveFazerRollback()
        {
            // Arrange
            var mockClimaService = new Mock<IClimaService>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRegistroClimaService = new Mock<IRegistroClimaService>();

            var cidadeRequest = new CidadeRequestDTO("Cascavel");

            mockClimaService
                .Setup(s => s.ObterClimaPorCidade(cidadeRequest, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro ao buscar clima"));

            var useCase = new ObterClimaPorCidadeUseCase(
                mockClimaService.Object,
                mockUnitOfWork.Object,
                mockRegistroClimaService.Object,
                _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => useCase.Executar(cidadeRequest));

            mockUnitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
            mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
