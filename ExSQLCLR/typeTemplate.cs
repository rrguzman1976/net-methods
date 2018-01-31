using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace ExSQLCLR
{
    [Serializable]
    [SqlUserDefinedType(Format.Native)]
    public struct typePoint : INullable
    {
        private bool m_Null;
        private double m_x;
        private double m_y;

        public override string ToString()
        {
            if (this.IsNull)
                return "NULL";
            else
                return this.m_x + ":" + this.m_y;
        }

        public bool IsNull
        {
            get
            {
                return m_Null;
            }
        }

        public static typePoint Null
        {
            get
            {
                typePoint pt = new typePoint();
                pt.m_Null = true;
                return pt;
            }
        }

        // Should not be overloaded as SQL SERVER calls this method 
        // implicitly in expressions such as:
        //
        //  DECLARE @t AS dbo.typePoint;
        //  SET @t = '3:5'; -- typePoint.Parse()
        public static typePoint Parse(SqlString s)
        {
            if (s.IsNull)
                return Null;
            else
            {
                //Parse input string here to separate out points
                typePoint pt = new typePoint();
                char[] parms = new char[1];
                parms[0] = ':';
                string str = (string)s;
                string[] xy = str.Split(parms);
                pt.X = double.Parse(xy[0]);
                pt.Y = double.Parse(xy[1]);
                return pt;
            }
        }

        public static double Sum(typePoint p)
        {
            return p.X + p.Y;
        }

        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }
    }
}
