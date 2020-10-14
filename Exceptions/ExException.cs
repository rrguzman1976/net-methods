using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ExamRef.ExamLib.Exceptions
{
    public class ExException
    {
        // Ex 1: Use FailFast to prevent finally block from running.
        public void Ex1_FailFast()
        {
            string s = "42";

            try
            {
                int i = int.Parse(s);

                if (i == 42)
                {
                    Environment.FailFast("Special number entered");
                    //Environment.FailFast("Special number entered", new InvalidOperationException("Terminating application");
                }
            }
            catch
            {
                // legal syntax
            }
            finally
            {
                Console.WriteLine("Program complete.");
            }
        }

        // Ex 2: Throw new exception but use InnerException to preserve stack trace
        public void Ex2_InnerException()
        {
            try
            {
                originalMethodSource();
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidTimeZoneException("Wrapper exception", e);
            }
        }

        private void originalMethodSource()
        {
            throw new InvalidOperationException("Base exception");
        }

        // Ex 3: Use ExceptionDispatchInfo
        public void Ex3_ExceptionDispatchInfo()
        {
            ExceptionDispatchInfo possibleException = null;

            try
            {
                string s = "NaN";
                int.Parse(s);
            }
            catch (FormatException ex)
            {
                possibleException = ExceptionDispatchInfo.Capture(ex);
            }

            if (possibleException != null)
            {
                possibleException.Throw();
            }
        }

        // Ex 4: Create custom exception
        public void Ex4_CustomException()
        {
            string basepath = @"..\..\..\TmpDat";
            string filepath = Path.Combine(basepath, "OrderProcessingException.dat");

            // Serialize exception
            OrderProcessingException ope = new OrderProcessingException(7, "Serialize example");
            IFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                formatter.Serialize(fs, ope);
            }

            // Deserialize exception
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                OrderProcessingException opeDerialized = (OrderProcessingException)formatter.Deserialize(fs);
                Console.WriteLine("Exception: {0}", opeDerialized.OrderId);
            }
        }

        // Uses serialization.
        [Serializable]
        public class OrderProcessingException : Exception, ISerializable /* Implicit */
        {
            public OrderProcessingException() : base()
            {
            }

            public OrderProcessingException(int orderId) // custom constructor
                : base()
            {
                OrderId = orderId;
                this.HelpLink = "http://www.mydomain.com/infoaboutexception";
            }

            public OrderProcessingException(int orderId, string message)
                : base(message)
            {
                OrderId = orderId;
                this.HelpLink = "http://www.mydomain.com/infoaboutexception";
            }

            public OrderProcessingException(int orderId, string message, 
                                             Exception innerException)
                : base(message, innerException)
            {
                OrderId = orderId;
                this.HelpLink = "http://www.mydomain.com/infoaboutexception";
            }

            // The special constructor is used to deserialize values.
            protected OrderProcessingException(SerializationInfo info, StreamingContext context)
            {
                try
                {
                    OrderId = (int)info.GetValue("OrderId", typeof(int));
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Exception: {0}", e.Message);
                }
            }

            public int OrderId { get; set; }

            // Populates a SerializationInfo with the data needed to serialize the target object.
            // Base class implements ISerializable so override.
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
	            base.GetObjectData(info, context);
                info.AddValue("OrderId", OrderId, typeof(int));
            }
        }
    }
}
