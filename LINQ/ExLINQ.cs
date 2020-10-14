using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.LINQ
{
    public class ExLINQ
    {
        public void Ex1_LINQSelect()
        {
            int[] data = { 1, 2, 5, 8, 11 };

            // Query syntax
            var result = from d in data
                         where d % 2 == 0
                         orderby d descending
                         select d
                         ;

            foreach (int i in result)
            {
                Console.WriteLine(i);
            }
            // Displays 2 8

            // Equivalent to method syntax:
            var result2 = data
                            .Where(d => d % 2 == 0)
                            .OrderByDescending((o) => o)
                            //.Select(s => s) // optional
                            ;

            foreach (int i in result2)
            {
                Console.WriteLine(i);
            }
            // Displays 2 8
        }

        public void Ex2_LINQJoin()
        {
            int[] data1 = { 1, 2, 5 };
            int[] data2 = { 2, 4, 5, 6 };

            var result = from d1 in data1
                         from d2 in data2
                         where d1 == d2
                         select d1 * d2;

            // Same as:
            var resultb = from d1 in data1
                          join d2 in data2
                            on d1 equals d2 // can't use ==
                         select d1 * d2;

            Console.WriteLine(string.Join(", ", resultb)); // Displays 4, 25

            // Method syntax #1.
            var result2 = data1
                            // inner sequence, left comparand, right comparand, result
                            .Join(data2, s => s, t => t, (s, t) => s * t)
                            //.Select(e => e)
                            ;

            Console.WriteLine(string.Join(", ", result2)); // Displays 4, 25

            // Method syntax #2.
            var r3 = data1
                     .SelectMany(n => data2, (n1, n2) =>
                         {
                             return new
                             {
                                 N1 = n1,
                                 N2 = n2,
                                 R = n1 * n2
                             };
                         })
                     .Where(a => a.N1 == a.N2)
                     .Select(a => a.R);

            foreach(var d in r3)
            {
                Console.WriteLine(">>{0}", d);
            }
        }

        public void Ex3_LINQAvg()
        {
            var orders = GetOrders();

            var avgOrderLines = orders.Average(o => o.OrderLines.Count);
            Console.WriteLine("Avg: {0}", avgOrderLines);

            // Write in query syntax?
            var result = (from o in orders
                         //let totalLines = o.OrderLines.Count
                         select o.OrderLines.Count /*totalLines*/).Average();

            Console.WriteLine("Avg: {0}", result);
        }

        public void Ex4_LINQGroup()
        {
            var people = new[] { // NOTE: anonymous collection syntax [] 
                                    new { firstName = "sally", lastName = "johnson" }
                                    , new { firstName = "katie", lastName = "smith" }
                                    , new { firstName = "john", lastName = "smith" }
                                    , new { firstName = "zoe", lastName = "johnson" }
                                };

            // Group by last name. Note, where can be placed after from or group by.
            var grp = from p in people
                      //where p.lastName[0] == 'j'
                      group p by p.lastName into g
                      where g.Key[0] == 'j'
                      orderby g.Key descending
                      select new
                          {
                              key = g.Key
                              , Count = g.Count()
                          };

            foreach (var g in grp)
            {
                Console.WriteLine("Group Syntax: {0}: {1}", g.key, g.Count);
            }

            // Method syntax
            var grp2 = people
                        //.Where(p => p.lastName[0] == 'j')
                        .GroupBy((e) => e.lastName, (e) => e) // key, group
                        .Where(s => s.Key[0] == 'j')
                        .Select(e => new { key = e.Key, Count = e.Count() })
                        .OrderByDescending(e => e.key);

            foreach (var g in grp2)
            {
                Console.WriteLine("Method Syntax: {0}: {1}", g.key, g.Count);
            }
        }

        public List<Order> GetOrders()
        {
            return new List<Order>
            {
                new Order
                {
                    ID = 1
                    , OrderLines = new List<OrderLine>
                    {
                        new OrderLine
                        {
                            Amount = 5
                            , Product = new Product
                            {
                                Price = 1.0m
                                , Description = "Product A"
                            }
                        }
                    }
                }
                , new Order
                {
                    ID = 2
                    , OrderLines = new List<OrderLine>
                    {
                        new OrderLine
                        {
                            Amount = 10
                            , Product = new Product
                            {
                                Price = 5.0m
                                , Description = "Product A"
                            }
                        }
                        , new OrderLine
                        {
                            Amount = 20
                            , Product = new Product
                            {
                                Price = 10.0m
                                , Description = "Product B"
                            }
                        }
                    }
                }
                , new Order
                {
                    ID = 3
                    , OrderLines = new List<OrderLine>
                    {
                        new OrderLine
                        {
                            Amount = 15
                            , Product = new Product
                            {
                                Price = 10.0m
                                , Description = "Product A"
                            }
                        }
                        , new OrderLine
                        {
                            Amount = 30
                            , Product = new Product
                            {
                                Price = 20.0m
                                , Description = "Product B"
                            }
                        }
                        , new OrderLine
                        {
                            Amount = 45
                            , Product = new Product
                            {
                                Price = 30.0m
                                , Description = "Product C"
                            }
                        }
                    }
                }
            };
        }
    }

    public class Product
    {
        public string Description { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderLine
    {
        public int Amount { get; set; }
        public Product Product { get; set; }
    }

    public class Order
    {
        public int ID { get; set; }
        public List<OrderLine> OrderLines { get; set; }
    }
}
