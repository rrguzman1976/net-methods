using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.LINQ
{
    public class ExLINQ2XML
    {
        private string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
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
                                  <person firstname=""jelly"" lastname=""donut"">
                                    <contactdetails>
                                      <emailaddress>jdo@unknown.com</emailaddress>
                                      <phonenumber>001122334455</phonenumber>
                                    </contactdetails>
                                  </person>
                                </people>";

        // Use LINQ to select an XML document.
        public void Ex1_Select()
        {

            XDocument doc = XDocument.Parse(xml);
            IEnumerable<string> p2 = from XElement p in doc.Descendants("person") // type cast filter
                                    where p.Descendants("phonenumber").Any() // only with phone number
                                    let name = p.Attribute("firstname").Value
                                                + " " + p.Attribute("lastname").Value
                                    orderby name
                                    select name;

            foreach (string s in p2)
            {
                Console.WriteLine(s);
            }
        }

        // Use LINQ to transform an XML document.
        public void Ex2_Modify()
        {
            XElement root = XElement.Parse(xml);

            foreach (XElement p in root.Descendants("person"))
            {
                string name = (string)p.Attribute("firstname") + (string)p.Attribute("lastname");
                p.Add(new XAttribute("ismale", name.Contains("john")));
                XElement contactDetails = p.Element("contactdetails");
                if (!contactDetails.Descendants("phonenumber").Any())
                {
                    contactDetails.Add(new XElement("phonenumber", "001122334455"));
                }
            }

            Console.WriteLine("{0}", root.ToString());

            // Equivalent LINQ expression.
            XElement root2 = XElement.Parse(xml);
            XElement newTree = new XElement("people",
                from p in root2.Descendants("person")
                    let name = (string)p.Attribute("firstname") + (string)p.Attribute("lastname")
                    let contactDetails = p.Element("contactdetails")
                select new XElement("person",
                    p.Attributes(),
                    new XAttribute("ismale", name.Contains("john")),
                    new XElement("contactdetails",
                        contactDetails.Element("emailaddress"),
                        contactDetails.Element("phonenumber") // null coalesce
                            ?? new XElement("phonenumber", "112233455")
                    )));

            Console.WriteLine("{0}", newTree.ToString());
        }
    }
}
