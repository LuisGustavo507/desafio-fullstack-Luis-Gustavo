using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases
{
    public class ValidarCredenciaisUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly ILogger<ValidarCredenciaisUseCase> _logger;

        public ValidarCredenciaisUseCase(
            IUserRepository userRepository,
            IPasswordHashService passwordHashService,
            ILogger<ValidarCredenciaisUseCase> logger)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
            _logger = logger;
        }

        public async Task<bool> Executar(UserRequestDTO userRequest, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation( 
                "Iniciando UseCase ValidarCredenciais. Usuário: {Nome}",
                userRequest.Nome);

           User? usuario = await _userRepository.ObterPorNomeAsync(userRequest.Nome, cancellationToken);

            if (usuario == null)
            {
                _logger.LogWarning(
                    "Usuário não encontrado: {Nome}",
                    userRequest.Nome);
                
                return false;
            }

            bool senhaValida = _passwordHashService.VerifyPassword(userRequest.Senha, usuario.Senha);

            if (!senhaValida)
            {
                _logger.LogWarning(
                    "Senha inválida para o usuário: {Nome}",
                    userRequest.Nome);
                
                return false;
            }

            _logger.LogInformation(
                "Credenciais validadas com sucesso para o usuário: {Nome}",
                userRequest.Nome);

            return true;
        }
    }
}