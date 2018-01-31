using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Permissions;

namespace ExamRef.ExamLib.Security.CodeAccess
{
    public class ExCodeAccess
    {
        // Declarative
        [FileIOPermission(SecurityAction.Demand
            , AllLocalFiles = FileIOPermissionAccess.Write
            , AllFiles = FileIOPermissionAccess.Read)]
        public void Ex1_Declarative()
        {

        }

        // Equivalent to above
        public void Ex2_Imperative()
        {
            FileIOPermission f = new FileIOPermission(PermissionState.None);
            f.AllLocalFiles = FileIOPermissionAccess.AllAccess;
            f.AllFiles = FileIOPermissionAccess.Read;

            try
            {
                f.Demand();
            }
            catch (SecurityException s)
            {
                Console.WriteLine(s.Message);
            }
        }
    }
}
