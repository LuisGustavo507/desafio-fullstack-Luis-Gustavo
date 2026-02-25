using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases
{
    public class CriarUsuarioUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CriarUsuarioUseCase> _logger;

        public CriarUsuarioUseCase(
            IUserRepository userRepository,
            IPasswordHashService passwordHashService,
            IUnitOfWork unitOfWork,
            ILogger<CriarUsuarioUseCase> logger)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Executar(UserRequestDTO userRequest, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Iniciando UseCase CriarUsuario. Usuário: {Nome}",
                userRequest.Nome);

            try
            {
                

                User? usuarioExistente = await _userRepository.ObterPorNomeAsync(userRequest.Nome, cancellationToken);

                if (usuarioExistente != null)
                {
                    _logger.LogWarning(
                        "Tentativa de criar usuário com nome duplicado: {Nome}",
                        userRequest.Nome);

                    return false;
                }

                string senhaHash = _passwordHashService.HashPassword(userRequest.Senha);

                User novoUsuario = new User(userRequest.Nome, senhaHash);

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                await _userRepository.AdicionarAsync(novoUsuario, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Usuário criado com sucesso: {Nome}",
                    userRequest.Nome);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao criar usuário: {Nome}",
                    userRequest.Nome);

                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}