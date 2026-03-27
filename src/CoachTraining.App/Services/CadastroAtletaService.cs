using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs;
using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Services;

public class CadastroAtletaService
{
    private readonly IAtletaRepository _atletaRepository;

    public CadastroAtletaService(IAtletaRepository atletaRepository)
    {
        _atletaRepository = atletaRepository ?? throw new ArgumentNullException(nameof(atletaRepository));
    }

    public AtletaDto Cadastrar(CriarAtletaDto dto, Guid professorId)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "DTO de cadastro nao pode ser nulo");
        }

        if (professorId == Guid.Empty)
        {
            throw new ArgumentException("ProfessorId invalido", nameof(professorId));
        }

        var atleta = new Atleta(
            nome: dto.Nome,
            professorId: professorId,
            observacoesClinicas: dto.ObservacoesClinicas,
            nivelEsportivo: dto.NivelEsportivo
        );

        _atletaRepository.Adicionar(atleta);
        return MapearAtletaParaDto(atleta);
    }

    public AtletaDto? ObterPorId(Guid id, Guid professorId)
    {
        var atleta = ObterEntidadePorId(id, professorId);
        if (atleta == null)
        {
            return null;
        }

        return MapearAtletaParaDto(atleta);
    }

    public Atleta? ObterEntidadePorId(Guid id, Guid professorId)
    {
        if (id == Guid.Empty || professorId == Guid.Empty)
        {
            return null;
        }

        return _atletaRepository.ObterPorId(id, professorId);
    }

    private static AtletaDto MapearAtletaParaDto(Atleta atleta)
    {
        return new AtletaDto
        {
            Id = atleta.Id,
            ProfessorId = atleta.ProfessorId,
            Nome = atleta.Nome,
            ObservacoesClinicas = atleta.ObservacoesClinicas,
            NivelEsportivo = atleta.NivelEsportivo,
            DataCriacao = DateTime.UtcNow
        };
    }
}
