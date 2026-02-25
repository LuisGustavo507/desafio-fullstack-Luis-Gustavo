using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class CidadeRepository : ICidadeRepository
    {
        private readonly AppDbContext _context;

        public CidadeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cidade?> ObterCidadePorNome(string nome, CancellationToken cancellationToken = default)
        {
            return await _context.Cidades
                .FirstOrDefaultAsync(c => c.Nome == nome, cancellationToken);
        }

        public async Task AdicionarCidade(Cidade cidade, CancellationToken cancellationToken = default)
        {
            await _context.Cidades.AddAsync(cidade, cancellationToken);
        }
    }
}
