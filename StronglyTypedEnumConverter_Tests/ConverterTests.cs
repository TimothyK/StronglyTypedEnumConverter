using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
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
        public void Converter_BasicEnum_HasPrivateConstructor()
        {
            var type = ConvertBasicCowboyTypeEnum();
            Assert.IsTrue(type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Any());            
        }
        
        [TestMethod]
        public void Converter_BasicEnum_HasNoPublicConstructor()
        {
            var type = ConvertBasicCowboyTypeEnum();            
            Assert.IsFalse(type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Any());
        }
        
        [TestMethod]
        public void Converter_BasicEnum_ReturnsHasThreeStaticReadOnlyFields()
        {
            var type = ConvertBasicCowboyTypeEnum();

            var fieldNames = type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly)
                .Select(f => f.Name)
                .ToArray();

            Assert.IsTrue(fieldNames.Contains("Good"));     
            Assert.IsTrue(fieldNames.Contains("Bad"));     
            Assert.IsTrue(fieldNames.Contains("Ugly"));     
        }
        
        [TestMethod]
        public void Converter_BasicEnum_StaticFieldsHaveUniqueValues()
        {
            var type = ConvertBasicCowboyTypeEnum();
            var values = GetEnumValues(type);

            Assert.AreEqual(values.Length, 3);
            Assert.AreEqual(values.Distinct().Count(), 3);
        }

        private static object[] GetEnumValues(Type type)
        {
            return type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly)
                .Select(f => f.GetValue(null))
                .Where(x => x != null)
                .ToArray();
        }

        [TestMethod]
        public void Converter_BasicEnum_HasAnAllMethod()
        {
            var type = ConvertBasicCowboyTypeEnum();

            var hasAllMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Any(f => f.Name == "All");

            Assert.IsTrue(hasAllMethod);     
        }
        
        [TestMethod]
        public void Converter_BasicEnum_AllMethodReturnsAllValues()
        {
            var type = ConvertBasicCowboyTypeEnum();
            var expected = GetEnumValues(type);

            var allMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(f => f.Name == "All");

            var actual = ((IEnumerable<object>) allMethod.Invoke(null, null)).ToArray();

            Assert.AreEqual(expected.Length, actual.Length);
            foreach (var item in expected)
                Assert.IsTrue(actual.Contains(item));
        }

        [TestMethod]
        public void Converter_BasicEnum_ToStringReturnsExpected()
        {
            var type = ConvertBasicCowboyTypeEnum();

            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly)
                .ToArray();

            var fieldNamesAndValues = fields.ToDictionary(f => f.Name, f => f.GetValue(null));
            foreach (var kvp in fieldNamesAndValues)
                Assert.AreEqual(kvp.Key, kvp.Value.ToString());
        }

        [TestMethod]
        public void Converter_BasicEnum_HasFromStringMethod()
        {
            var type = ConvertBasicCowboyTypeEnum();

            var hasFromStringMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Any(f => f.Name == "FromString");

            Assert.IsTrue(hasFromStringMethod);
        }
        
        [TestMethod]
        public void Converter_BasicEnum_FromStringReturnsValue()
        {
            var type = ConvertBasicCowboyTypeEnum();

            var strings = new[] {"Good", "Bad", "Ugly"};

            var fromStringMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(f => f.Name == "FromString");

            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly);

            var map = strings.ToDictionary(
                value => fields.First(f => f.Name == value).GetValue(null),
                value => fromStringMethod.Invoke(null, new object[] {value}));

            foreach (var kvp in map)
                Assert.AreSame(kvp.Key, kvp.Value);            
        }
        
        [TestMethod]
        public void Converter_BasicEnum_FromStringWithNullThrowsArgNullException()
        {
            var type = ConvertBasicCowboyTypeEnum();

            var fromStringMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(f => f.Name == "FromString");

            try
            {
                fromStringMethod.Invoke(null, new object[] {null});
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof (ArgumentNullException));
                return;
            }

            Assert.Fail("Expected exception did not occur");
            
        }
        
        [TestMethod]
        public void Converter_BasicEnum_FromStringWithInvalidValueThrowsArgRangeException()
        {
            var type = ConvertBasicCowboyTypeEnum();

            var fromStringMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(f => f.Name == "FromString");

            try
            {
                fromStringMethod.Invoke(null, new object[] {"Garbage"});
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof (ArgumentOutOfRangeException));
                StringAssert.Contains(ex.InnerException.Message, "Garbage");
                return;
            }

            Assert.Fail("Expected exception did not occur");
            
        }



    }
    
}
