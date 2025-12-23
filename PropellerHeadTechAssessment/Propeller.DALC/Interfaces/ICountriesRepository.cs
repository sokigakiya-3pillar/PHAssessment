using Propeller.Entities;

namespace Propeller.DALC.Interfaces
{
    public interface ICountriesRepository
    {
        Task<IEnumerable<Country>> RetrieveCountries();
    }
}
