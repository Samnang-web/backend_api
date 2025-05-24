using Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _conn;
        public UserRepository(IConfiguration conn)
        {
            _conn = conn;
        }

        public async Task<User> GetByUsername(string username)
        {
            using var connect = GetConnection();
            var users = await connect.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Username = @Username", new { Username = username });
            return users;
        }

        public async Task<int> Create(User user)
        {
            using var connect = GetConnection();
            var users = await connect.ExecuteAsync("INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)", user);
            return users;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_conn.GetConnectionString("DBConnection"));
        }
    }
}
