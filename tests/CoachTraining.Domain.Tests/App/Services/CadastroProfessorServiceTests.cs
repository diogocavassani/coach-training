using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;

namespace CoachTraining.Tests.App.Services;

public class CadastroProfessorServiceTests
{
    private sealed class ProfessorRepositoryFake : IProfessorRepository
    {
        private readonly Dictionary<string, Professor> _professores = new();

        public void Adicionar(Professor professor)
            => _professores[professor.Email] = professor;

        public bool ExistePorEmail(string email)
            => _professores.ContainsKey(Professor.NormalizarEmail(email));

        public Professor? ObterPorEmail(string email)
            => _professores.TryGetValue(Professor.NormalizarEmail(email), out var professor) ? professor : null;
    }

    private sealed class PasswordHasherFake : IPasswordHasher
    {
        public string Hash(string senha) => $"hash::{senha}";
        public bool Verify(string senha, string senhaHash) => senhaHash == $"hash::{senha}";
    }

    [Fact]
    public void Cadastrar_ComDadosValidos_DeveCriarProfessor()
    {
        var service = new CadastroProfessorService(new ProfessorRepositoryFake(), new PasswordHasherFake());
        var dto = new CriarProfessorDto
        {
            Nome = "Professor Teste",
            Email = "professor@teste.com",
            Senha = "123456"
        };

        var resultado = service.Cadastrar(dto);

        Assert.NotEqual(Guid.Empty, resultado.Id);
        Assert.Equal("Professor Teste", resultado.Nome);
        Assert.Equal("professor@teste.com", resultado.Email);
    }

    [Fact]
    public void Cadastrar_ComEmailDuplicado_DeveLancarExcecao()
    {
        var repository = new ProfessorRepositoryFake();
        var service = new CadastroProfessorService(repository, new PasswordHasherFake());

        service.Cadastrar(new CriarProfessorDto
        {
            Nome = "Professor 1",
            Email = "professor@teste.com",
            Senha = "123456"
        });

        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.Cadastrar(new CriarProfessorDto
            {
                Nome = "Professor 2",
                Email = "professor@teste.com",
                Senha = "abcdef"
            }));

        Assert.Contains("Email ja cadastrado", exception.Message);
    }

    [Fact]
    public void Cadastrar_ComSenhaCurta_DeveLancarExcecao()
    {
        var service = new CadastroProfessorService(new ProfessorRepositoryFake(), new PasswordHasherFake());

        Assert.Throws<ArgumentException>(() => service.Cadastrar(new CriarProfessorDto
        {
            Nome = "Professor Teste",
            Email = "professor@teste.com",
            Senha = "12345"
        }));
    }
}
