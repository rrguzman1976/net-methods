using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;

namespace ExamRef.ExamLib.Types.ClassHierarchy
{
    public class ExReflection
    {
        // Ex 1: Inspect Type
        public void Ex1_Type()
        {
            int i = 21;
            Type t = i.GetType();

            Console.WriteLine("Type: {0}", t.ToString());
        }
        
        // Ex 2: Use reflection to inspect assembly
        public void Ex2_Assembly()
        {
            Assembly myAss = Assembly.Load("ExamRef");

            foreach (Type t in myAss.GetTypes())
            {
                Console.WriteLine("Type: {0}", t.ToString());
            }
        }

        // Ex 3: Use reflection to inspect property/field
        public void Ex3_Property()
        {
            ExClassHierarchy ech = new ExClassHierarchy();

            // Get private, non-static fields
            FieldInfo[] fields = ech.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var f in fields)
            {
                Console.WriteLine("Field: {0}", f.ToString());
            }

            // Get public, instance properties
            PropertyInfo[] props = ech.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var p in props)
            {
                Console.WriteLine("Property: {0}", p.ToString());
            }
        }

        // Ex 4: Use reflection to invoke a method dynamically
        public void Ex4_Method()
        {
            ExClassHierarchy ech = new ExClassHierarchy();

            MethodInfo m1 = ech.GetType().GetMethod("Ex2_Base", new Type[] {});
            m1.Invoke(ech, new object[] { });
        }

        // Ex 5: Use CodeDOM to produce Hello world
        public void Ex5_CodeDOM()
        {
            CodeDomProvider csc = CodeDomProvider.CreateProvider("CSharp");

            string path = @"..\..\..\TmpDat";
            string srcFile = "HelloWorld." + csc.FileExtension;
            string fullPath = Path.Combine(path, srcFile);

            // Create an IndentedTextWriter, constructed with a StreamWriter to the source file.
            using (StreamWriter tw = new StreamWriter(fullPath, false))
            {
                CodeCompileUnit compileUnit = GenerateGraph();

                // Generate source code using the code generator.
                csc.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());
            }
        }

        private CodeCompileUnit GenerateGraph()
        {
            // Create a new CodeCompileUnit to contain 
            // the program graph.
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            // Declare a new namespace called Samples.
            CodeNamespace samples = new CodeNamespace("Samples");

            // Add the new namespace to the compile unit.
            compileUnit.Namespaces.Add(samples);

            // Add the new namespace import for the System namespace.
            samples.Imports.Add(new CodeNamespaceImport("System"));

            // Declare a new type called Class1.
            CodeTypeDeclaration class1 = new CodeTypeDeclaration("Class1");
            
            // Add the new type to the namespace type collection.
            samples.Types.Add(class1);

            // Declare a new code entry point method (main).
            CodeEntryPointMethod start = new CodeEntryPointMethod();

            // Create a type reference for the System.Console class.
            CodeTypeReferenceExpression csSystemConsoleType = new CodeTypeReferenceExpression("System.Console");

            // Build a Console.WriteLine statement.
            CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression(
                csSystemConsoleType, "WriteLine",
                new CodePrimitiveExpression("Hello World!"));

            // Add the WriteLine call to the statement collection.
            start.Statements.Add(cs1);

            // Build another Console.WriteLine statement.
            CodeMethodInvokeExpression cs2 = new CodeMethodInvokeExpression(
                csSystemConsoleType, "WriteLine",
                new CodePrimitiveExpression("Press the Enter key to continue."));

            // Add the WriteLine call to the statement collection.
            start.Statements.Add(cs2);

            // Build a call to System.Console.ReadLine.
            CodeMethodInvokeExpression csReadLine = new CodeMethodInvokeExpression(
                csSystemConsoleType, "ReadLine");

            // Add the ReadLine statement.
            start.Statements.Add(csReadLine);

            // Add the code entry point method to
            // the Members collection of the type.
            class1.Members.Add(start);

            return compileUnit;
        }


    }
}
