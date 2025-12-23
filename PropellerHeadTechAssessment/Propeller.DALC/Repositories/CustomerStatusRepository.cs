using Microsoft.EntityFrameworkCore;
using Propeller.DALC.Interfaces;
using Propeller.DALC.Sqlite;
using Propeller.Entities;

namespace Propeller.DALC.Repositories
{
    public class CustomerStatusRepository : ICustomerStatusRepository
    {
        private PropellerDbContext _customerDbContext;

        public CustomerStatusRepository(PropellerDbContext customerDbContext)
        {
            _customerDbContext = customerDbContext ?? throw new ArgumentNullException(nameof(customerDbContext));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<CustomerStatus>> RetrieveStatusesAsync()
        {
            return await _customerDbContext.CustomerStatuses.ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public async Task<bool> ValidateStatusExists(int statusId)
        {
            return await _customerDbContext.CustomerStatuses.Where(x=>x.ID.Equals(statusId)).AnyAsync();
        }
    }
}
