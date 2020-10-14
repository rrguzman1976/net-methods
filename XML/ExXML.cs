using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;

namespace ExamRef.ExamLib.XML
{
    public class ExXML
    {
        public void Ex1_XML()
        {
            string path = @"..\..\..\TmpFile";
            string xsdPath = Path.Combine(path, @"person.xsd");
            string xmlPath = Path.Combine(path, @"person.xml");

            XmlReader reader = XmlReader.Create(xmlPath);
            XmlDocument document = new XmlDocument();
            document.Schemas.Add("", xsdPath);
            document.Load(reader);

            ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);
            document.Validate(eventHandler);
        }

        public void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Console.WriteLine("Error: {0}", e.Message);
                    break;
                case XmlSeverityType.Warning:
                    Console.WriteLine("Warning {0}", e.Message);
                    break;
            }
        }

        public void Ex2_ParseXML()
        {
            string path = @"..\..\..\TmpFile";
            string xmlPath = Path.Combine(path, @"people.xml");
            /*
            string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                            <people>
                              <person firstname=""john"" lastname=""doe"">
                                <contactdetails>
                                  <emailaddress>john@unknown.com</emailaddress>
                                </contactdetails>
                              </person>
                              <person firstname=""jane"" lastname=""doe"">
                                <contactdetails>
                                  <emailaddress>jane@unknown.com</emailaddress>
                                  <phonenumber>001122334455</phonenumber>
                                </contactdetails>
                              </person>
                            </people>";
            */

            using (var stream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read))
            {
                using (XmlReader xmlReader = XmlReader.Create(stream, new XmlReaderSettings() { IgnoreWhitespace = true }))
                {
                    xmlReader.MoveToContent(); // people
                    string val = xmlReader.Name; // people

                    xmlReader.Read(); // person john doe
                    string tmp = xmlReader.Name; // person

                    if (xmlReader.HasAttributes)
                    {
                        Console.WriteLine("Attributes of <" + xmlReader.Name + ">");

                        while (xmlReader.MoveToNextAttribute())
                        {
                            Console.WriteLine(" {0}={1}", xmlReader.Name, xmlReader.Value);
                        }

                        // Move the xmlReader back to the element node.
                        //xmlReader.MoveToElement();
                    }

                    xmlReader.Read(); // contact details

                    tmp = xmlReader.Name; // contact details

                    xmlReader.Read(); // email address element

                    tmp = xmlReader.Name; // email address element

                    xmlReader.Read(); // text value: email address

                    val = xmlReader.Value; // john @

                    xmlReader.Read(); // email address end element
                    tmp = xmlReader.Name; // email address end element

                    xmlReader.Read(); // contact details end element
                    tmp = xmlReader.Name; //  contact details end element

                    xmlReader.Read(); // person end element
                    tmp = xmlReader.Name; // person end element

                    xmlReader.Read(); // person jane doe
                    tmp = xmlReader.Name; // person jane doe

                    if (xmlReader.HasAttributes)
                    {
                        Console.WriteLine("Attributes of <" + xmlReader.Name + ">");

                        while (xmlReader.MoveToNextAttribute())
                        {
                            Console.WriteLine(" {0}={1}", xmlReader.Name, xmlReader.Value);
                        }

                        // Move the xmlReader back to the element node.
                        //xmlReader.MoveToElement();
                    }

                    xmlReader.Read(); // contact details
                    tmp = xmlReader.Name; // contact details

                    xmlReader.Read(); // email address element
                    tmp = xmlReader.Name; // email address element

                    xmlReader.Read(); // text value: email address
                    val = xmlReader.Value; // john @

                    xmlReader.Read(); // email address end element
                    val = xmlReader.Name; // email address end element

                    xmlReader.Read(); // phone number element
                    val = xmlReader.Name; // phone number element

                    xmlReader.Read(); // text value: phone number
                    val = xmlReader.Value; // 011

                    xmlReader.Read(); // phone end element
                    val = xmlReader.Name; // phone end element

                    xmlReader.Read(); // contact details end element
                    val = xmlReader.Name; // contact details end element

                    xmlReader.Read(); // person end element
                    val = xmlReader.Name; // person end element

                    xmlReader.Read(); // people end element
                    val = xmlReader.Name; // people end element

                    if (!xmlReader.Read())
                    {
                        Console.WriteLine("EOL");
                    }

/*                  // Equivalent to:
                    while (xmlReader.Read())
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                Console.WriteLine("Start Element {0}", xmlReader.Name);

                                if (xmlReader.HasAttributes)
                                {
                                    Console.WriteLine("Attributes of <" + xmlReader.Name + ">");
                                    
                                    while (xmlReader.MoveToNextAttribute())
                                    {
                                        Console.WriteLine(" {0}={1}", xmlReader.Name, xmlReader.Value);
                                    }

                                    // Move the xmlReader back to the element node.
                                    //xmlReader.MoveToElement();
                                }

                                break;
                            case XmlNodeType.Text:
                                Console.WriteLine("Text Node: {0}", xmlReader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                Console.WriteLine("End Element {0}", xmlReader.Name);
                                break;
                            default:
                                Console.WriteLine("Other node {0} with value {1}", xmlReader.NodeType, xmlReader.Value);
                                break;
                        }
                    }
 */
                }
            }
        }

        public void Ex3_WriteXML()
        {
            StringWriter stream = new StringWriter();

            using (XmlWriter writer = XmlWriter.Create(stream,
                new XmlWriterSettings() { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("People");
                writer.WriteStartElement("Person");
                writer.WriteAttributeString("firstName", "John");
                writer.WriteAttributeString("lastName", "Doe");
                writer.WriteStartElement("ContactDetails");
                writer.WriteElementString("EmailAddress", "john@unknown.com");
                
                writer.Flush();
            }

            Console.WriteLine(stream.ToString());
        }

        public void Ex3_WriteXMLFile()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"xml1.xml");

            using (StreamWriter stream = File.CreateText(file))
            {
                using (XmlWriter writer = XmlWriter.Create(stream,
                    new XmlWriterSettings() { Indent = true }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("People");
                    writer.WriteStartElement("Person");
                    writer.WriteAttributeString("firstName", "John");
                    writer.WriteAttributeString("lastName", "Doe");
                    writer.WriteStartElement("ContactDetails");
                    writer.WriteElementString("EmailAddress", "john@unknown.com");
                    //writer.WriteEndElement(); // contact details
                    //writer.WriteEndElement(); // person
                    //writer.WriteEndElement(); // people

                    writer.Flush();
                }
            }
        }

        public void Ex4_XMLDoc()
        {
            string path = @"..\..\..\TmpFile";
            string file = Path.Combine(path, @"xml2.xml");

            string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                            <people>
                              <person firstname=""john"" lastname=""doe"">
                                <contactdetails>
                                  <emailaddress>john@unknown.com</emailaddress>
                                </contactdetails>
                              </person>
                              <person firstname=""jane"" lastname=""doe"">
                                <contactdetails>
                                  <emailaddress>jane@unknown.com</emailaddress>
                                  <phonenumber>001122334455</phonenumber>
                                </contactdetails>
                              </person>
                            </people>";

            using (StreamWriter s = File.CreateText(file))
            {
                XmlDocument doc = new XmlDocument();

                doc.LoadXml(xml);
                XmlNodeList nodes = doc.GetElementsByTagName("person");

                // Output the names of the people in the document
                foreach (XmlNode node in nodes)
                {
                    string firstName = node.Attributes["firstname"].Value;
                    string lastName = node.Attributes["lastname"].Value;
                    Console.WriteLine("Name: {0} {1}", firstName, lastName);
                }

                // Start creating a new node
                XmlNode newNode = doc.CreateNode(XmlNodeType.Element, "person", "");

                XmlAttribute firstNameAttribute = doc.CreateAttribute("firstname");
                firstNameAttribute.Value = "Foo";

                XmlAttribute lastNameAttribute = doc.CreateAttribute("lastname");
                lastNameAttribute.Value = "Bar";

                newNode.Attributes.Append(firstNameAttribute);
                newNode.Attributes.Append(lastNameAttribute);

                XmlNode newNode2 = doc.CreateNode(XmlNodeType.Element, "contactdetails", "");
                
                XmlNode newNode3 = doc.CreateNode(XmlNodeType.Element, "emailaddress", "");
                XmlNode newNode3b = doc.CreateNode(XmlNodeType.Text, "emailaddress", "");
                newNode3b.Value = "rrguzman@hotmail.com";
                newNode3.AppendChild(newNode3b);

                XmlNode newNode4 = doc.CreateNode(XmlNodeType.Element, "phonenumber", "");
                XmlNode newNode4b = doc.CreateNode(XmlNodeType.Text, "phonenumber", "");
                newNode4b.Value = "2817725250";
                newNode4.AppendChild(newNode4b);

                newNode2.AppendChild(newNode3);
                newNode2.AppendChild(newNode4);
                newNode.AppendChild(newNode2);

                doc.DocumentElement.AppendChild(newNode);
                Console.WriteLine("Modified xml...");

                doc.Save(s);
            }
        }

        public void Ex5_XPath()
        {
            string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                            <people>
                              <person firstname=""john"" lastname=""doe"">
                                <contactdetails>
                                  <emailaddress>john@unknown.com</emailaddress>
                                </contactdetails>
                              </person>
                              <person firstname=""jane"" lastname=""doe"">
                                <contactdetails>
                                  <emailaddress>jane@unknown.com</emailaddress>
                                  <phonenumber>001122334455</phonenumber>
                                </contactdetails>
                              </person>
                            </people>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XPathNavigator nav = doc.CreateNavigator();
            string query = "//people/person[@firstname='jane']";

            XPathNodeIterator iterator = nav.Select(query);

            Console.WriteLine(iterator.Count); // Displays 1

            while (iterator.MoveNext())
            {
                string firstName = iterator.Current.GetAttribute("firstname", "");
                string lastName = iterator.Current.GetAttribute("lastname", "");

                Console.WriteLine("Name: {0} {1}", firstName, lastName);
            }
        }

        public void Ex6_XElement()
        {
            XElement root = new XElement("Root",
                new XElement("Child1"),
                new XElement("Child2"
                    , "Content"),
                new XElement("Child3"
                    , new XAttribute("Test2", "Value2")),
                new XAttribute(name: "Test", value: "Value")
                );

            Console.WriteLine("{0}", root.ToString());
        }

        public void Ex7_XElement()
        {
            XElement root = new XElement("Root",
                new List<XElement>()
                {
                    new XElement("Child1"),
                    new XElement("Child2"),
                    new XElement("Child3")
                },
                new XAttribute(name: "Test", value: "Value")
                );

            Console.WriteLine("{0}", root.ToString());
        }
    }
}
