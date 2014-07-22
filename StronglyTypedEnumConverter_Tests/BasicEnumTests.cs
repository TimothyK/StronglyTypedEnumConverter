using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class BasicEnumTests
    {
        private static Type _type;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _type = ConvertBasicCowboyTypeEnum();
        }

        private const string CowboyTypeEnumDef = "enum CowboyType {Good,Bad,Ugly};";

        private static Type ConvertBasicCowboyTypeEnum()
        {
            var converter = new Converter();
            var sourceCode = converter.Convert(CowboyTypeEnumDef);
            Console.WriteLine(sourceCode);

            var assembly = CompileCode(sourceCode);

            var type = assembly.GetTypes().SingleOrDefault(t => !IsAnonymousType(t));
            return type;
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
       
        /// <summary>
        /// Returns true if the Type is anonymous
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        /// Credit: http://www.liensberger.it/web/blog/?p=191
        /// </returns>
        private static bool IsAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && (type.Name.StartsWith("<") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        [TestMethod]
        public void Class_SameNameAsEnum()
        {
            Assert.AreEqual("CowboyType", _type.Name);
        }

        [TestMethod]
        public void Class_HasPrivateConstructor()
        {
            Assert.IsTrue(_type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Any());            
        }
        
        [TestMethod]
        public void Class_HasNoPublicConstructor()
        {
            Assert.IsFalse(_type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Any());
        }
        
        [TestMethod]
        public void Members_HasThreeStaticReadOnlyFields()
        {
            var fieldNames = _type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly)
                .Select(f => f.Name)
                .ToArray();

            Assert.IsTrue(fieldNames.Contains("Good"));     
            Assert.IsTrue(fieldNames.Contains("Bad"));     
            Assert.IsTrue(fieldNames.Contains("Ugly"));     
        }
        
        [TestMethod]
        public void Members_HaveUniqueValues()
        {
            var values = GetEnumValues();

            Assert.AreEqual(values.Length, 3);
            Assert.AreEqual(values.Distinct().Count(), 3);
        }

        private static object[] GetEnumValues()
        {
            return _type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly)
                .Select(f => f.GetValue(null))
                .Where(x => x != null)
                .ToArray();
        }

        [TestMethod]
        public void All_MethodExists()
        {
            var hasAllMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Any(f => f.Name == "All");

            Assert.IsTrue(hasAllMethod);     
        }
        
        [TestMethod]
        public void All_ReturnsAllValues()
        {
            var expected = GetEnumValues();

            var allMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(f => f.Name == "All");

            var actual = ((IEnumerable<object>) allMethod.Invoke(null, null)).ToArray();

            Assert.AreEqual(expected.Length, actual.Length);
            foreach (var item in expected)
                Assert.IsTrue(actual.Contains(item));
        }

        [TestMethod]
        public void ToString_ReturnsExpected()
        {
            var fields = _type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly)
                .ToArray();

            var fieldNamesAndValues = fields.ToDictionary(f => f.Name, f => f.GetValue(null));
            foreach (var kvp in fieldNamesAndValues)
                Assert.AreEqual(kvp.Key, kvp.Value.ToString());
        }

        [TestMethod]
        public void FromString_MethodExists()
        {
            var hasFromStringMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Any(f => f.Name == "FromString");

            Assert.IsTrue(hasFromStringMethod);
        }
        
        [TestMethod]
        public void FromString_ValidInputs_ReturnsValidValues()
        {
            var strings = new[] {"Good", "Bad", "Ugly"};

            var fromStringMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(f => f.Name == "FromString");

            var fields = _type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly);

            var map = strings.ToDictionary(
                value => fields.First(f => f.Name == value).GetValue(null),
                value => fromStringMethod.Invoke(null, new object[] {value}));

            foreach (var kvp in map)
                Assert.AreSame(kvp.Key, kvp.Value);            
        }
        
        [TestMethod]
        public void FromString_NullInput_ThrowsArgNullException()
        {
            var fromStringMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
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
        public void FromString_GarbageInput_ThrowsArgRangeException()
        {
            var fromStringMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
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
