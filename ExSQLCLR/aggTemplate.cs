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
    // Partial class wrapper not needed.
    [Serializable]
    [SqlUserDefinedAggregate(Format.Native)]
    public struct BakersDozen
    {
        private SqlInt32 DonutCount;
        public void Init()
        {
            DonutCount = 0;
        }
        public void Accumulate(SqlInt32 Value)
        {
            DonutCount += Value + ((Int32)Value) / 12;
        }
        public void Merge(BakersDozen Group)
        {
            DonutCount += Group.DonutCount;
        }
        public SqlInt32 Terminate()
        {
            return DonutCount;
        }
    }
}
