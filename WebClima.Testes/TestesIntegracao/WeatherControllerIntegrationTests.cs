using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WebClima.Testes.TestesIntegracao
{
    public class WeatherControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private string? _authToken;
        private string? _testUserName;
        private string? _testUserPassword;

        public WeatherControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        /// <summary>
        /// Gera credenciais únicas para cada teste
        /// </summary>
        private void GerarCredenciaisUnicas()
        {
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            _testUserName = $"user_{uniqueId}";
            _testUserPassword = $"Pass_{uniqueId}!123";
        }

        /// <summary>
        /// Cria um usuário de teste e obtém um token JWT válido
        /// </summary>
        private async Task<string> CriarUsuarioEObterToken()
        {
            if (!string.IsNullOrEmpty(_authToken))
            {
                return _authToken;
            }

            GerarCredenciaisUnicas();

            var userRequest = new UserRequestDTO
            {
                Nome = _testUserName,
                Senha = _testUserPassword
            };

            var createUserResponse = await _client.PostAsJsonAsync("/api/weather/registro", userRequest);
            
            if (!createUserResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Falha ao criar usuário de teste: {createUserResponse.StatusCode}");
            }

            var loginResponse = await _client.PostAsJsonAsync("/api/weather/login", userRequest);

            if (!loginResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Falha ao fazer login: {loginResponse.StatusCode}");
            }

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDTO>();

            if (loginResult?.Token == null)
            {
                throw new Exception("Token não recebido na resposta de login");
            }

            _authToken = loginResult.Token;
            return _authToken;
        }

        /// <summary>
        /// Adiciona o token JWT ao header de autorização
        /// </summary>
        private void AdicionarTokenAoHeader()
        {
            if (!string.IsNullOrEmpty(_authToken))
            {
                _client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
            }
        }

        [Fact]
        public async Task ObterClimaPorCoordenadas_DeveRetornar200_EPersistirDadosNoBanco()
        {
            // Arrange
            await CriarUsuarioEObterToken();
            AdicionarTokenAoHeader();

            decimal latitude = -23.5505m;
            decimal longitude = -46.6333m;
            string url = $"/api/weather/coordenadas?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert - Valida StatusCode
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert - Valida retorno JSON
            var climaResponse = await response.Content.ReadFromJsonAsync<ClimaResponseDTO>();
            Assert.NotNull(climaResponse);
            Assert.Equal(latitude, climaResponse.Latitude);
            Assert.Equal(longitude, climaResponse.Longitude);
            Assert.NotNull(climaResponse.Cidade);
            Assert.NotEmpty(climaResponse.Condicao);
            Assert.True(climaResponse.Temperatura > -100 && climaResponse.Temperatura < 100);

            // Assert - Valida persistência no banco em memória
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var registrosClima = await dbContext.Climas
                .Include(c => c.Cidade)
                .Where(c => c.Latitude == latitude && c.Longitude == longitude)
                .ToListAsync();

            Assert.NotEmpty(registrosClima);
            
            var registroClima = registrosClima.First();
            Assert.Equal(latitude, registroClima.Latitude);
            Assert.Equal(longitude, registroClima.Longitude);
            Assert.Equal(climaResponse.Temperatura, registroClima.Temperatura);
            Assert.Equal(climaResponse.Condicao, registroClima.Condicao);

            // Valida que a cidade foi criada/associada
            if (climaResponse.Cidade.Nome != "Localização Desconhecida")
            {
                Assert.NotNull(registroClima.Cidade);
                Assert.Equal(climaResponse.Cidade.Nome, registroClima.Cidade.Nome);
            }
        }

        [Fact]
        public async Task ObterClimaPorCoordenadas_ComCoordenadasInvalidas_DeveRetornar400()
        {
            // Arrange
            await CriarUsuarioEObterToken();
            AdicionarTokenAoHeader();

            decimal latitude = 999m; // Latitude inválida
            decimal longitude = -46.6333m;
            string url = $"/api/weather/coordenadas?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ObterClimaPorCoordenadas_DeveCriarCidadeSeNaoExistir()
        {
            // Arrange
            await CriarUsuarioEObterToken();
            AdicionarTokenAoHeader();

            decimal latitude = -22.9068m;
            decimal longitude = -43.1729m;
            string url = $"/api/weather/coordenadas?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var climaResponse = await response.Content.ReadFromJsonAsync<ClimaResponseDTO>();
            Assert.NotNull(climaResponse);

            // Verifica persistência da cidade
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (climaResponse.Cidade.Nome != "Localização Desconhecida")
            {
                var cidade = await dbContext.Cidades
                    .FirstOrDefaultAsync(c => c.Nome == climaResponse.Cidade.Nome);

                Assert.NotNull(cidade);
                Assert.Equal(climaResponse.Cidade.Pais, cidade.Pais);
            }
        }
    }

    public class LoginResponseDTO
    {
        public string? Token { get; set; }
    }

    public class UserRequestDTO
    {
        public string? Nome { get; set; }
        public string? Senha { get; set; }
    }
}