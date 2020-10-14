using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.LINQ
{
    // More LINQ examples
    public class ExLINQ2
    {
        // Create a data source by using a collection initializer. 
        #region data
        protected enum GradeLevel: Int16 { FirstYear = 1, SecondYear, ThirdYear, FourthYear };
        protected class Student
        {
            public string First { get; set; }
            public string Last { get; set; }
            public int ID { get; set; }
            public GradeLevel Year;
            public List<int> Scores;
        }

        protected List<Student> students = new List<Student>
        {
            new Student {First = "Terry", Last = "Adams", ID = 120, 
                Year = GradeLevel.SecondYear, 
                Scores = new List<int>{ 99, 82, 81, 79}},
            new Student {First = "Fadi", Last = "Fakhouri", ID = 116, 
                Year = GradeLevel.ThirdYear,
                Scores = new List<int>{ 99, 86, 90, 94}},
            new Student {First = "Hanying", Last = "Feng", ID = 117, 
                Year = GradeLevel.FirstYear, 
                Scores = new List<int>{ 93, 92, 80, 87}},
            new Student {First = "Cesar", Last = "Garcia", ID = 114, 
                Year = GradeLevel.FourthYear,
                Scores = new List<int>{ 97, 89, 85, 82}},
            new Student {First = "Debra", Last = "Garcia", ID = 115, 
                Year = GradeLevel.ThirdYear, 
                Scores = new List<int>{ 35, 72, 91, 70}},
            new Student {First = "Hugo", Last = "Garcia", ID = 118, 
                Year = GradeLevel.SecondYear, 
                Scores = new List<int>{ 92, 90, 83, 78}},
            new Student {First = "Sven", Last = "Mortensen", ID = 113, 
                Year = GradeLevel.FirstYear, 
                Scores = new List<int>{ 88, 94, 65, 91}},
            new Student {First = "Claire", Last = "O'Donnell", ID = 112, 
                Year = GradeLevel.FourthYear, 
                Scores = new List<int>{ 75, 84, 91, 39}},
            new Student {First = "Svetlana", Last = "Omelchenko", ID = 111, 
                Year = GradeLevel.SecondYear, 
                Scores = new List<int>{ 97, 92, 81, 60}},
            new Student {First = "Lance", Last = "Tucker", ID = 119, 
                Year = GradeLevel.ThirdYear, 
                Scores = new List<int>{ 68, 79, 88, 92}},
            new Student {First = "Michael", Last = "Tucker", ID = 122, 
                Year = GradeLevel.FirstYear, 
                Scores = new List<int>{ 94, 92, 91, 91}},
            new Student {First = "Eugene", Last = "Zabokritski", ID = 121,
                Year = GradeLevel.FourthYear, 
                Scores = new List<int>{ 96, 85, 91, 60}}
        };
        #endregion

        public void Ex1_Select()
        {
            // Get students whose score on first test > 90.
            var set = from s in students
                      where s.Scores[0] > 90 && s.Scores[3] < 90
                      group s by s.Year into grp
                      orderby grp.Key
                      select new
                      {
                          Key = grp.Key
                          , Group = grp
                      };

            foreach (var s in set)
            {
                Console.WriteLine("{0}", s.Key);

                foreach (var s2 in s.Group)
                {
                    Console.WriteLine("{0}, {1}, {2}", s2.Last, s2.First, s2.Scores[0]);
                }
            }

            // Method syntax equivalent
            var set2 = students
                        .Where(s => s.Scores[0] > 90 && s.Scores[3] < 90)
                        .GroupBy(s => s.Year, s => s) // key, selector
                        .Select(g => new { Key = g.Key, Group = g })
                        .OrderBy(g => g.Key)
                        ;

            foreach (var s in set2)
            {
                Console.WriteLine("{0}", s.Key);

                foreach (var s2 in s.Group)
                {
                    Console.WriteLine("{0}, {1}, {2}", s2.Last, s2.First, s2.Scores[0]);
                }
            }
        }

        public void Ex2_SubQuery()
        {
            // Use a subquery to get the highest score for each group.
            var qry = from s in students
                      //let max = s.Scores.Max() // student max
                      group s by s.Year into grp
                      orderby grp.Key
                      select new
                      {
                          Key = grp.Key,
                          // Subquery produces sequence of average (double)
                          MaxAvg = (from s in grp
                                    select s.Scores.Average()).Max()
                      };

            foreach (var s in qry)
            {
                Console.WriteLine("1: {0} {1}", s.Key, s.MaxAvg);
            }

            // Method syntax equivalent.
            var qry2 = students
                        .GroupBy(s => s.Year, s => s)
                        .OrderBy(s => s.Key)
                        .Select(g => new
                        {
                            Key = g.Key
                            , MaxAvg = g.Select(h => h.Scores.Average()).Max()
                        });

            foreach (var s in qry2)
            {
                Console.WriteLine("2: {0} {1}", s.Key, s.MaxAvg);
            }

            // Use select many to take highest score in each year
            var qry3 = students
                        .GroupBy(s => s.Year, s => s)
                        .OrderBy(s => s.Key)
                        .Select(g => new
                        {
                            Key = g.Key
                            ,
                            MaxAvg = g.SelectMany(h => h.Scores).Max()
                        });

            foreach (var s in qry3)
            {
                Console.WriteLine("3: {0} {1}", s.Key, s.MaxAvg);
            }
        }
    }
}
