using HospitalSystem.Domain.Entities;
using HospitalSystem.DataAccess.Interfaces;
using HospitalSystem.Services.PatientService.Interfaces;

namespace HospitalSystem.Services.PatientService
{
    // Business logic
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        /// <summary>
        /// Retrieves a paginated list of all patients.
        /// </summary>
        /// <param name="page">Current page.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>A paginated list of patients.</returns>
        public async Task<PaginatedResult<Patient>> GetAllPatientsAsync(int page, int pageSize)
        {
            return await _patientRepository.GetAllPatientsAsync(page, pageSize);
        }

        /// <summary>
        /// Retrieves a patient by ID.
        /// </summary>
        /// <param name="id">The ID of the patient to retrieve.</param>
        /// <returns>The patient with the specified ID, or null if not found.</returns>
        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            return await _patientRepository.GetPatientByIdAsync(id);
        }

        /// <summary>
        /// Adds a new patient.
        /// </summary>
        /// <param name="patient">The patient to add.</param>
        /// <returns>The ID of the newly added patient.</returns>
        public async Task<int> CreatePatientAsync(Patient patient)
        {
            return await _patientRepository.CreatePatientAsync(patient);
        }

        /// <summary>
        /// Updates an existing patient.
        /// </summary>
        /// <param name="patient">The updated patient data.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            return await _patientRepository.UpdatePatientAsync(patient);
        }

        /// <summary>
        /// Deletes a patient by ID.
        /// </summary>
        /// <param name="id">The ID of the patient to delete.</param>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        public async Task<bool> DeletePatientAsync(int id)
        {
            return await _patientRepository.DeletePatientAsync(id);
        }
    }
}
