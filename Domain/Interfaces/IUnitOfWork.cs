namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICidadeRepository Cidades { get; }
        IClimaRepository Climas { get; }
        IUserRepository Usuarios { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync();
    }
}