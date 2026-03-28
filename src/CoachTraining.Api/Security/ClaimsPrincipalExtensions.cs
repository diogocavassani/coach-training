using System.Security.Claims;

namespace CoachTraining.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetProfessorId(this ClaimsPrincipal user, out Guid professorId)
    {
        professorId = Guid.Empty;

        var professorIdValue = user.FindFirst("professor_id")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(professorIdValue))
        {
            return false;
        }

        return Guid.TryParse(professorIdValue, out professorId) && professorId != Guid.Empty;
    }
}
