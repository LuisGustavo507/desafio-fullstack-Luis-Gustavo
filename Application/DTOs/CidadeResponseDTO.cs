using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public record CidadeResponseDTO(
        [property: JsonPropertyName("nome")]
        string Nome,
        
        [property: JsonPropertyName("pais")]
        string Pais
    );
}
