using Dapper;
using Microsoft.Extensions.Configuration;
using HospitalSystem.Domain.Entities;
using Microsoft.Data.SqlClient;
using HospitalSystem.DataAccess.Interfaces;
using System.Data;

namespace HospitalSystem.DataAccess.Repositories
{
    // Dapper implementation
    public class PatientRepository : IPatientRepository
    {
        private readonly IDbConnection _dbConnection;

        public PatientRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Gets a paginated list of all patients from the database.
        /// </summary>
        /// <param name="page">Current page.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>A paginated list of patients.</returns>
        public async Task<PaginatedResult<Patient>> GetAllPatientsAsync(int page, int pageSize)
        {
            // Calculate the offset and limit for pagination
            int offset = (page - 1) * pageSize;
            int limit = pageSize;

            // Query the database for the paginated results
            var sql = @"SELECT COUNT(*) FROM Patients;
                        SELECT * FROM Patients ORDER BY Id DESC OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;";
            using (var multi = await _dbConnection.QueryMultipleAsync(sql, new { Offset = offset, Limit = limit }))
            {
                var totalRecords = await multi.ReadFirstOrDefaultAsync<int>();
                var patients = (await multi.ReadAsync<Patient>()).ToList();
                return new PaginatedResult<Patient>(patients, totalRecords, page, pageSize);
            }
        }

        /// <summary>
        /// Gets a patient by ID from the database.
        /// </summary>
        /// <param name="id">The ID of the patient to retrieve.</param>
        /// <returns>The patient with the specified ID, or null if not found.</returns>
        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync<Patient>("SELECT * FROM Patients WHERE Id = @Id",new { Id = id });
        }

        /// <summary>
        /// Adds a new patient to the database.
        /// </summary>
        /// <param name="patient">The patient to add.</param>
        /// <returns>The ID of the newly added patient.</returns>
        public async Task<int> CreatePatientAsync(Patient patient)
        {
            const string query = @"INSERT INTO Patients (Name, Address, PhoneNumber) 
                                       VALUES (@Name, @Address, @PhoneNumber);
                                       SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _dbConnection.ExecuteScalarAsync<int>(query, patient);
        }

        /// <summary>
        /// Updates an existing patient in the database.
        /// </summary>
        /// <param name="patient">The updated patient data.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            const string query = @"UPDATE Patients 
                                       SET Name = @Name, Address = @Address, PhoneNumber = @PhoneNumber 
                                       WHERE Id = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, patient);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Deletes a patient by ID from the database.
        /// </summary>
        /// <param name="id">The ID of the patient to delete.</param>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        public async Task<bool> DeletePatientAsync(int id)
        {
            const string query = @"DELETE FROM Patients WHERE Id = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}

