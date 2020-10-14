using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Serialization
{
    [Serializable]
    public class Orchestra
    {
        public Instrument[] Instruments;
    }

    [Serializable]
    public class Instrument
    {
        public string Name;
    }

    [Serializable]
    public class Brass : Instrument
    {
        public bool IsValved;
    }

    [Serializable]
    public class ClassRoom
    {
        public string RoomNumber { get; set; }
    }

    public class ExXMLAttributeOverrides
    {
        public void Ex1_AttribOverride()
        {
            SerializeObject("Override.xml");
            DeserializeObject("Override.xml");
        }

        public void SerializeObject(string filename)
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, filename);

            /* Each overridden field, property, or type requires an XmlAttributes object. */
            XmlAttributes attrs = new XmlAttributes();

            /* Create an XmlElementAttribute to override the field that returns Instrument objects. 
             * The overridden field returns Brass objects instead. */
            XmlElementAttribute attr = new XmlElementAttribute();
            attr.ElementName = "Brass";
            attr.Type = typeof(Brass);

            // Add the element to the collection of elements.
            attrs.XmlElements.Add(attr);

            // Create the XmlAttributeOverrides object.
            XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();

            /* Add the type of the class that contains the overridden  member and the XmlAttributes 
             * to override it with to the XmlAttributeOverrides object. */
            attrOverrides.Add(typeof(Orchestra), "Instruments", attrs);

            // Writing the file requires a TextWriter.
            using (TextWriter writer = new StreamWriter(file))
            {
                // Create the XmlSerializer using the XmlAttributeOverrides.
                XmlSerializer s = new XmlSerializer(typeof(Orchestra), attrOverrides);

                // Create the object that will be serialized.
                Orchestra band = new Orchestra();

                Instrument[] myInstruments = 
                {
                    // Create an object of the derived type.
                    new Brass()
                    {
                        Name = "Trumpet"
                        , IsValved = true
                    }
                    /*
                    , new Instrument()
                      {
                        Name = "Violin"
                      }
                      */
                };
                band.Instruments = myInstruments;

                // Serialize the object.
                s.Serialize(writer, band);
            }
        }

        public void DeserializeObject(string filename)
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, filename);

            XmlAttributes attrs = new XmlAttributes();

            // Create an XmlElementAttribute to override the Instrument.
            XmlElementAttribute attr = new XmlElementAttribute();
            attr.ElementName = "Brass";
            attr.Type = typeof(Brass);

            // Add the XmlElementAttribute to the collection of objects.
            attrs.XmlElements.Add(attr);

            XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
            attrOverrides.Add(typeof(Orchestra), "Instruments", attrs);

            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                // Create the XmlSerializer using the XmlAttributeOverrides.
                XmlSerializer s = new XmlSerializer(typeof(Orchestra), attrOverrides);

                Orchestra band = (Orchestra)s.Deserialize(fs);
                Console.WriteLine("Brass:");

                /* The difference between deserializing the overridden XML document and serializing it is this: 
                 * To read the derived object values, you must declare an object of the derived type (Brass), 
                 * and cast the Instrument instance to it. */
                Brass b;

                foreach (Instrument i in band.Instruments)
                {
                    b = (Brass)i;
                    Console.WriteLine(b.Name + "\n" + b.IsValved);
                }
            }
        }

        public void Ex2_AttribReshape()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"ClassRoom.xml");

            XmlAttributes attrs = new XmlAttributes();

            // Create an XmlElementAttribute to override the Instrument.
            XmlAttributeAttribute attr = new XmlAttributeAttribute();
            attr.AttributeName = "Brass";

            // Add the XmlElementAttribute to the collection of objects.
            attrs.XmlAttribute = attr;

            XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
            attrOverrides.Add(typeof(ClassRoom), "RoomNumber", attrs);

            // Writing the file requires a TextWriter.
            using (TextWriter writer = new StreamWriter(file))
            {
                // Create the XmlSerializer using the XmlAttributeOverrides.
                XmlSerializer s = new XmlSerializer(typeof(ClassRoom), attrOverrides);

                // Create the object that will be serialized.
                ClassRoom band = new ClassRoom() { RoomNumber = "1931" };

                // Serialize the object.
                s.Serialize(writer, band);
            }

        }
    }
}
