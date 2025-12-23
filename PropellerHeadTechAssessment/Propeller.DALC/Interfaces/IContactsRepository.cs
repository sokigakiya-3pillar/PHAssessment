using Propeller.Entities;
using Propeller.Models.Metadata;

namespace Propeller.DALC.Interfaces
{

    public interface IContactsRepository
    {
        Task<Contact> InsertContactAsync(Contact newContact);

        Task<Contact?> RetrieveContact(int contactId);

        Task<Contact?> RetrieveContact(Contact contact);

        Task<List<Contact>> RetrieveContactsByCustomerId(int customerId);

        Task<(IEnumerable<Contact> contacts, PaginationMeta pagination)> 
            RetrieveContactsAsync(string searchCriteria, string searchField, int pageNumber, int pageSize);

        Task<int> DeleteContactAsync(Contact contact);

    }

}
