using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations; // EF annotations
using System.Data.Entity; // EF core (DbContext, etc.)

namespace ExamRef.ExamLib.Security.DataIntegrity.EntityFramework
{
    public class ExEntityFramework
    {
        // Ex 1: Use EF to persist objects to database.
        // RKO NOTE: Cannot get EF to recognize a connection string.
        public void Ex1_EFSave()
        {
            using (ShopContext ctx = new ShopContext())
            {
                Address a = new Address
                {
                    AddressLine1 = "Somewhere 1",
                    AddressLine2 = "At some floor",
                    City = "SomeCity",
                    ZipCode = "1111AA"
                };

                Customer c = new Customer()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    BillingAddress = a,
                    ShippingAddress = a,
                };

                ctx.Customers.Add(c);
                ctx.SaveChanges();
            }
        }

        // Ex 2: Use EF to manually validate an object.
        public void Ex2_ManualValidation()
        {
            Address a = new Address
            {
                AddressLine1 = "Somewhere 1",
                AddressLine2 = "At some floor",
                City = "SomeCity",
                ZipCode = "1111AA"
            };

            Customer c = new Customer()
            {
                FirstName = "John",
                //Last = "Doe", // forget last name
                BillingAddress = a,
                ShippingAddress = a,
            };

            var r = GenericValidator<Customer>.Validate(c);

            foreach (var s in r)
            {
                Console.WriteLine("Validation result: {0}", s.ErrorMessage);
            }
        }
    }

    public class Customer
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string FirstName { get; set; }

        [Required, MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        public Address ShippingAddress { get; set; }

        [Required]
        public Address BillingAddress { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string AddressLine1 { get; set; }

        [Required, MaxLength(20)]
        public string AddressLine2 { get; set; }

        [Required, MaxLength(20)]
        public string City { get; set; }

        [RegularExpression(@"^[1-9][0-9]{3}\s?[a-zA-Z]{2}$")]
        public string ZipCode { get; set; }
    }

    public class ShopContext : DbContext
    {
        public IDbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Make sure the database knows how to handle the duplicate address property
            modelBuilder.Entity<Customer>().HasRequired(bm => bm.BillingAddress)
                .WithMany().WillCascadeOnDelete(false);
        }
    }

    public static class GenericValidator<T>
    {
        public static IList<ValidationResult> Validate(T entity)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(entity, null, null);
            Validator.TryValidateObject(entity, context, results);
            return results;
        }
    }
}
