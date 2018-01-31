using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ExamRef.ExamLib.Serialization
{
    public class Person101
    {
        public int PersonID { get; set; }
        public string Name { get; set; }
        public bool Registered { get; set; }
    }

    public class ExJavaScriptSerializer
    {
        public void Ex1_Json()
        {
            var RegisteredUsers = new List<Person101>();
            RegisteredUsers.Add(new Person101() { PersonID = 1, Name = "Bryon Hetrick", Registered = true });
            RegisteredUsers.Add(new Person101() { PersonID = 2, Name = "Nicole Wilcox", Registered = true });
            RegisteredUsers.Add(new Person101() { PersonID = 3, Name = "Adrian Martinson", Registered = false });
            RegisteredUsers.Add(new Person101() { PersonID = 4, Name = "Nora Osborn", Registered = false });

            var serializer = new JavaScriptSerializer();
            var serializedResult = serializer.Serialize(RegisteredUsers);

            Console.WriteLine(serializedResult);
            // Produces string value of:
            // [
            //     {"PersonID":1,"Name":"Bryon Hetrick","Registered":true},
            //     {"PersonID":2,"Name":"Nicole Wilcox","Registered":true},
            //     {"PersonID":3,"Name":"Adrian Martinson","Registered":false},
            //     {"PersonID":4,"Name":"Nora Osborn","Registered":false}
            // ]

            var deserializedResult = serializer.Deserialize<List<Person101>>(serializedResult);

            foreach (var p in deserializedResult)
            {
                Console.WriteLine("{0}, {1}, {2}", p.PersonID, p.Name, p.Registered);
            }
        }
    }
}
