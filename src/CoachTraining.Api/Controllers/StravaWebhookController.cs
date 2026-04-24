using System.Text.Json;
using CoachTraining.App.Services.Integrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachTraining.Api.Controllers;

[ApiController]
[Route("api/integrations/strava/webhook/{secret}")]
[AllowAnonymous]
[Produces("application/json")]
public class StravaWebhookController : ControllerBase
{
    private readonly ReceberWebhookStravaService _receberWebhookService;

    public StravaWebhookController(ReceberWebhookStravaService receberWebhookService)
    {
        _receberWebhookService = receberWebhookService ?? throw new ArgumentNullException(nameof(receberWebhookService));
    }

    [HttpGet]
    public IActionResult Validar(
        string secret,
        [FromQuery(Name = "hub.verify_token")] string verifyToken,
        [FromQuery(Name = "hub.challenge")] string challenge)
    {
        if (!_receberWebhookService.ValidarHandshake(secret, verifyToken))
        {
            return Unauthorized();
        }

        return Ok(new Dictionary<string, string> { ["hub.challenge"] = challenge });
    }

    [HttpPost]
    public async Task<IActionResult> Receber(string secret, [FromBody] JsonElement payload)
    {
        try
        {
            await _receberWebhookService.ReceberAsync(secret, payload, HttpContext.RequestAborted);
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}
