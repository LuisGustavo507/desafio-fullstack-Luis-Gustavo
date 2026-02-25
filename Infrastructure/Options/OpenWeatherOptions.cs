using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options
{
    public class OpenWeatherOptions
    {
        public const string SectionName = "OpenWeather";

        [Required(ErrorMessage = "ApiKey é obrigatória")]
        public string ApiKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "BaseUrl é obrigatória")]
        [Url(ErrorMessage = "BaseUrl deve ser uma URL válida")]
        public string BaseUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit é obrigatória")]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lang é obrigatória")]
        public string Lang { get; set; } = string.Empty;
    }
}