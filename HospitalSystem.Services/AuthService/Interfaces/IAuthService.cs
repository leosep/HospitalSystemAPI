namespace HospitalSystem.Services.AuthService.Interfaces
{
    public interface IAuthService
    {
        Task<string?> AuthenticateUserAsync(string username, string password);
        string GenerateJwtToken(string userId, string username);
    }
}
