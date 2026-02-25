using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases
{
    public class ObterClimaPorCidadeUseCase
    {
        private readonly IClimaService _climaService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRegistroClimaService _registroClimaService;
        private readonly ILogger<ObterClimaPorCidadeUseCase> _logger;

        public ObterClimaPorCidadeUseCase(
            IClimaService climaService,
            IUnitOfWork unitOfWork,
            IRegistroClimaService registroClimaService,
            ILogger<ObterClimaPorCidadeUseCase> logger)
        {
            _climaService = climaService;
            _unitOfWork = unitOfWork;
            _registroClimaService = registroClimaService;
            _logger = logger;
        }

        public async Task<ClimaResponseDTO> Executar(CidadeRequestDTO cidadeRequest, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Iniciando UseCase ObterClimaPorCidade. Cidade: {Cidade}",
                cidadeRequest.Nome);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                ClimaResponseDTO climaResponse = await _climaService.ObterClimaPorCidade(cidadeRequest, cancellationToken);

                Cidade? cidade = await _registroClimaService.ObterOuCriarCidade(climaResponse.Cidade, cancellationToken);

                await _registroClimaService.RegistrarClima(climaResponse, cidade, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "UseCase ObterClimaPorCidade concluído com sucesso. Cidade: {Cidade}, País: {Pais}, Temperatura: {Temperatura}°C",
                    climaResponse.Cidade.Nome,
                    climaResponse.Cidade.Pais,
                    climaResponse.Temperatura);

                return climaResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro no UseCase ObterClimaPorCidade. Cidade: {Cidade}",
                    cidadeRequest.Nome);

                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
