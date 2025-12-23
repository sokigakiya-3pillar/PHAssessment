using Microsoft.EntityFrameworkCore;
using Propeller.DALC.Interfaces;
using Propeller.DALC.Sqlite;
using Propeller.Entities;
using Propeller.Models.Metadata;

namespace Propeller.DALC.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private PropellerDbContext _customerDbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerDbContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CustomerRepository(PropellerDbContext customerDbContext)
        {
            _customerDbContext = customerDbContext ?? throw new ArgumentNullException(nameof(customerDbContext));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<Customer?> RetrieveCustomerAsync(int customerId)
        {
            return await _customerDbContext.Customers.FirstOrDefaultAsync(c => c.ID.Equals(customerId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCustomer"></param>
        /// <returns></returns>
        public async Task<Customer> InsertCustomerAsync(Customer newCustomer)
        {
            newCustomer.CreatedOn = DateTime.UtcNow;
            newCustomer.LastModified = DateTime.UtcNow;

            _customerDbContext.Customers.Add(newCustomer);
            await _customerDbContext.SaveChangesAsync();

            return newCustomer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _customerDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task<int> DeleteCustomerAsync(Customer customer)
        {
            _customerDbContext.Customers.Remove(customer);
            return await _customerDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<(IEnumerable<Customer> customers, PaginationMeta pagination)>
            RetrieveCustomersAsync(string? query, string? sortField, string? sortDirection,
                int pageNumber, int pageSize)
        {

            var tempColl = _customerDbContext.Customers as IQueryable<Customer>;

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim();
                tempColl = tempColl.Where(x => x.Name.ToUpper().Contains(query.ToUpper()));
            }

            string sortingField = "Name"; // Default

            var propertyInfo = typeof(Customer).GetProperty(sortingField);
            tempColl = ConfigureSorting(sortField, sortDirection, tempColl);

            int totalRecords = await tempColl.CountAsync();

            var paginationMeta = new PaginationMeta(totalRecords, pageSize, pageNumber);

            var records = await tempColl
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (records, paginationMeta);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="sortDirection"></param>
        /// <param name="tempColl"></param>
        /// <returns></returns>
        private static IQueryable<Customer> ConfigureSorting(string sortField, string? sortDirection, IQueryable<Customer> tempColl)
        {
            if (!string.IsNullOrEmpty(sortDirection)) // Sort
            {
                if (sortDirection == "a")
                {
                    if (sortField == "n")
                    {
                        tempColl = tempColl.OrderBy(x => x.Name);
                    }
                    else
                    {
                        tempColl = tempColl.OrderBy(x => x.LastModified);
                    }
                }
                else
                {
                    if (sortField == "n")
                    {
                        tempColl = tempColl.OrderByDescending(x => x.Name);
                    }
                    else
                    {
                        tempColl = tempColl.OrderByDescending(x => x.LastModified);
                    }
                }
            }

            return tempColl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contaciId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<Customer>> RetrieveCustomersByContact(int contaciId)
        {
            var customers = await _customerDbContext.Customers.Where(x => x.Contacts.Where(c => c.ID == contaciId).Any()).ToListAsync();
            return customers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerName"></param>
        /// <returns></returns>
        public async Task<Customer?> RetrieveCustomerByNameAsync(string customerName)
        {
            return await _customerDbContext.Customers.Where(x => x.Name.ToUpper().Equals(customerName.ToUpper())).FirstOrDefaultAsync();
        }

    }
}
