using Infrastructure.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Infrastructure.HealthChecks
{
    public class OpenWeatherApiHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;
        private readonly OpenWeatherOptions _options;

        public OpenWeatherApiHealthCheck(IHttpClientFactory httpClientFactory, IOptions<OpenWeatherOptions> options)
        {
            _httpClient = httpClientFactory.CreateClient();
            _options = options.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                string testUrl = $"{_options.BaseUrl}?lat=0&lon=0&appid={_options.ApiKey}";
                
                HttpResponseMessage response = await _httpClient.GetAsync(testUrl, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("OpenWeather API está respondendo normalmente");
                }

                return HealthCheckResult.Degraded($"OpenWeather API retornou status code {(int)response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                return HealthCheckResult.Unhealthy("OpenWeather API não está acessível", ex);
            }
            catch (TaskCanceledException)
            {
                return HealthCheckResult.Unhealthy("Timeout ao tentar acessar OpenWeather API");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Erro ao verificar saúde da OpenWeather API", ex);
            }
        }
    }
}