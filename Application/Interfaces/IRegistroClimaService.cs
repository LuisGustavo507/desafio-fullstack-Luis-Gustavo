using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IRegistroClimaService
    {
        public Task<Cidade?> ObterOuCriarCidade(CidadeResponseDTO cidadeResponse, CancellationToken cancellationToken = default);

        public Task RegistrarClima(ClimaResponseDTO climaResponse, Cidade? cidadeResponse, CancellationToken cancellationToken = default);

    }
}
