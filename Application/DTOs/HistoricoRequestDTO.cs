using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    [HistoricoRequestDTOValidation]
    public record HistoricoRequestDTO(
        decimal? Latitude,

        decimal? Longitude,

        [StringLength(100, ErrorMessage = "O nome da cidade deve ter até 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "O nome da cidade deve conter apenas letras e espaços.")]
        string? NomeCidade
    );

    public class HistoricoRequestDTOValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            HistoricoRequestDTO? objRequest = value as HistoricoRequestDTO;

            if (objRequest == null)
                return ValidationResult.Success;

            bool blnTemNome = !string.IsNullOrWhiteSpace(objRequest.NomeCidade);
            bool blnTemLatitude = objRequest.Latitude.HasValue;
            bool blnTemLongitude = objRequest.Longitude.HasValue;

            // Se tem nome, não pode ter latitude ou longitude
            if (blnTemNome && (blnTemLatitude || blnTemLongitude))
                return new ValidationResult("Informe apenas o nome da cidade ou apenas coordenadas (latitude e longitude), não ambos.");

            // Se tem latitude, deve ter longitude (e vice-versa)
            if ((blnTemLatitude || blnTemLongitude) && !(blnTemLatitude && blnTemLongitude))
                return new ValidationResult("Latitude e longitude devem ser informadas juntas.");

            return ValidationResult.Success;
        }
    }
}