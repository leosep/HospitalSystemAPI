using HospitalSystem.Domain.Entities;

namespace HospitalSystem.DataAccess.Interfaces
{
    public interface IPatientRepository
    {
        Task<PaginatedResult<Patient>> GetAllPatientsAsync(int page, int pageSize);
        Task<Patient> GetPatientByIdAsync(int id);
        Task<int> CreatePatientAsync(Patient patient);
        Task<bool> UpdatePatientAsync(Patient patient);
        Task<bool> DeletePatientAsync(int id);
    }
}
