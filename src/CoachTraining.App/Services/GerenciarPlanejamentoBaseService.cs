using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs;

namespace CoachTraining.App.Services;

public class GerenciarPlanejamentoBaseService
{
    private readonly IAtletaRepository _atletaRepository;

    public GerenciarPlanejamentoBaseService(IAtletaRepository atletaRepository)
    {
        _atletaRepository = atletaRepository ?? throw new ArgumentNullException(nameof(atletaRepository));
    }

    public PlanejamentoBaseDto? ObterPorAtletaId(Guid atletaId, Guid professorId)
    {
        if (atletaId == Guid.Empty || professorId == Guid.Empty)
        {
            return null;
        }

        var atleta = _atletaRepository.ObterPorId(atletaId, professorId);
        if (atleta?.TreinosPlanejadosPorSemana is not int treinosPlanejadosPorSemana)
        {
            return null;
        }

        return new PlanejamentoBaseDto
        {
            AtletaId = atletaId,
            TreinosPlanejadosPorSemana = treinosPlanejadosPorSemana
        };
    }

    public PlanejamentoBaseDto Salvar(Guid atletaId, SalvarPlanejamentoBaseDto dto, Guid professorId)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (atletaId == Guid.Empty)
        {
            throw new ArgumentException("AtletaId invalido.", nameof(atletaId));
        }

        if (professorId == Guid.Empty)
        {
            throw new ArgumentException("ProfessorId invalido.", nameof(professorId));
        }

        var atleta = _atletaRepository.ObterPorId(atletaId, professorId);
        if (atleta == null)
        {
            throw new UnauthorizedAccessException("Atleta nao encontrado para o professor autenticado.");
        }

        atleta.DefinirTreinosPlanejadosPorSemana(dto.TreinosPlanejadosPorSemana);
        _atletaRepository.AtualizarPlanejamentoBase(atletaId, professorId, atleta.TreinosPlanejadosPorSemana!.Value);

        return new PlanejamentoBaseDto
        {
            AtletaId = atletaId,
            TreinosPlanejadosPorSemana = atleta.TreinosPlanejadosPorSemana.Value
        };
    }
}
