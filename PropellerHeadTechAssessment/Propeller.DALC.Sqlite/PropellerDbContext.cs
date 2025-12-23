using Microsoft.EntityFrameworkCore;
using Propeller.Entities;
using Propeller.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Propeller.DALC.Sqlite
{
    public class PropellerDbContext : DbContext, ICustomerDbContext
    {
        public PropellerDbContext(DbContextOptions<PropellerDbContext> options)
          : base(options) { }

        public DbSet<Customer> Customers { get; set; } = null!;

        public DbSet<CustomerStatus> CustomerStatuses { get; set; } = null!;

        public DbSet<Contact> Contacts { get; set; } = null!;

        public DbSet<Note> Notes { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Country> Countries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedSampleData(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void SeedSampleData(ModelBuilder modelBuilder)
        {
            // Seed Status catalog
            modelBuilder.Entity<CustomerStatus>().HasData(
                new CustomerStatus { ID = 1, State = "prospective" }, // default
                new CustomerStatus { ID = 2, State = "current" },
                new CustomerStatus { ID = 3, State = "non-active" }
                );

            modelBuilder.Entity<User>().HasData(
                new User { ID = 1, UserName = "admin.en@mail.com", Password = "s3cUrE.p4s5W0Rd.1", Name = "English Administrator", CountryCode = "NZL", Role = 99, Locale = "en-NZ" },
                new User { ID = 2, UserName = "power.en@mail.com", Password = "s3cUrE.p4s5W0Rd.2", Name = "English Power User", CountryCode = "NZL", Role = 98, Locale = "en-NZ" },
                new User { ID = 3, UserName = "user.en@mail.com", Password = "s3cUrE.p4s5W0Rd.3", Name = "English User", CountryCode = "NZL", Role = 1, Locale = "en-NZ" },
                new User { ID = 4, UserName = "admin.es@mail.com", Password = "s3cUrE.p4s5W0Rd.1", Name = "Administrador México", CountryCode="MEX", Role = 99, Locale = "es-MX" },
                new User { ID = 5, UserName = "power.es@mail.com", Password = "s3cUrE.p4s5W0Rd.2", Name = "Usuario Poder México", CountryCode = "MEX", Role = 98, Locale = "es-MX" },
                new User { ID = 6, UserName = "user.es@mail.com", Password = "s3cUrE.p4s5W0Rd.3", Name = "Usuario México", CountryCode = "MEX", Role = 1, Locale = "es-MX" },
                new User { ID = 7, UserName = "admin.fr@mail.com", Password = "s3cUrE.p4s5W0Rd.1", Name = "French Administrateur", CountryCode = "FRA", Role = 99, Locale = "fr-FR" },
                new User { ID = 8, UserName = "power.fr@mail.com", Password = "s3cUrE.p4s5W0Rd.2", Name = "French Power Utilisateur", CountryCode = "FRA", Role = 98, Locale = "fr-FR" },
                new User { ID = 9, UserName = "user.fr@mail.com", Password = "s3cUrE.p4s5W0Rd.3", Name = "French Utilisateur", CountryCode = "FRA", Role = 1, Locale = "fr-FR" }
                );

            modelBuilder.Entity<Country>().HasData(
                new Country { ID = Guid.NewGuid(), Name = "New Zealand", CountryCode = "NZL", DefaultLocale = "en-NZ" },
                new Country { ID = Guid.NewGuid(), Name = "Mexico", CountryCode = "MEX", DefaultLocale = "es-MX" },
                new Country { ID = Guid.NewGuid(), Name = "France", CountryCode = "FRA", DefaultLocale = "fr-FR" }
                );

            // Seed customers for testing filters and pagination
            //modelBuilder.Entity<Customer>().HasData(
            //        new Customer { ID = 1, Name = "Customer One", CustomerStatusID = 1 },
            //        new Customer { ID = 2, Name = "Customer Two", CustomerStatusID = 1 },
            //        new Customer { ID = 3, Name = "Customer Three", CustomerStatusID = 1 },
            //        new Customer { ID = 4, Name = "Customer Four", CustomerStatusID = 1 },
            //        new Customer { ID = 5, Name = "Customer Five", CustomerStatusID = 1 },
            //        new Customer { ID = 6, Name = "Customer Six", CustomerStatusID = 1 },
            //        new Customer { ID = 7, Name = "Customer Seven", CustomerStatusID = 1 },
            //        new Customer { ID = 8, Name = "Customer Nine", CustomerStatusID = 1 },
            //        new Customer { ID = 9, Name = "Customer Ten", CustomerStatusID = 1 },
            //        new Customer { ID = 10, Name = "Customer Eleven", CustomerStatusID = 1 },
            //        new Customer { ID = 11, Name = "Customer Twelve", CustomerStatusID = 1 },
            //        new Customer { ID = 12, Name = "Customer Thirteen", CustomerStatusID = 1 },
            //        new Customer { ID = 13, Name = "Customer Fourteen", CustomerStatusID = 1 }
            //    );

            //modelBuilder.Entity<Note>().HasData(
            //        new Note { CustomerID = 1, ID = 1, Text = "Note 1", TimeStamp = DateTime.UtcNow },
            //        new Note { CustomerID = 1, ID = 2, Text = "Note 2", TimeStamp = DateTime.UtcNow }
            //    );

            // add-migration InitialCustomerVersion
            // update-database
        }
    }
}
