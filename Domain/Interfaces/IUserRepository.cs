using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default);
        Task AdicionarAsync(User user, CancellationToken cancellationToken = default);
    }
}