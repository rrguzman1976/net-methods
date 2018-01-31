using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server; // in System.Data.dll

namespace ExSQLCLR
{
    public partial class procTemplate
    {
        [SqlProcedure]
        public static void spSelect()
        {
            //SqlContext.Pipe.ExecuteAndSend(new SqlCommand("SELECT * FROM [Production].[Categories];"));
            // Equivalent to:
            using (SqlConnection conn = new SqlConnection("context connection=true")) // within SQLSERVER process
            {
                conn.Open();

                SqlCommand cm = new SqlCommand("SELECT	[ProductCategoryID] AS categoryid, [Name] AS categoryname " +
                                                "FROM[AdventureWorks2017].[Production].[ProductCategory]; ", conn);

                using (SqlDataReader dr = cm.ExecuteReader())
                {
                    // Return to client (i.e. SSMS results window)
                    SqlContext.Pipe.Send("Starting data dump"); // goes to messages window in SSMS

                    //SqlContext.Pipe.Send(dr);
                    // Equivalent to:
                    if (dr.Read())
                    {
                        SqlContext.Pipe.Send("First row"); // goes to messages window in SSMS

                        SqlDataRecord drc = new SqlDataRecord(
                            new SqlMetaData[]
                            {
                                new SqlMetaData("categoryid", SqlDbType.Int)
                                , new SqlMetaData("categoryname", SqlDbType.NVarChar, 15)
                            });

                        drc.SetInt32(0, (int)dr["categoryid"]);
                        drc.SetString(1, (string)dr["categoryname"]);

                        SqlContext.Pipe.SendResultsStart(drc);
                    }

                    while (dr.Read())
                    {
                        SqlDataRecord drc = new SqlDataRecord(
                            new SqlMetaData[]
                            {
                                new SqlMetaData("categoryid", SqlDbType.Int)
                                , new SqlMetaData("categoryname", SqlDbType.NVarChar, 15)
                            });

                        drc.SetInt32(0, (int) dr["categoryid"]);
                        drc.SetString(1, (string) dr["categoryname"]);

                        SqlContext.Pipe.SendResultsRow(drc);
                    }

                    SqlContext.Pipe.SendResultsEnd();

                    SqlContext.Pipe.Send("Data dump complete"); // goes to messages window in SSMS
                }
            }
        }
    }
}
