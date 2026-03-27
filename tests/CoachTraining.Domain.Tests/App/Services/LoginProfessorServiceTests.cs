using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;

namespace CoachTraining.Tests.App.Services;

public class LoginProfessorServiceTests
{
    private sealed class ProfessorRepositoryFake : IProfessorRepository
    {
        private readonly Dictionary<string, Professor> _professores = new();

        public void Adicionar(Professor professor) => _professores[professor.Email] = professor;

        public bool ExistePorEmail(string email) => _professores.ContainsKey(Professor.NormalizarEmail(email));

        public Professor? ObterPorEmail(string email)
            => _professores.TryGetValue(Professor.NormalizarEmail(email), out var professor) ? professor : null;
    }

    private sealed class PasswordHasherFake : IPasswordHasher
    {
        public string Hash(string senha) => $"hash::{senha}";
        public bool Verify(string senha, string senhaHash) => senhaHash == $"hash::{senha}";
    }

    private sealed class TokenServiceFake : ITokenService
    {
        public TokenResult GerarToken(Professor professor)
        {
            return new TokenResult
            {
                Token = $"token::{professor.Id}",
                ExpiraEmUtc = DateTime.UtcNow.AddHours(8)
            };
        }
    }

    [Fact]
    public void Login_ComCredenciaisValidas_DeveRetornarToken()
    {
        var repo = new ProfessorRepositoryFake();
        var hasher = new PasswordHasherFake();
        var professor = new Professor("Professor", "professor@teste.com", hasher.Hash("123456"));
        repo.Adicionar(professor);
        var service = new LoginProfessorService(repo, hasher, new TokenServiceFake());

        var response = service.Login(new LoginDto
        {
            Email = "professor@teste.com",
            Senha = "123456"
        });

        Assert.StartsWith("token::", response.Token);
    }

    [Fact]
    public void Login_ComCredenciaisInvalidas_DeveLancarUnauthorized()
    {
        var repo = new ProfessorRepositoryFake();
        var hasher = new PasswordHasherFake();
        var professor = new Professor("Professor", "professor@teste.com", hasher.Hash("123456"));
        repo.Adicionar(professor);
        var service = new LoginProfessorService(repo, hasher, new TokenServiceFake());

        Assert.Throws<UnauthorizedAccessException>(() => service.Login(new LoginDto
        {
            Email = "professor@teste.com",
            Senha = "errada"
        }));
    }
}
