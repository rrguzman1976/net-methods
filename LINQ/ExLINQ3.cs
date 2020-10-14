using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.LINQ
{
    public class ExLINQ3
    {
        #region
        List<Person> people = new List<Person> { 
            new Person { FirstName = "Magnus", LastName = "Hedlund" }, 
            new Person { FirstName = "Terry", LastName = "Adams" }, 
            new Person { FirstName = "Charlotte", LastName = "Weiss" }, 
            new Person { FirstName = "Arlene", LastName = "Huff" }
        };
        List<Pet> pets = new List<Pet> { 
            new Pet { Name = "Barley", Owner = new Person { FirstName = "Terry", LastName = "Adams" } },
            new Pet { Name = "Boots", Owner = new Person { FirstName = "Terry", LastName = "Adams" } },
            new Pet { Name = "Whiskers", Owner = new Person { FirstName = "Charlotte", LastName = "Weiss" } },
            new Pet { Name = "Blue Moon", Owner = new Person { FirstName = "Terry", LastName = "Adams" } },
            new Pet { Name = "Daisy", Owner = new Person { FirstName = "Magnus", LastName = "Hedlund" } }
        };

        // Specify the first data source.
        List<Category> categories = new List<Category>()
        { 
            new Category(){Name="Beverages", ID=001},
            new Category(){ Name="Condiments", ID=002},
            new Category(){ Name="Vegetables", ID=003},
            new Category() {  Name="Grains", ID=004},
            new Category() {  Name="Fruit", ID=005},
            new Category() {  Name="Candy", ID=006}
        };

        // Specify the second data source.
        List<Product2> products = new List<Product2>()
        {
           new Product2{Name="Cola",  CategoryID=001},
           new Product2{Name="Tea",  CategoryID=001},
           new Product2{Name="Mustard", CategoryID=002},
           new Product2{Name="Pickles", CategoryID=002},
           new Product2{Name="Carrots", CategoryID=003},
           new Product2{Name="Bok Choy", CategoryID=003},
           new Product2{Name="Peaches", CategoryID=005},
           new Product2{Name="Melons", CategoryID=005},
         };
        #endregion

        public void Ex1_GroupJoin()
        {
            // Create a list where each element is an anonymous type 
            // that contains the person's first name and a collection of  
            // pets that are owned by them. 
            var query = from person in people
                        join pet in pets
                            on new { person.FirstName, person.LastName }
                                equals new { pet.Owner.FirstName, pet.Owner.LastName } 
                                    into gj // into converts this from inner join to group join
                                                // which returns a person even if matched pets 
                                                // are null (left join)
                        select new { OwnerName = person.FirstName, Pets = gj };

            foreach (var v in query)
            {
                // Output the owner's name.
                Console.WriteLine("{0}:", v.OwnerName);
                //Console.WriteLine("{0}:", v.Pets);
                
                // Output each of the owner's pet's names. 
                foreach (Pet pet in v.Pets)
                    Console.WriteLine("  {0}", pet.Name);
            }
        }

        public void Ex2_SortJoin()
        {
            var groupJoinQuery2 =
                 from category in categories
                 join prod in products 
                    on category.ID equals prod.CategoryID 
                        into prodGroup // group join 1 to M
                 orderby category.Name
                 select new
                 {
                     Category = category.Name,
                     // Re-sort group
                     Products = from prod2 in prodGroup
                                orderby prod2.Name
                                select prod2
                 };

            foreach (var productGroup in groupJoinQuery2)
            {
                Console.WriteLine(productGroup.Category);
                foreach (var prodItem in productGroup.Products)
                {
                    Console.WriteLine("  {0,-10} {1}", prodItem.Name, prodItem.CategoryID);
                }
            }

            // Query syntax
            var r2 = categories
                        // join and group
                        .GroupJoin(products, c => c.ID, p => p.CategoryID, (c, g) =>
                            {
                                return new
                                {
                                    Category = c.Name,
                                    Products = g
                                };
                            }
                        )
                        .OrderBy(g => g.Category)
                        // Re-sort group
                        .Select(g =>
                        {
                            return new
                            {
                                Category = g.Category,
                                Products = from prod2 in g.Products
                                           orderby prod2.Name
                                           select prod2
                            };
                        })
                        ;

            foreach (var p in r2)
            {
                Console.WriteLine("** {0}", p.Category);

                foreach (var prodItem in p.Products)
                {
                    Console.WriteLine("  {0,-10} {1}", prodItem.Name, prodItem.CategoryID);
                }
            }
        }

        public void Ex3_NonEquijoin()
        {
            var nonEquijoinQuery =
                from p in products
                let catIds = from c in categories
                             select c.ID
                where catIds.Contains(p.CategoryID)
                select new { Product = p.Name, CategoryID = p.CategoryID };

            Console.WriteLine("Non-equijoin query:");
            foreach (var v in nonEquijoinQuery)
            {
                Console.WriteLine("{0,-5}{1}", v.CategoryID, v.Product);
            }

            // Equivalent
            var nonEquijoinQuery2 =
                from p in products
                join c in categories
                    on p.CategoryID equals c.ID // order of aliases matters here!
                select new { Product = p.Name, CategoryID = c.ID };

            Console.WriteLine("Equijoin query: left join");
            foreach (var v in nonEquijoinQuery2)
            {
                Console.WriteLine("{0,-5}{1}", v.CategoryID, v.Product);
            }

            // Left outer join
            var nonEquijoinQuery3 =
                from c in categories
                join p in products
                    on c.ID equals p.CategoryID // order of aliases matters here!
                    into gj
                from subproduct in gj.DefaultIfEmpty()
                select new { CategoryID = c.ID, Product = subproduct?.Name ?? String.Empty };

            Console.WriteLine("Equijoin query: right join");
            foreach (var v in nonEquijoinQuery3)
            {
                Console.WriteLine("{0,-5}{1}", v.CategoryID, v.Product);
            }
        }
    }

    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    class Pet
    {
        public string Name { get; set; }
        public Person Owner { get; set; }
    }

    class Product2
    {
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public override string ToString()
        {
            return String.Format("{0} {1}", Name, CategoryID);
        }
    }

    class Category
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public override string ToString()
        {
            return String.Format("{0} {1}", Name, ID);
        }
    }
}
