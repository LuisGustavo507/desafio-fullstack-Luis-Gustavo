using Application.DTOs;

namespace Application.Interfaces
{
    public interface IClimaService
    {
       Task<ClimaResponseDTO> ObterClimaPorCoordenadas(CoordenadasRequestDTO coordenadas, CancellationToken cancellationToken = default);
       Task<ClimaResponseDTO> ObterClimaPorCidade(CidadeRequestDTO cidade, CancellationToken cancellationToken = default);
    }
}
