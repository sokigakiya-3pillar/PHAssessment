using Microsoft.EntityFrameworkCore;
using Propeller.DALC.Interfaces;
using Propeller.DALC.Sqlite;
using Propeller.Entities;
using Propeller.Models.Metadata;

namespace Propeller.DALC.Repositories
{
    public class ContactsRepository : IContactsRepository
    {
        private readonly PropellerDbContext _customerDbContext;

        public ContactsRepository(PropellerDbContext customerDbContext)
        {
            _customerDbContext = customerDbContext ?? throw new ArgumentNullException(nameof(customerDbContext));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newContact"></param>
        /// <returns></returns>
        public async Task<Contact> InsertContactAsync(Contact newContact)
        {
            await _customerDbContext.Contacts.AddAsync(newContact);
            var result = await _customerDbContext.SaveChangesAsync();
            return newContact;
        }

        public async Task<Contact?> RetrieveContact(int contactId)
        {
            return await _customerDbContext.Contacts.Include(x => x.Customers)
                .Where(x => x.ID == contactId).FirstOrDefaultAsync();
        }

        public async Task<List<Contact>> RetrieveContactsByCustomerId(int customerId)
        {
            // TODO: Optimize this query?
            return await _customerDbContext.Contacts
                .Where(x => x.Customers.Where(x => x.ID == customerId).Any()).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<(IEnumerable<Contact> contacts, PaginationMeta pagination)>
            RetrieveContactsAsync(string searchCriteria, string searchField, int pageNumber, int pageSize)
        {
            var tempColl = _customerDbContext.Contacts as IQueryable<Contact>;

            // If no search criteria is specified, we retrieve everything

            if (!string.IsNullOrEmpty(searchCriteria.Trim()))
            {
                if (searchField == "f") // Search only FirstName
                {
                    tempColl = tempColl.Where(x => x.FirstName.ToUpper().Contains(searchCriteria.ToUpper()));
                }
                else if (searchField == "l") // Search only LastName
                {
                    tempColl = tempColl.Where(x => x.LastName.ToUpper().Contains(searchCriteria.ToUpper()));
                }
                else if (searchField == "e") // Search only Email
                {
                    tempColl = tempColl.Where(x => x.EMail.ToUpper().Contains(searchCriteria.ToUpper()));

                }
                else if (searchField == "p") // Search only Phone Number
                {
                    tempColl = tempColl.Where(x => x.PhoneNumber.ToUpper().Contains(searchCriteria.ToUpper()));
                }
                else
                {
                    tempColl = tempColl.Where(x =>
                       x.FirstName.ToUpper().Contains(searchCriteria.ToUpper()) ||
                       x.LastName.ToUpper().Contains(searchCriteria.ToUpper()) ||
                       x.EMail.ToUpper().Contains(searchCriteria.ToUpper()) ||
                       x.PhoneNumber.ToUpper().Contains(searchCriteria.ToUpper())
                   );
                }

                // TODO: remove toUpper and use InvariantCase
            }

            int totalRecords = await tempColl.CountAsync();
            var paginationMeta = new PaginationMeta(totalRecords, pageSize, pageNumber);

            // TODO: Add sorting
            var records = await tempColl
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (records, paginationMeta);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> DeleteContactAsync(Contact contact)
        {
            _customerDbContext.Contacts.Remove(contact);
            return await _customerDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Contact?> RetrieveContact(Contact contact)
        {
            return await _customerDbContext.Contacts.Include(x => x.Customers)
                .Where(x =>
                    x.FirstName == contact.FirstName &&
                    x.LastName == contact.LastName &&
                    x.EMail == contact.EMail &&
                    x.PhoneNumber == contact.PhoneNumber
                ).FirstOrDefaultAsync();
        }

    }
}
