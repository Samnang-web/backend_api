using Backend.DTOs;
using Backend.Models;
using Backend.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteRepository _conn;
        public NoteController(INoteRepository conn)
        {
            _conn = conn;
        }

        // Return nullable int, null if user is not authenticated or invalid
        private int? GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                return null;

            if (int.TryParse(userIdString, out var userId))
                return userId;

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("User is not authenticated.");

            var notes = await _conn.GetAllNotesByUser(userId.Value);
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("User is not authenticated.");

            var note = await _conn.GetNoteById(id, userId.Value);
            return note == null ? NotFound() : Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NotesDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("User is not authenticated.");

            var note = new Notes
            {
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.Now,
                UserId = userId.Value
            };

            await _conn.CreateNote(note);
            return Ok("Note created");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, NotesDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("User is not authenticated.");

            var existing = await _conn.GetNoteById(id, userId.Value);
            if (existing == null) return NotFound();

            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.UpdatedAt = DateTime.Now;

            await _conn.UpdateNote(existing);
            return Ok("Note updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("User is not authenticated.");

            var result = await _conn.DeleteNote(id, userId.Value);
            return result > 0 ? Ok("Note deleted") : NotFound();
        }
    }
}
