using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.SqlServer.Server;

namespace ExamRef.ExamLib.IO.Db
{
    public class ExDbTVP
    {
        // Passes a TVP to a sproc
        public void Ex1_SprocTVPParam()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ScratchDBLocal"].ConnectionString;

            string cmd = @"EXEC dbo.spTypeParam
	                        @pTableType = @MyOrderTotalsByYear
	                        , @pCount = @MyCount OUTPUT;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Console.WriteLine("Connection established...");

                SqlCommand command = new SqlCommand(cmd, connection);

                // Single round-trip for a recordset of parameters.
                // Can optionally use a DataTable
                /*
                var tvp = new DataTable();
                tvp.Columns.Add("orderyear", typeof(int));
                tvp.Columns.Add("qty", typeof(int));

                for (int i = 1; i < 5; i++)
                    tvp.Rows.Add(i, i);

                var tvpParam = new SqlParameter("@ObjData",
                     SqlDbType.Structured)
                {
                    TypeName = "dbo.OrderTotalsByYear",
                    Value = tvp
                };

                cmd.Parameters.Add(tvpParam);
                 */

                MyTVPList tvp = new MyTVPList()
                {
                    new MyTVP { orderyear = 1, qty = 1}
                    , new MyTVP { orderyear = 2, qty = 2}
                    , new MyTVP { orderyear = 3, qty = 3}
                    , new MyTVP { orderyear = 4, qty = 4}
                    , new MyTVP { orderyear = 5, qty = 5}
                };

                SqlParameter p0 = new SqlParameter();
                p0.ParameterName = "@MyOrderTotalsByYear";
                p0.SqlDbType = SqlDbType.Structured;
                p0.TypeName = "dbo.OrderTotalsByYear";
                p0.Direction = ParameterDirection.Input;
                p0.Value = tvp;
                command.Parameters.Add(p0);

                SqlParameter p1 = new SqlParameter();
                p1.ParameterName = "@MyCount";
                p1.SqlDbType = SqlDbType.Int;
                p1.Direction = ParameterDirection.Output;
                command.Parameters.Add(p1);

                command.ExecuteNonQuery();

                // Get output/return param values.
                Int32 outval = (Int32)command.Parameters["@MyCount"].Value;

                Console.WriteLine("Out value: {0}", outval);

            }
        }

    } // ExDbTVP

    public class MyTVP
    {
        public int orderyear { get; set; }
        public int qty { get; set; }

    }

    public class MyTVPList : List<MyTVP>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var sdr = new SqlDataRecord(
            new SqlMetaData("orderyear", SqlDbType.Int),
            new SqlMetaData("qty", SqlDbType.Int));

            foreach (MyTVP o in this)
            {
                sdr.SetInt32(0, o.orderyear);
                sdr.SetInt32(1, o.qty);

                yield return sdr;
            }
        }
    }

}
