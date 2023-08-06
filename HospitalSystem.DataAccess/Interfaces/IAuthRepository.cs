using HospitalSystem.Domain.Entities;

namespace HospitalSystem.DataAccess.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
    }
}
