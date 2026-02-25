using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases
{
    public class ObterHistoricoClimaUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ObterHistoricoClimaUseCase> _logger;

        public ObterHistoricoClimaUseCase(IUnitOfWork unitOfWork, ILogger<ObterHistoricoClimaUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<ClimaResponseDTO>> Executar(HistoricoRequestDTO? requestHistorico, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Iniciando UseCase ObterHistoricoClima. Filtros: Cidade={Cidade}, Latitude={Latitude}, Longitude={Longitude}", 
                requestHistorico?.NomeCidade, 
                requestHistorico?.Latitude, 
                requestHistorico?.Longitude);

            List<RegistroClima> listaRegistroClima = requestHistorico switch
            {
                { Latitude: not null, Longitude: not null } => 
                    await _unitOfWork.Climas.ObterHistoricoClimaPorCoordenadas(
                        requestHistorico.Latitude.Value, 
                        requestHistorico.Longitude.Value,
                        cancellationToken),
                
                { NomeCidade: not null } => 
                    await _unitOfWork.Climas.ObterHistoricoClimaPorCidade(
                        requestHistorico.NomeCidade,
                        cancellationToken),
                
                _ => await _unitOfWork.Climas.ObterHistoricoClima(cancellationToken),
            };

            _logger.LogInformation(
                "Histórico obtido. Total de registros: {TotalRegistros}", 
                listaRegistroClima.Count);

            List<ClimaResponseDTO> listaRegistroClimaResponse = [];

            foreach (RegistroClima registroClima in listaRegistroClima)
            {
                CidadeResponseDTO cidadeResponse = registroClima.Cidade == null
                    ? new CidadeResponseDTO("Localização Desconhecida", "")
                    : new CidadeResponseDTO(registroClima.Cidade.Nome, registroClima.Cidade.Pais);

                listaRegistroClimaResponse.Add(
                    new ClimaResponseDTO(
                        cidadeResponse,
                        registroClima.Latitude,
                        registroClima.Longitude,
                        registroClima.Temperatura,
                        registroClima.TemperaturaMin,
                        registroClima.TemperaturaMax,
                        registroClima.Condicao,
                        registroClima.DataHoraRegistro
                    )
                );
            }

            _logger.LogDebug("UseCase ObterHistoricoClima concluído");

            return listaRegistroClimaResponse;
        }
    }
}
