using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public record CidadeRequestDTO(
        [Required(ErrorMessage = "O nome da cidade não pode ser vazio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "O nome da cidade deve conter apenas letras.")]
        string Nome
    );
}
