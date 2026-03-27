using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.DTOs;
using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Services;

public class LoginProfessorService
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginProfessorService(
        IProfessorRepository professorRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _professorRepository = professorRepository ?? throw new ArgumentNullException(nameof(professorRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public LoginResponseDto Login(LoginDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "DTO de login nao pode ser nulo.");
        }

        var emailNormalizado = Professor.NormalizarEmail(dto.Email);
        var professor = _professorRepository.ObterPorEmail(emailNormalizado);
        if (professor == null || !_passwordHasher.Verify(dto.Senha, professor.SenhaHash))
        {
            throw new UnauthorizedAccessException("Credenciais invalidas.");
        }

        var token = _tokenService.GerarToken(professor);
        return new LoginResponseDto
        {
            Token = token.Token,
            ExpiraEmUtc = token.ExpiraEmUtc
        };
    }
}
