using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public record UserRequestDTO(
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 100 caracteres.")]
        string Nome,

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "A senha deve conter entre 6 e 255 caracteres.")]
        string Senha
    );
}