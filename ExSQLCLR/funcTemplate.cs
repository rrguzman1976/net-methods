using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace ExSQLCLR
{
    public partial class funcTemplate
    {
        [SqlFunction]
        public static SqlDecimal fnMethod1(SqlInt32 fahrenheit)
        {
            return new SqlDecimal((((Int32)fahrenheit) - 32) / 1.8);
        }

        [SqlFunction(
        FillRowMethodName = "FillSplitRow",
        TableDefinition = "[SplitValue] NVARCHAR(100), OriginalValue NVARCHAR(4000)")]
        public static IEnumerable fnSplit(SqlString valuesPacked, SqlString delimiter)
        {
            string[] valuesSplit;
            char[] parms = new char[1] { ';' };

            valuesSplit = valuesPacked.Value.Split(((String)delimiter)[0]);

            object[] compoundArray = new object[3] 
            {
                new object[2] { valuesSplit[0], (String) valuesPacked }
                , new object[2] { valuesSplit[1], (String) valuesPacked }
                , new object[2] { valuesSplit[2], (String) valuesPacked }
            };

            return compoundArray;
        }
        public static void FillSplitRow(object row, ref SqlString splitValue, ref SqlString originalValue)
        {
            object[] rowArray = (object[])row;
            splitValue = new SqlString((string)rowArray[0]);
            originalValue = new SqlString((string)rowArray[1]);
        }
    }
}
