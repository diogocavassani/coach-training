using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.DTOs;
using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Services;

public class CadastroProfessorService
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CadastroProfessorService(IProfessorRepository professorRepository, IPasswordHasher passwordHasher)
    {
        _professorRepository = professorRepository ?? throw new ArgumentNullException(nameof(professorRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public ProfessorDto Cadastrar(CriarProfessorDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "DTO de cadastro nao pode ser nulo.");
        }

        if (string.IsNullOrWhiteSpace(dto.Senha) || dto.Senha.Trim().Length < 6)
        {
            throw new ArgumentException("Senha deve ter no minimo 6 caracteres.", nameof(dto.Senha));
        }

        var emailNormalizado = Professor.NormalizarEmail(dto.Email);
        if (_professorRepository.ExistePorEmail(emailNormalizado))
        {
            throw new InvalidOperationException("Email ja cadastrado.");
        }

        var senhaHash = _passwordHasher.Hash(dto.Senha);
        var professor = new Professor(
            nome: dto.Nome,
            email: emailNormalizado,
            senhaHash: senhaHash);

        _professorRepository.Adicionar(professor);

        return new ProfessorDto
        {
            Id = professor.Id,
            Nome = professor.Nome,
            Email = professor.Email,
            DataCriacao = professor.DataCriacao
        };
    }
}
