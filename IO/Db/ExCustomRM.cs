using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Transactions;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace ExamRef.ExamLib.IO.Db
{
    public class ExCustomRM
    {
        public VolatileRM Ex1_CustomRM()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TSQL2012Local"].ConnectionString;
            string connectionString2 = ConfigurationManager.ConnectionStrings["ScratchDBLocal"].ConnectionString;

            string cmd = @"INSERT INTO [dbo].[Test01] ([SVAL], [NVAL])
                             VALUES (@pSVAL , @pNVAL)";
            string cmd2 = @"INSERT INTO [dbo].[Test01] ([SVAL], [NVAL])
                             VALUES (@pSVAL , @pNVAL)";

            var vrm = new VolatileRM("RM1");
            Console.WriteLine("Member Value:" + vrm.MemberValue);

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

                    // DTC enlisted here
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

                    vrm.MemberValue = 3;

                    // hack rollback
                    //throw new InvalidOperationException("ERROR: Simulate Rollback.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // post-transaction
            Console.WriteLine("Member Value:" + vrm.MemberValue);
            return vrm;
        }
    }

    // Customized resource manager. Allows this .NET object to participate in
    // transactions within a TransactionScope.
    // This RM doesn't force promotion to DTC even when used with a database
    // RM.
    public class VolatileRM : IEnlistmentNotification
    {
        private string _whoAmI = "";
        private int _memberValue = 0;
        private int _oldMemberValue = 0;

        public VolatileRM(string whoAmI)
        {
            _whoAmI = whoAmI;
        }

        public int MemberValue
        {
            get
            {
                return _memberValue;
            }

            set
            {
                Transaction tran = Transaction.Current;
                if (tran != null)
                {
                    Console.WriteLine(_whoAmI + ": MemberValue setter - EnlistVolatile");
                    tran.EnlistVolatile(this, EnlistmentOptions.None);
                }
                _oldMemberValue = _memberValue;
                _memberValue = value;
            }
        }

        #region IEnlistmentNotification Members

        public void Commit(Enlistment enlistment)
        {
            Console.WriteLine(_whoAmI + ": Commit");
            // Clear out _oldMemberValue
            _oldMemberValue = 0;
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            Console.WriteLine(_whoAmI + ": InDoubt");
            enlistment.Done();
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            Console.WriteLine(_whoAmI + ": Prepare");
            preparingEnlistment.Prepared();
        }

        public void Rollback(Enlistment enlistment)
        {
            Console.WriteLine(_whoAmI + ": Rollback");
            // Restore previous state
            _memberValue = _oldMemberValue;
            _oldMemberValue = 0;
            enlistment.Done();
        }
        #endregion
    }
}
