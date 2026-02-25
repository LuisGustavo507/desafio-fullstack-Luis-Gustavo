using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public record CoordenadasRequestDTO(
        [Required(ErrorMessage = "A latitude não pode ser vazia.")]
        [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90.")]
        decimal Latitude,

        [Required(ErrorMessage = "A longitude não pode ser vazia.")]
        [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180.")]
        decimal Longitude
    );
}
