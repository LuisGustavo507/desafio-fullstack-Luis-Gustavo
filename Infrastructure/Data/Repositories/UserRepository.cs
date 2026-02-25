using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
        {
            User? usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Nome == nome, cancellationToken);
            
            return usuario;
        }

        public async Task AdicionarAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }
    }
}