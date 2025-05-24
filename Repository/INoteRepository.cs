using Backend.Models;

namespace Backend.Repository
{
    public interface INoteRepository
    {
        Task<IEnumerable<Notes>> GetAllNotesByUser(int userId);
        Task<Notes> GetNoteById(int id, int userId);
        Task<int> CreateNote(Notes note);
        Task<int> UpdateNote(Notes note);
        Task<int> DeleteNote(int id, int userId);
    }
}
