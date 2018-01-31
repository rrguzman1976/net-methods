using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Transactions;

namespace ExamRef.ExamLib.IO.Db
{
    public class ExTransact
    {
        // Same database so doesn't require MSDTC.
        public void Ex1_Transaction()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TSQL2012Local"].ConnectionString;
            string cmd = @"INSERT INTO [dbo].[Test01] ([SVAL], [NVAL])
                             VALUES (@pSVAL , @pNVAL)";
            string cmd2 = @"INSERT INTO [dbo].[Test01] ([SVAL], [NVAL])
                             VALUES (@pSVAL , @pNVAL)";

            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        SqlCommand command1 = new SqlCommand(cmd, connection);
                        command1.Parameters.AddWithValue("@pSVAL", "Txn TEST 1");
                        command1.Parameters.AddWithValue("@pNVAL", "5");

                        SqlCommand command2 = new SqlCommand(cmd2, connection);
                        command2.Parameters.AddWithValue("@pSVAL", "Txn TEST 1b");
                        command2.Parameters.AddWithValue("@pNVAL", "5");

                        int p = command1.ExecuteNonQuery();

                        int q = command2.ExecuteNonQuery();

                        using (TransactionScope ts2 = new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            Console.WriteLine("Inserted: {0}", p + q);
                        }
                    }

                    transactionScope.Complete();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Distributed transaction is automatically reqeusted.
        // Requires MSDTC to be on.
        public void Ex2_DistributedTrans()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TSQL2012Local"].ConnectionString;
            string connectionString2 = ConfigurationManager.ConnectionStrings["DBPREPLocal"].ConnectionString;

            string cmd = @"INSERT INTO [dbo].[Test01] ([SVAL], [NVAL])
                             VALUES (@pSVAL , @pNVAL)";
            string cmd2 = @"INSERT INTO [dbo].[Test02] ([SVAL], [NVAL])
                             VALUES (@pSVAL , @pNVAL)";

            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        SqlCommand command1 = new SqlCommand(cmd, connection);
                        command1.Parameters.AddWithValue("@pSVAL", "Txn TEST 1");
                        command1.Parameters.AddWithValue("@pNVAL", "5");

                        int p = command1.ExecuteNonQuery();

                        using (TransactionScope ts2 = new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            Console.WriteLine("Inserted: {0}", p);
                        }
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString2))
                    {
                        connection.Open();

                        SqlCommand command2 = new SqlCommand(cmd2, connection);
                        command2.Parameters.AddWithValue("@pSVAL", "Txn TEST 1b");
                        command2.Parameters.AddWithValue("@pNVAL", "5");

                        int q = command2.ExecuteNonQuery();

                        using (TransactionScope ts2 = new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            Console.WriteLine("Inserted: {0}", q);
                        }
                    }

                    transactionScope.Complete();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
