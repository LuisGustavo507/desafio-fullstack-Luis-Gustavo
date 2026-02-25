namespace Application.DTOs
{
    public record ErrorResponse(
        int StatusCode,
        string Mensagem,
        List<string>? Detalhes = null
    );
}