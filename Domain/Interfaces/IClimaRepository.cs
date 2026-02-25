using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IClimaRepository
    {
        Task<RegistroClima?> ObterClimaPorCoordenadas(decimal latitude, decimal longitude, CancellationToken cancellationToken = default);
        Task<List<RegistroClima>> ObterHistoricoClima(CancellationToken cancellationToken = default);
        Task<List<RegistroClima>> ObterHistoricoClimaPorCidade(string nomeCidade, CancellationToken cancellationToken = default);
        Task<List<RegistroClima>> ObterHistoricoClimaPorCoordenadas(decimal latitude, decimal longitude, CancellationToken cancellationToken = default);
        Task RegistrarClima(RegistroClima registroClima, CancellationToken cancellationToken = default);
    }
}
