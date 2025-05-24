using Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Backend.Repository
{
    public class NoteRepository:INoteRepository
    {
        private readonly IConfiguration _conn;
        public NoteRepository (IConfiguration conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<Notes>> GetAllNotesByUser(int userId)
        {
            using var connection = GetConnection();
            var notes = await connection.QueryAsync<Notes>("SELECT * FROM Notes WHERE UserId=@UserId ORDER BY CreatedAt DESC", new {UserId = userId});
            return notes;
        }

        public async Task<Notes> GetNoteById(int id, int userId)
        {
            using var connection = GetConnection();
            var notes = await connection.QuerySingleOrDefaultAsync<Notes>("SELECT * FROM Notes WHERE Id=@Id AND UserId = @UserId", new {Id = id, UserId = userId});
            return notes;
        }
        public async Task<int> CreateNote(Notes note)
        {
            using var connect = GetConnection();
            var notes = await connect.ExecuteAsync("INSERT INTO Notes (Title, Content, CreatedAt, UserId) VALUES (@Title, @Content, @CreatedAt, @UserId)", note);
            return notes;
        }

        public async Task<int> UpdateNote(Notes note)
        {
            using var connect = GetConnection();
            var notes = await connect.ExecuteAsync("UPDATE Notes SET Title=@Title, Content=@Content, UpdatedAt=@UpdatedAt WHERE Id=@Id AND UserId=@UserId", note);
            return notes;
        }

        public async Task<int> DeleteNote(int id, int userId)
        {
            using var connect = GetConnection();
            var notes = await connect.ExecuteAsync("DELETE FROM Notes WHERE Id=@Id AND UserId=@UserId", new {Id = id, UserId = userId});
            return notes;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_conn.GetConnectionString("DBConnection"));
        }
    }
}
