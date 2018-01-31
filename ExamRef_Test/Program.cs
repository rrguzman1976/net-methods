#region namespace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Security;
using System.Security.Permissions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ExamRef.ExamLib.Multithreading.Threads;
using ExamRef.ExamLib.Multithreading.Tasks;
using ExamRef.ExamLib.Multithreading.PLINQ;
using ExamRef.ExamLib.Multithreading.ConcurrentCollections;
using ExamRef.ExamLib.Multithreading.Synchronize;
using ExamRef.ExamLib.Events.Delegates;
using ExamRef.ExamLib.Events;
using ExamRef.ExamLib.Exceptions;
using ExamRef.ExamLib.Types;
using ExamRef.ExamLib.Types.ValueType;
using ExamRef.ExamLib.Types.ReferenceType;
using ExamRef.ExamLib.Types.DynamicType;
using ExamRef.ExamLib.Types.Encapsulation;
using ExamRef.ExamLib.Types.ClassHierarchy;
using ExamRef.ExamLib.Types.Strings;
using ExamRef.ExamLib.Security.DataIntegrity.EntityFramework;
using ExamRef.ExamLib.JSON;
using ExamRef.ExamLib.XML;
using ExamRef.ExamLib.Encryption;
using ExamRef.ExamLib.Hashing;
using ExamRef.ExamLib.Certificates;
using ExamRef.ExamLib.Security.CodeAccess;
using ExamRef.ExamLib.Debug;
using ExamRef.ExamLib.IO.Files;
using ExamRef.ExamLib.IO.Streams;
using ExamRef.ExamLib.IO.Db;
using ExamRef.ExamLib.LINQ;
using ExamRef.ExamLib.Serialization;
using ExamRef.ExamLib.Multithreading.SignalWaits;
using ExamRef.ExamLib.Types.Arrays;
using ExamRef.ExamLib.Types.Collections;
using ExamRef.ExamLib.Types.Unmanaged;
#endregion namespace

namespace ExamRef_Test
{
    public class Program 
    {

        public static void Main(string[] args)
        {
            var v = new MyBinaryFormatter();

            v.UnitTest();

            Console.ReadKey();
        }

    }
}
    
