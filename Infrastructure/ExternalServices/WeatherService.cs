using System.Net;
using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Domain.Exceptions;
using Infrastructure.ExternalServices.Models;
using Infrastructure.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices
{
    public class WeatherService : IClimaService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenWeatherOptions _options;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient, IOptions<OpenWeatherOptions> options, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<ClimaResponseDTO> ObterClimaPorCoordenadas(
            CoordenadasRequestDTO coordenadas, 
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Iniciando chamada à API externa para obter clima por coordenadas. Latitude: {Latitude}, Longitude: {Longitude}", 
                coordenadas.Latitude, 
                coordenadas.Longitude);

            Dictionary<string, string?> queryParameters = new Dictionary<string, string?>
            {
                ["lat"] = coordenadas.Latitude.ToString(),
                ["lon"] = coordenadas.Longitude.ToString(),
                ["appid"] = _options.ApiKey,
                ["units"] = _options.Unit,
                ["lang"] = _options.Lang
            };

            string url = QueryHelpers.AddQueryString(_options.BaseUrl, queryParameters);

            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Erro na chamada à API externa. StatusCode: {StatusCode}, Latitude: {Latitude}, Longitude: {Longitude}", 
                    response.StatusCode, 
                    coordenadas.Latitude, 
                    coordenadas.Longitude);
                    
                throw new HttpRequestException("Erro ao obter dados do clima");
            }

            string json = await response.Content.ReadAsStringAsync(cancellationToken);

            OpenWeatherResponse? weatherResponse = JsonSerializer.Deserialize<OpenWeatherResponse>(json) 
                ?? throw new InvalidOperationException("Falha ao desserializar dados da API");

            _logger.LogInformation(
                "Clima obtido com sucesso por coordenadas. Cidade: {Cidade}, Temperatura: {Temperatura}°C", 
                weatherResponse.Name ?? "Localização Desconhecida", 
                weatherResponse.Main.Temp);

            CidadeResponseDTO cidade = new(
                Nome: !string.IsNullOrEmpty(weatherResponse.Name) 
                    ? weatherResponse.Name 
                    : "Localização Desconhecida",
                Pais: !string.IsNullOrEmpty(weatherResponse.Sys.Country) 
                    ? weatherResponse.Sys.Country 
                    : "Localização Desconhecida"
            );

            return new ClimaResponseDTO(
                Cidade: cidade,
                Latitude: weatherResponse.Coord.Lat,
                Longitude: weatherResponse.Coord.Lon,
                Temperatura: weatherResponse.Main.Temp,
                TemperaturaMin: weatherResponse.Main.TempMin,
                TemperaturaMax: weatherResponse.Main.TempMax,
                Condicao: weatherResponse.Weather.FirstOrDefault()?.Description ?? "Desconhecido",
                DataHoraRegistro: DateTime.Now
            );
        }

        public async Task<ClimaResponseDTO> ObterClimaPorCidade(
            CidadeRequestDTO cidade, 
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Iniciando chamada à API externa para obter clima por cidade. Cidade: {Cidade}", 
                cidade.Nome);

            var queryParameters = new Dictionary<string, string?>
            {
                ["q"] = cidade.Nome,
                ["appid"] = _options.ApiKey,
                ["units"] = _options.Unit,
                ["lang"] = _options.Lang
            };

            string url = QueryHelpers.AddQueryString(_options.BaseUrl, queryParameters);


                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    
                    _logger.LogWarning(
                        "Cidade não encontrada na API externa. Cidade: {Cidade}, StatusCode: {StatusCode}", 
                        cidade.Nome, 
                        response.StatusCode);
                    
                    throw new NegocioException(
                        $"Cidade não encontrada: {response.StatusCode}. Detalhes: {content}"
                    );
                }

                if (!response.IsSuccessStatusCode)
                {
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    
                    _logger.LogError(
                        "Erro na chamada à API externa. StatusCode: {StatusCode}, Cidade: {Cidade}, Detalhes: {Detalhes}", 
                        response.StatusCode, 
                        cidade.Nome, 
                        content);
                    
                    throw new HttpRequestException(
                        $"Erro ao consultar API: {response.StatusCode}. Detalhes: {content}"
                    );
                }

                string json = await response.Content.ReadAsStringAsync(cancellationToken);

                OpenWeatherResponse? weatherResponse = JsonSerializer.Deserialize<OpenWeatherResponse>(json) 
                    ?? throw new InvalidOperationException("Falha ao desserializar dados da API");

                _logger.LogInformation(
                    "Clima obtido com sucesso por cidade. Cidade: {Cidade}, País: {Pais}, Temperatura: {Temperatura}°C", 
                    weatherResponse.Name, 
                    weatherResponse.Sys.Country, 
                    weatherResponse.Main.Temp);

                CidadeResponseDTO cidadeResponse = new(
                    Nome: weatherResponse.Name,
                    Pais: weatherResponse.Sys.Country
                );

                return new ClimaResponseDTO(
                    Cidade: cidadeResponse,
                    Latitude: weatherResponse.Coord.Lat,
                    Longitude: weatherResponse.Coord.Lon,
                    Temperatura: weatherResponse.Main.Temp,
                    TemperaturaMin: weatherResponse.Main.TempMin,
                    TemperaturaMax: weatherResponse.Main.TempMax,
                    Condicao: weatherResponse.Weather.FirstOrDefault()?.Description ?? "Desconhecido",
                    DataHoraRegistro: DateTime.Now
                );
        }
    }
}
