using Propeller.Entities;

namespace Propeller.DALC.Interfaces
{
    public interface IUsersRepository
    {
        Task<User?> ValidateUser(string userName, string password);
    }
}
