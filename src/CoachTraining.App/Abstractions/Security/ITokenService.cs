using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Abstractions.Security;

public interface ITokenService
{
    TokenResult GerarToken(Professor professor);
}
