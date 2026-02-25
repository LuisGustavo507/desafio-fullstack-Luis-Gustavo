using Application.DTOs;
using Application.UseCases;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace WebClima.Testes.TestesUnitarios
{
    public class ObterHistoricoClimaUseCaseTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IClimaRepository> _mockClimaRepository;
        private readonly Mock<ILogger<ObterHistoricoClimaUseCase>> _mockLogger;

        public ObterHistoricoClimaUseCaseTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockClimaRepository = new Mock<IClimaRepository>();
            _mockLogger = new Mock<ILogger<ObterHistoricoClimaUseCase>>();
            _mockUnitOfWork.Setup(u => u.Climas).Returns(_mockClimaRepository.Object);
        }

        [Fact]
        public async Task Executar_ComLatitudeLongitude_DeveRetornarHistoricoPorCoordenadas()
        {
            // Arrange
            var requestHistorico = new HistoricoRequestDTO(-23, -46, null);

            var cidade = new Cidade("São Paulo", "Brasil");

            List<RegistroClima> registrosClima = new();

            registrosClima.Add(new RegistroClima(-23, -46, 25.5, 20.0, 30.0, "Ensolarado", DateTime.Now, cidade));

            _mockClimaRepository
                .Setup(r => r.ObterHistoricoClimaPorCoordenadas(
                    requestHistorico.Latitude.Value,
                    requestHistorico.Longitude.Value,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(registrosClima);


            var useCase = new ObterHistoricoClimaUseCase(_mockUnitOfWork.Object, _mockLogger.Object);

            // Act
            var resultado = await useCase.Executar(requestHistorico);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("São Paulo", resultado[0].Cidade.Nome);
            Assert.Equal("Brasil", resultado[0].Cidade.Pais);
            Assert.Equal(-23, resultado[0].Latitude);
            Assert.Equal(-46, resultado[0].Longitude);
            Assert.Equal(25.5, resultado[0].Temperatura);
            
            _mockClimaRepository.Verify(
                r => r.ObterHistoricoClimaPorCoordenadas(
                    requestHistorico.Latitude.Value,
                    requestHistorico.Longitude.Value,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Executar_ComNomeCidade_DeveRetornarHistoricoPorCidade()
        {
            // Arrange
            var requestHistorico = new HistoricoRequestDTO(null, null, "Rio de Janeiro");

            var cidade = new Cidade("Rio de Janeiro", "Brasil");

            List<RegistroClima> registrosClima = new();

            registrosClima.Add(new RegistroClima(-22.9068m, -43.1729m, 28.0, 24.0, 32.0, "Parcialmente nublado", DateTime.Now, cidade));
            registrosClima.Add(new RegistroClima(-22.9068m, -43.1729m, 27.5, 23.0, 31.0, "Nublado", DateTime.Now.AddHours(-1), cidade));

            _mockClimaRepository
                .Setup(r => r.ObterHistoricoClimaPorCidade(
                    requestHistorico.NomeCidade,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(registrosClima);

            var useCase = new ObterHistoricoClimaUseCase(_mockUnitOfWork.Object, _mockLogger.Object);

            // Act
            var resultado = await useCase.Executar(requestHistorico);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, r => Assert.Equal("Rio de Janeiro", r.Cidade.Nome));

            _mockClimaRepository.Verify(
                r => r.ObterHistoricoClimaPorCidade(
                    requestHistorico.NomeCidade,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Executar_SemFiltros_DeveRetornarHistoricoCompleto()
        {
            // Arrange
            var requestHistorico = new HistoricoRequestDTO(null, null, null);

            var cidade1 = new Cidade("São Paulo", "Brasil");
            var cidade2 = new Cidade("Rio de Janeiro", "Brasil");

            List<RegistroClima> registrosClima = new();

            registrosClima.Add(new RegistroClima(-23.5505m, -46.6333m, 25.5, 20.0, 30.0, "Ensolarado", DateTime.Now, cidade1));
            registrosClima.Add(new RegistroClima(-22.9068m, -43.1729m, 28.0, 24.0, 32.0, "Parcialmente nublado", DateTime.Now, cidade2));

            _mockClimaRepository
                .Setup(r => r.ObterHistoricoClima(It.IsAny<CancellationToken>()))
                .ReturnsAsync(registrosClima);

            var useCase = new ObterHistoricoClimaUseCase(_mockUnitOfWork.Object, _mockLogger.Object);

            // Act
            var resultado = await useCase.Executar(requestHistorico);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, r => r.Cidade.Nome == "São Paulo");
            Assert.Contains(resultado, r => r.Cidade.Nome == "Rio de Janeiro");

            _mockClimaRepository.Verify(
                r => r.ObterHistoricoClima(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Executar_ComRequestNull_DeveRetornarHistoricoCompleto()
        {
            // Arrange
            HistoricoRequestDTO? requestHistorico = null;

            var cidade = new Cidade("São Paulo", "Brasil");

            List<RegistroClima> registrosClima = new();

            registrosClima.Add(new RegistroClima(-23.5505m, -46.6333m, 25.5, 20.0, 30.0, "Ensolarado", DateTime.Now, cidade));

            _mockClimaRepository
                .Setup(r => r.ObterHistoricoClima(It.IsAny<CancellationToken>()))
                .ReturnsAsync(registrosClima);

            var useCase = new ObterHistoricoClimaUseCase(_mockUnitOfWork.Object, _mockLogger.Object);

            // Act
            var resultado = await useCase.Executar(requestHistorico);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);

            _mockClimaRepository.Verify(
                r => r.ObterHistoricoClima(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Executar_ComCidadeNula_DeveRetornarLocalizacaoDesconhecida()
        {
            // Arrange
            var requestHistorico = new HistoricoRequestDTO(-23.5505m, -46.6333m, null);

            List<RegistroClima> registrosClima = new();

            registrosClima.Add(new RegistroClima(-23.5505m, -46.6333m, 25.5, 20.0, 30.0, "Ensolarado", DateTime.Now, null));

            _mockClimaRepository
                .Setup(r => r.ObterHistoricoClimaPorCoordenadas(
                    requestHistorico.Latitude.Value,
                    requestHistorico.Longitude.Value,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(registrosClima);

            var useCase = new ObterHistoricoClimaUseCase(_mockUnitOfWork.Object, _mockLogger.Object);

            // Act
            var resultado = await useCase.Executar(requestHistorico);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("Localização Desconhecida", resultado[0].Cidade.Nome);
            Assert.Equal("", resultado[0].Cidade.Pais);

            _mockClimaRepository.Verify(
                r => r.ObterHistoricoClimaPorCoordenadas(
                    requestHistorico.Latitude.Value,
                    requestHistorico.Longitude.Value,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Executar_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var requestHistorico = new HistoricoRequestDTO(null, null, "Cidade Inexistente");

            List<RegistroClima> registrosClima = new();

            _mockClimaRepository
                .Setup(r => r.ObterHistoricoClimaPorCidade(
                    requestHistorico.NomeCidade,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(registrosClima);

            var useCase = new ObterHistoricoClimaUseCase(_mockUnitOfWork.Object, _mockLogger.Object);

            // Act
            var resultado = await useCase.Executar(requestHistorico);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            _mockClimaRepository.Verify(
                r => r.ObterHistoricoClimaPorCidade(
                    requestHistorico.NomeCidade,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}