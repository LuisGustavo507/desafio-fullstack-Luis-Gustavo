using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public record ClimaResponseDTO(
        [property: JsonPropertyName("cidade")]
        CidadeResponseDTO Cidade,
        
        [property: JsonPropertyName("latitude")]
        decimal Latitude,
        
        [property: JsonPropertyName("longitude")]
        decimal Longitude,
        
        [property: JsonPropertyName("temperatura")]
        double Temperatura,
        
        [property: JsonPropertyName("temperaturaMin")]
        double TemperaturaMin,
        
        [property: JsonPropertyName("temperaturaMax")]
        double TemperaturaMax,
        
        [property: JsonPropertyName("condicao")]
        string Condicao,
        
        [property: JsonPropertyName("dataHora")]
        DateTime DataHoraRegistro
    );
}
