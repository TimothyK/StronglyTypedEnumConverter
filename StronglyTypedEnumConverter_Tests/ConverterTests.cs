using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class ConverterTests
    {
        [TestMethod]
        public void Converter_Initialize_ObjectCreated()
        {
            var converter = new Converter();
            Assert.IsNotNull(converter);
        }

        private const string CowboyTypeEnumDef = "enum CowboyType {Good,Bad,Ugly};";        

        [TestMethod]
        public void Converter_BasicEnum_ReturnsNonEmptyValue()
        {
            var converter = new Converter();
            var output = converter.Convert(CowboyTypeEnumDef);
            Assert.IsNotNull(output);
        }
        
        [TestMethod]
        public void Converter_BasicEnum_ReturnsCodeThatCompiles()
        {
            var converter = new Converter();
            var sourceCode = converter.Convert(CowboyTypeEnumDef);
            
            var assembly = CompileCode(sourceCode);
            Assert.IsNotNull(assembly);
        }

        private static Assembly CompileCode(string sourceCode)
        {
            var compiler = new Microsoft.CSharp.CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");

            var compilerOut = compiler.CompileAssemblyFromSource(parameters, sourceCode);

            if (compilerOut.Errors.Count == 0) 
                return compilerOut.CompiledAssembly;

            foreach (var error in compilerOut.Errors)
                Console.Error.WriteLine(error.ToString());
            throw new ApplicationException("Could Not Compile Code");
        }

        [TestMethod]
        public void Converter_GarbageIn_ThrowsArgumentException()
        {
            var converter = new Converter();
            try
            {
                converter.Convert("garbage }{");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
                return;
            }

            Assert.Fail("Expected exception did not occur");            
        }


        [TestMethod]
        public void Converter_BasicEnum_ReturnsCodeWithOneClass()
        {
            var type = ConvertBasicCowboyTypeEnum();

            Assert.IsNotNull(type);
        }

        private static Type ConvertBasicCowboyTypeEnum()
        {
            var converter = new Converter();
            var sourceCode = converter.Convert(CowboyTypeEnumDef);
            Console.WriteLine(sourceCode);

            var assembly = CompileCode(sourceCode);

            var type = assembly.GetTypes().SingleOrDefault(t => !IsAnonymousType(t));
            return type;
        }

        //Credit: http://www.liensberger.it/web/blog/?p=191
        private static bool IsAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && (type.Name.StartsWith("<") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        [TestMethod]
        public void Converter_BasicEnum_ReturnsClassWithSameNameAsEnum()
        {
            var type = ConvertBasicCowboyTypeEnum();           
            Assert.AreEqual("CowboyType", type.Name);
        }
        
        [TestMethod]
        public void Converter_BasicEnum_ReturnsHasThreeStaticFields()
        {
            var type = ConvertBasicCowboyTypeEnum();

            var fieldNames = type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(f => f.Name)
                .ToArray();

            Assert.IsTrue(fieldNames.Contains("Good"));     
            Assert.IsTrue(fieldNames.Contains("Bad"));     
            Assert.IsTrue(fieldNames.Contains("Ugly"));     
        }
        
        [TestMethod]
        public void Converter_BasicEnum_HasAnAllMethod()
        {
            var type = ConvertBasicCowboyTypeEnum();

            var hasAllMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Any(f => f.Name == "All");

            Assert.IsTrue(hasAllMethod);     
        }

    }
    
}
