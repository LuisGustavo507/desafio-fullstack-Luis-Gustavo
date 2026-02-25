using Domain.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private IDbContextTransaction? _transaction;
        private ICidadeRepository? _cidadeRepository;
        private IClimaRepository? _climaRepository;
        private IUserRepository? _userRepository;

        public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ICidadeRepository Cidades
        {
            get
            {
                _cidadeRepository ??= new CidadeRepository(_context);
                return _cidadeRepository;
            }
        }

        public IClimaRepository Climas
        {
            get
            {
                _climaRepository ??= new ClimaRepository(_context);
                return _climaRepository;
            }
        }

        public IUserRepository Usuarios
        {
            get
            {
                _userRepository ??= new UserRepository(_context);
                return _userRepository;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                int changesSaved = await _context.SaveChangesAsync(cancellationToken);
                _logger.LogDebug("SaveChanges executado. Registros afetados: {ChangesSaved}", changesSaved);
                return changesSaved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar SaveChangesAsync");
                throw;
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            // InMemory database não suporta transações, então pula se for InMemory
            if (_context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {   
                _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                _logger.LogDebug("Transação iniciada. TransactionId: {TransactionId}", _transaction?.TransactionId);
            }
            else
            {
                _logger.LogDebug("Transação não iniciada (usando InMemory Database)");
            }
        }   

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                    _logger.LogInformation("Transação commitada com sucesso. TransactionId: {TransactionId}", _transaction.TransactionId);
                }
                else
                {
                    _logger.LogDebug("SaveChanges executado sem transação (InMemory Database)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao commitar transação. Executando rollback");
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _logger.LogWarning("Transação revertida (rollback). TransactionId: {TransactionId}", _transaction.TransactionId);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}