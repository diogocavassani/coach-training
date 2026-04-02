using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs;
using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Services;

public class GerenciarProvaAlvoService
{
    private readonly IAtletaRepository _atletaRepository;
    private readonly IProvaAlvoRepository _provaAlvoRepository;

    public GerenciarProvaAlvoService(IAtletaRepository atletaRepository, IProvaAlvoRepository provaAlvoRepository)
    {
        _atletaRepository = atletaRepository ?? throw new ArgumentNullException(nameof(atletaRepository));
        _provaAlvoRepository = provaAlvoRepository ?? throw new ArgumentNullException(nameof(provaAlvoRepository));
    }

    public ProvaAlvoDto? ObterPorAtletaId(Guid atletaId, Guid professorId)
    {
        if (atletaId == Guid.Empty || professorId == Guid.Empty)
        {
            return null;
        }

        var atleta = _atletaRepository.ObterPorId(atletaId, professorId);
        if (atleta == null)
        {
            return null;
        }

        var prova = _provaAlvoRepository.ObterPorAtletaId(atletaId, professorId);
        if (prova == null)
        {
            return null;
        }

        return MapearParaDto(atletaId, prova);
    }

    public ProvaAlvoDto Salvar(Guid atletaId, SalvarProvaAlvoDto dto, Guid professorId)
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

        var provaExistente = _provaAlvoRepository.ObterPorAtletaId(atletaId, professorId);
        var prova = new ProvaAlvo(
            dataProva: dto.DataProva,
            distanciaKm: dto.DistanciaKm,
            objetivo: dto.Objetivo,
            id: provaExistente?.Id);

        _provaAlvoRepository.Salvar(atletaId, prova);

        return MapearParaDto(atletaId, prova);
    }

    private static ProvaAlvoDto MapearParaDto(Guid atletaId, ProvaAlvo prova)
    {
        return new ProvaAlvoDto
        {
            Id = prova.Id,
            AtletaId = atletaId,
            DataProva = prova.DataProva,
            DistanciaKm = prova.DistanciaKm,
            Objetivo = prova.Objetivo
        };
    }
}
