namespace CoachTraining.App.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEmUtc { get; set; }
}
