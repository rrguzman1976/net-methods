using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web.Script.Serialization;

namespace ExamRef.ExamLib.JSON
{
    public class ExJSON
    {
        public void Ex1_JSON()
        {
            string json = @"{
                                ""Name"" : ""Bryon Hetrick"",
                                ""Registered"" : true
                            }";

            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<Dictionary<string, object>>(json);

            foreach (var kp in result)
            {
                Console.WriteLine("{0}, {1}", kp.Key, kp.Value);
            }
        }
    }
}
