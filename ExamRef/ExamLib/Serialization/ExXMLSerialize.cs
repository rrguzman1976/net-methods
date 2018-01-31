using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace ExamRef.ExamLib.Serialization
{
    [Serializable]
    public enum PetType { Dog, Cat, Bird, Duck };

    [Serializable]
    public class Pet
    {
        public string Name { get; set; }

        [XmlAttribute]
        public PetType Type { get; set; }    
    }

    [Serializable]
    public class Person33
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        
        [XmlAttribute]
        public bool IsAlive { get; set; }

        [XmlIgnoreAttribute]
        public string SSN { get; set; }

        [XmlArray("Pets")]
        [XmlArrayItem("Pet")]
        public List<Pet> Pets { get; set; }
    }

    public class ExXMLSerialize
    {
        // Roundtrip to disk
        public void Ex1_XMLSerializer()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"Person.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(Person33));

            using (StreamWriter s = File.CreateText(file)) // utf-8
            {
                Person33 p = new Person33
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Age = 42,
                    IsAlive = true,
                    Pets = new List<Pet>
                    {
                        new Pet { Name = "Jerry", Type = PetType.Dog},
                        new Pet { Name = "PJ", Type = PetType.Dog},
                        new Pet { Name = "Trucker", Type = PetType.Dog},
                        new Pet { Name = "Daffy", Type = PetType.Duck}
                    }
                };
                serializer.Serialize(s, p);
            }

            using (StreamReader s = File.OpenText(file)) // utf-8
            {
                Person33 p = (Person33)serializer.Deserialize(s);
                Console.WriteLine("{0} {1} is {2} years old {3}", p.FirstName, p.LastName, p.Age, p.IsAlive ? "and alive" : "and dead");
            }
        }
    }
}
