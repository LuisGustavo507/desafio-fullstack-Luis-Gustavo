using Application.DTOs;
using Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebClima.Controllers
{
    /// <summary>
    /// Controller responsável por operações relacionadas ao clima
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class WeatherController : ControllerBase
    {
        private readonly ObterClimaPorCoordenadasUseCase _climaCoordenadasUseCase;
        private readonly ObterClimaPorCidadeUseCase _climaCidadeUseCase;
        private readonly ObterHistoricoClimaUseCase _historicoClimaUseCase;
        private readonly ValidarCredenciaisUseCase _validarCredenciaisUseCase;
        private readonly CriarUsuarioUseCase _criarUsuarioUseCase;
        private readonly IConfiguration _config;

        public WeatherController(
            ObterClimaPorCoordenadasUseCase climaCoordenadasUseCase,
            ObterClimaPorCidadeUseCase climaCidadeUseCase,
            ObterHistoricoClimaUseCase historicoClimaUseCase,
            ValidarCredenciaisUseCase validarCredenciaisUseCase,
            CriarUsuarioUseCase criarUsuarioUseCase,
            IConfiguration config)
        {
            _climaCoordenadasUseCase = climaCoordenadasUseCase;
            _climaCidadeUseCase = climaCidadeUseCase;
            _historicoClimaUseCase = historicoClimaUseCase;
            _validarCredenciaisUseCase = validarCredenciaisUseCase;
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _config = config;
        }

        /// <summary>
        /// Obtém e registra as informações climáticas atuais com base em coordenadas geográficas
        /// </summary>
        /// <param name="coordenadas">Objeto com as coordenadas</param>
        /// <param name="cancellationToken">Token de cancelamento para a operação assíncrona</param>
        /// <returns>Dados climáticos atuais da localização especificada</returns>
        /// <response code="200">Dados climáticos obtidos com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="500">Erro interno no servidor</response>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     GET /api/weather/coordenadas?latitude=-23.5505&amp;longitude=-46.6333
        ///     
        /// </remarks>
        [HttpGet("coordenadas")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterClimaPorCoordenadas([FromQuery] CoordenadasRequestDTO coordenadas, CancellationToken cancellationToken)
        {
            return Ok(await _climaCoordenadasUseCase.Executar(coordenadas, cancellationToken));
        }

        /// <summary>
        /// Obtém e registra as informações climáticas atuais com base no nome da cidade
        /// </summary>
        /// <param name="cidade">Objeto contendo o nome da cidade</param>
        /// <param name="cancellationToken">Token de cancelamento para a operação assíncrona</param>
        /// <returns>Dados climáticos atuais da cidade especificada</returns>
        /// <response code="200">Dados climáticos obtidos com sucesso</response>
        /// <response code="400">Nome da cidade inválido ou não encontrado</response>
        /// <response code="500">Erro interno no servidor</response>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     GET /api/weather/cidade?nome=São Paulo
        ///     
        /// O nome da cidade deve conter apenas letras e espaços, com 2 a 100 caracteres.
        /// </remarks>
        [HttpGet("cidade")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterClimaPorCidade([FromQuery] CidadeRequestDTO cidade, CancellationToken cancellationToken)
        {
            return Ok(await _climaCidadeUseCase.Executar(cidade, cancellationToken));
        }

        /// <summary>
        /// Obtém o histórico de registros climáticos por coordenadas geográficas ou nome da cidade
        /// dos últimos 30 dias, ordenados do mais recente para o mais antigo.
        /// </summary>
        /// <param name="requestDto">Objeto contendo latitude/longitude ou nome da cidade</param>
        /// <param name="cancellationToken">Token de cancelamento para a operação assíncrona</param>
        /// <returns>Lista de registros históricos do clima</returns>
        /// <response code="200">Histórico de clima obtido com sucesso</response>
        /// <response code="400">Parâmetros inválidos. É necessário informar coordenadas (lat/lon) ou nome da cidade</response>
        /// <response code="404">Nenhum registro histórico encontrado</response>
        /// <response code="500">Erro interno no servidor</response>
        /// <remarks>
        /// Exemplos de requisição:
        /// 
        ///     GET /api/weather/historico?latitude=-23.5505&amp;longitude=-46.6333
        ///     GET /api/weather/historico?nomeCidade=São Paulo
        ///     
        /// É obrigatório informar ou as coordenadas (latitude e longitude juntas) ou o nome da cidade.
        /// </remarks>
        [HttpGet("historico")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterHistoricoClima([FromQuery] HistoricoRequestDTO? requestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { erro = "Informe latitude e longitude ou o nome da cidade." });
            }
            return Ok(await _historicoClimaUseCase.Executar(requestDto, cancellationToken));
        }

        /// <summary>
        /// Realiza o registro de um novo usuário na aplicação
        /// </summary>
        /// <param name="userRequest">Objeto contendo nome e senha do novo usuário</param>
        /// <param name="cancellationToken">Token de cancelamento para a operação assíncrona</param>
        /// <returns>Mensagem de sucesso ou erro no registro</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos ou usuário já existe</response>
        /// <response code="500">Erro interno no servidor</response>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST /api/weather/registro?nome=novoUsuario&amp;senha=senha123
        ///     
        /// O nome deve ter entre 3 e 100 caracteres e a senha entre 6 e 255 caracteres.
        /// </remarks>
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrarUsuario([FromBody] UserRequestDTO userRequest, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { erro = "Nome e senha são obrigatórios e devem atender aos requisitos de validação." });
            }

            bool usuarioCriado = await _criarUsuarioUseCase.Executar(userRequest, cancellationToken);

            if (!usuarioCriado)
            {
                return BadRequest(new { erro = "Usuário com este nome já existe." });
            }

            return CreatedAtAction(nameof(RegistrarUsuario), new { mensagem = "Usuário registrado com sucesso." });
        }

        /// <summary>
        /// Realiza autenticação do usuário e gera um token JWT
        /// </summary>
        /// <param name="userRequest">Objeto contendo nome e senha do usuário</param>
        /// <param name="cancellationToken">Token de cancelamento para a operação assíncrona</param>
        /// <returns>Token JWT válido por 3 horas</returns>
        /// <response code="200">Autenticação realizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Credenciais inválidas</response>
        /// <response code="500">Erro interno no servidor</response>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST /api/weather/login?nome=usuario&amp;senha=senha123
        ///     
        /// O token JWT gerado é válido por 3 horas e deve ser utilizado no header Authorization.
        /// </remarks>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserRequestDTO userRequest, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { erro = "Nome e senha são obrigatórios." });
            }

            bool credenciaisValidas = await _validarCredenciaisUseCase.Executar(userRequest, cancellationToken);

            if (!credenciaisValidas)
            {
                return Unauthorized(new { erro = "Nome ou senha inválidos." });
            }

            string jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("Configuração 'Jwt:Key' não encontrada");
            string? issuer = _config["Jwt:Issuer"];
            string? audience = _config["Jwt:Audience"];

            Claim[] claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userRequest.Nome),
                new Claim(ClaimTypes.Name, userRequest.Nome),
                new Claim(ClaimTypes.Role, "User")
            };

            byte[] keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            SymmetricSecurityKey key = new SymmetricSecurityKey(keyBytes);
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });
        }
    }
}
