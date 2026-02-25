using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class ClimaRepository : IClimaRepository
    {
        private readonly AppDbContext _context;

        public ClimaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RegistroClima?> ObterClimaPorCoordenadas(decimal latitude, decimal longitude, CancellationToken cancellationToken = default)
        {
            RegistroClima? registroClima = await _context.Climas
                .FirstOrDefaultAsync(c => c.Latitude == latitude && c.Longitude == longitude, cancellationToken);
            
            return registroClima;
        }

        public async Task<List<RegistroClima>> ObterHistoricoClima(CancellationToken cancellationToken = default)
        {
            List<RegistroClima> registros = await _context.Climas
                .Include(c => c.Cidade)
                .OrderByDescending(c => c.DataHoraRegistro)
                .Take(30)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            return registros;
        }

        public async Task<List<RegistroClima>> ObterHistoricoClimaPorCidade(string nomeCidade, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nomeCidade, nameof(nomeCidade));

            List<RegistroClima> registros = await _context.Climas
                .Include(c => c.Cidade)
                .Where(c => c.Cidade != null && c.Cidade.Nome.ToLower() == nomeCidade.ToLower())
                .OrderByDescending(c => c.DataHoraRegistro)
                .Take(30)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            return registros;
        }

        public async Task<List<RegistroClima>> ObterHistoricoClimaPorCoordenadas(decimal latitude, decimal longitude, CancellationToken cancellationToken = default)
        {
            List<RegistroClima> registros = await _context.Climas
                .Include(c => c.Cidade)
                .Where(c => c.Latitude == latitude && c.Longitude == longitude)
                .OrderByDescending(c => c.DataHoraRegistro)
                .Take(30)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            return registros;
        }

        public async Task RegistrarClima(RegistroClima registroClima, CancellationToken cancellationToken = default)
        {
            await _context.Climas.AddAsync(registroClima, cancellationToken);
        }
    }
}
