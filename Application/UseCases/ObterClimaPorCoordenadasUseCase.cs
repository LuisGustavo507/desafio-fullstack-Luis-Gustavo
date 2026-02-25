using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases
{
    public class ObterClimaPorCoordenadasUseCase
    {
        private readonly IClimaService _climaService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRegistroClimaService _registroClimaService;
        private readonly ILogger<ObterClimaPorCoordenadasUseCase> _logger;

        public ObterClimaPorCoordenadasUseCase(
            IClimaService climaService,
            IUnitOfWork unitOfWork,
            IRegistroClimaService registroClimaService,
            ILogger<ObterClimaPorCoordenadasUseCase> logger)
        {
            _climaService = climaService;
            _unitOfWork = unitOfWork;
            _registroClimaService = registroClimaService;
            _logger = logger;
        }

        public async Task<ClimaResponseDTO> Executar(CoordenadasRequestDTO coordenadas, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Iniciando UseCase ObterClimaPorCoordenadas. Latitude: {Latitude}, Longitude: {Longitude}", 
                coordenadas.Latitude, 
                coordenadas.Longitude);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                ClimaResponseDTO climaResponse = await _climaService.ObterClimaPorCoordenadas(coordenadas, cancellationToken);

                Cidade? cidade = await _registroClimaService.ObterOuCriarCidade(climaResponse.Cidade, cancellationToken);

                await _registroClimaService.RegistrarClima(climaResponse, cidade, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "UseCase ObterClimaPorCoordenadas concluído com sucesso. Cidade: {Cidade}, Temperatura: {Temperatura}°C", 
                    climaResponse.Cidade.Nome, 
                    climaResponse.Temperatura);

                return climaResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Erro no UseCase ObterClimaPorCoordenadas. Latitude: {Latitude}, Longitude: {Longitude}", 
                    coordenadas.Latitude, 
                    coordenadas.Longitude);
                
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
