using Dapper;
using Microsoft.Extensions.Configuration;
using HospitalSystem.Domain.Entities;
using Microsoft.Data.SqlClient;
using HospitalSystem.DataAccess.Interfaces;
using System.Data;

namespace HospitalSystem.DataAccess.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnection _dbConnection;

        public AuthRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Gets user by username
        /// </summary>
        /// <param name="username">User's username.</param>
        /// <returns>The user.</returns>
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            const string query = @"SELECT * FROM Users WHERE Username = @Username;";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
        }
    }
}
