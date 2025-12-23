using Propeller.Entities;

namespace Propeller.DALC.Interfaces
{
    public interface ICustomerStatusRepository
    {
        Task<IEnumerable<CustomerStatus>> RetrieveStatusesAsync();

        Task<bool> ValidateStatusExists(int statusId);
    }
}
