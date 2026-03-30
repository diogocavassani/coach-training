using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.App.Services;

public class CadastrarSessaoDeTreinoService
{
    private readonly IAtletaRepository _atletaRepository;
    private readonly ISessaoDeTreinoRepository _sessaoDeTreinoRepository;

    public CadastrarSessaoDeTreinoService(
        IAtletaRepository atletaRepository,
        ISessaoDeTreinoRepository sessaoDeTreinoRepository)
    {
        _atletaRepository = atletaRepository ?? throw new ArgumentNullException(nameof(atletaRepository));
        _sessaoDeTreinoRepository = sessaoDeTreinoRepository ?? throw new ArgumentNullException(nameof(sessaoDeTreinoRepository));
    }

    public SessaoDeTreinoDto Cadastrar(CadastrarSessaoDeTreinoDto dto, Guid professorId)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (professorId == Guid.Empty)
        {
            throw new ArgumentException("ProfessorId invalido", nameof(professorId));
        }

        var atleta = _atletaRepository.ObterPorId(dto.AtletaId, professorId);
        if (atleta == null)
        {
            throw new UnauthorizedAccessException("Atleta nao encontrado para o professor autenticado");
        }

        var sessao = new SessaoDeTreino(
            atletaId: dto.AtletaId,
            data: dto.Data,
            tipo: dto.Tipo,
            duracaoMinutos: dto.DuracaoMinutos,
            distanciaKm: dto.DistanciaKm,
            rpe: new RPE(dto.Rpe));

        _sessaoDeTreinoRepository.Adicionar(sessao);

        return new SessaoDeTreinoDto
        {
            Id = sessao.Id,
            AtletaId = sessao.AtletaId,
            Data = sessao.Data,
            Tipo = sessao.Tipo,
            DuracaoMinutos = sessao.DuracaoMinutos,
            DistanciaKm = sessao.DistanciaKm,
            Rpe = sessao.Rpe.Valor
        };
    }
}
