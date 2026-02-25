using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICidadeRepository
    {
        Task<Cidade?> ObterCidadePorNome(string nome, CancellationToken cancellationToken = default);
        Task AdicionarCidade(Cidade cidade, CancellationToken cancellationToken = default);
    }
}
