using Microsoft.EntityFrameworkCore;

namespace Propeller.Entities.Interface
{
    public interface ICustomerDbContext
    {
        DbSet<Customer> Customers { get; set; }
        DbSet<Contact> Contacts { get; set; }
        DbSet<Note> Notes { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Country> Countries { get; set; }
    }
}
