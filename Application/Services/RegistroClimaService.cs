using Domain.Entities;
using Domain.Interfaces;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class RegistroClimaService: IRegistroClimaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegistroClimaService> _logger;

        public RegistroClimaService(IUnitOfWork unitOfWork, ILogger<RegistroClimaService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Cidade?> ObterOuCriarCidade(CidadeResponseDTO cidadeResponse, CancellationToken cancellationToken = default)
        {
            if (cidadeResponse.Nome.Equals("Localização Desconhecida"))
            {
                _logger.LogDebug("Localização desconhecida detectada. Retornando null");
                return null;
            }

            _logger.LogDebug("Buscando cidade: {Cidade}, País: {Pais}", cidadeResponse.Nome, cidadeResponse.Pais);

            Cidade? cidade = await _unitOfWork.Cidades.ObterCidadePorNome(cidadeResponse.Nome, cancellationToken);

            if (cidade == null)
            {
                _logger.LogInformation("Cidade não encontrada. Criando nova cidade: {Cidade}, País: {Pais}", 
                    cidadeResponse.Nome, 
                    cidadeResponse.Pais);
                
                cidade = new Cidade(cidadeResponse.Nome, cidadeResponse.Pais);
                await _unitOfWork.Cidades.AdicionarCidade(cidade, cancellationToken);
            }
            else
            {
                _logger.LogDebug("Cidade encontrada no banco: {Cidade}, Id: {CidadeId}", cidade.Nome, cidade.CidadeId);
            }

            return cidade;
        }

        public async Task RegistrarClima(ClimaResponseDTO climaResponse, Cidade? cidade, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Registrando clima. Cidade: {Cidade}, Latitude: {Latitude}, Longitude: {Longitude}, Temperatura: {Temperatura}°C", 
                cidade?.Nome ?? "Localização Desconhecida", 
                climaResponse.Latitude, 
                climaResponse.Longitude, 
                climaResponse.Temperatura);

            RegistroClima registroClima = new (
                latitude: climaResponse.Latitude,
                longitude: climaResponse.Longitude,
                temperatura: climaResponse.Temperatura,
                temperaturaMin: climaResponse.TemperaturaMin,
                temperaturaMax: climaResponse.TemperaturaMax,
                condicao: climaResponse.Condicao,
                dataHora: climaResponse.DataHoraRegistro,
                cidade: cidade
            );

            await _unitOfWork.Climas.RegistrarClima(registroClima, cancellationToken);
            
            _logger.LogDebug("Registro de clima adicionado ao contexto");
        }
    }
}
