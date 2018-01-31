using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple=true, Inherited=true)]
    public class DocAttribute : Attribute
    {
        private string _author;
        public string Modified { get; set; }

        public DocAttribute(string author)
        {
            _author = author;
            Modified = DateTime.Now.ToString();
        }
    }

    public class ExAttributeParam
    {
        [DocAttribute(author: "Ricardo Guzman", Modified="20150101")]
        public void Ex1_AttribParam()
        {
            Console.WriteLine("Ex1_AttribParam");
        }
    }
}
