using Propeller.Entities;

namespace Propeller.DALC.Interfaces
{
    public interface INotesRepository
    {

        Task<IEnumerable<Note>> RetrieveNotesAsync(int customerID);

        Task<Note?> RetrieveNoteAsync(int customerID, int noteID);

        Task<bool> DeleteNoteAsync(int customerID, int noteID);

        Task<bool> DeleteNotesAsync(int customerID);

        Task<Note> InsertNoteAsync(Note newNote);

        Task<int> SaveChangesAsync();
    }
}
