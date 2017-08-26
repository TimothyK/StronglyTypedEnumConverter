using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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
                throw new ArgumentNullException(nameof(type));

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && (type.Name.StartsWith("<") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        /// <summary>
        /// Returns the public static readonly fields that represent the Enum members
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<FieldInfo> GetEnumMembers()
        {
            return _type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly);
        }

        private static object[] GetEnumValues()
        {
            return GetEnumMembers()
                .Select(f => f.GetValue(null))
                .Where(x => x != null)
                .ToArray();
        }

        [TestMethod]
        public void Class_SameNameAsEnum()
        {
            _type.Name.ShouldBe("CowboyType");
        }

        [TestMethod]
        public void Class_HasPrivateConstructor()
        {
            _type
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any()
                .ShouldBeTrue();
        }

        [TestMethod]
        public void Class_HasNoPublicConstructor()
        {
            _type
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Any()
                .ShouldBeFalse();
        }

        [TestMethod]
        public void Members_HasThreeStaticReadOnlyFields()
        {
            var fieldNames = GetEnumMembers()
                .Select(f => f.Name)
                .ToArray();

            fieldNames.ShouldContain("Good");
            fieldNames.ShouldContain("Bad");
            fieldNames.ShouldContain("Ugly");
        }

        [TestMethod]
        public void Members_HaveUniqueValues()
        {
            var values = GetEnumValues();

            values.Length.ShouldBe(3);
            values.Distinct().Count().ShouldBe(3);
        }

        [TestMethod]
        public void All_MethodExists()
        {
            var hasAllMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Any(f => f.Name == "All");

            hasAllMethod.ShouldBeTrue();
        }

        [TestMethod]
        public void All_ReturnsAllValues()
        {
            var expected = GetEnumValues();

            var allMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Single(f => f.Name == "All");

            var actual = ((IEnumerable<object>)allMethod.Invoke(null, null)).ToArray();

            Assert.AreEqual(expected.Length, actual.Length);
            foreach (var item in expected)
                actual.ShouldContain(item);
        }

        [TestMethod]
        public void ToString_ReturnsExpected()
        {
            var fields = GetEnumMembers().ToArray();

            var fieldNamesAndValues = fields.ToDictionary(f => f.Name, f => f.GetValue(null));
            foreach (var kvp in fieldNamesAndValues)
                kvp.Key.ShouldBe(kvp.Value.ToString());
        }

        [TestMethod]
        public void FromString_MethodExists()
        {
            var hasFromStringMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Any(f => f.Name == "FromString");

            hasFromStringMethod.ShouldBeTrue();
        }

        [TestMethod]
        public void FromString_ValidInputs_ReturnsValidValues()
        {
            var strings = new[] { "Good", "Bad", "Ugly" };

            var fromStringMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(f => f.Name == "FromString");

            var fields = GetEnumMembers();

            var map = strings.ToDictionary(
                value => fields.First(f => f.Name == value).GetValue(null),
                value => fromStringMethod.Invoke(null, new object[] { value }));

            foreach (var kvp in map)
                kvp.Key.ShouldBeSameAs(kvp.Value);
        }

        [TestMethod]
        public void FromString_NullInput_ThrowsArgNullException()
        {
            var fromStringMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(f => f.Name == "FromString");

            try
            {
                fromStringMethod.Invoke(null, new object[] { null });
            }
            catch (TargetInvocationException ex)
            {
                ex.InnerException.ShouldBeOfType<ArgumentNullException>();
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
                fromStringMethod.Invoke(null, new object[] { "Garbage" });
            }
            catch (TargetInvocationException ex)
            {
                var innerException = ex.InnerException;
                innerException.ShouldNotBeNull();
                innerException.ShouldBeOfType<ArgumentOutOfRangeException>();
                innerException.Message.ShouldContain("Garbage");
                return;
            }

            Assert.Fail("Expected exception did not occur");
        }

        [TestMethod]
        public void ExplicitToInt_HasMethod()
        {
            var opExplicitMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.Name == "op_Explicit")
                .Where(f => f.ReturnType == typeof(int))
                .SingleOrDefault(f => f.GetParameters().Single().ParameterType == _type);

            opExplicitMethod.ShouldNotBeNull();
        }

        [TestMethod]
        public void ExplicitToInt_Values_CastCorrectly()
        {
            var fields = GetEnumMembers().ToArray();

            var opExplicitMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.Name == "op_Explicit")
                .Where(f => f.ReturnType == typeof(int))
                .Single(f => f.GetParameters().Single().ParameterType == _type);

            var map = new Dictionary<string, int>
            {
                {"Good", 0},
                {"Bad", 1},
                {"Ugly", 2}
            };

            foreach (var kvp in map)
            {
                var field = fields.First(f => f.Name == kvp.Key);
                var enumValue = field.GetValue(null);
                var actual = opExplicitMethod.Invoke(null, new[] { enumValue });
                actual.ShouldBe(kvp.Value, "Value of " + kvp.Key + " has incorrect integer mapping");
            }

        }

        [TestMethod]
        public void ExplicitFromInt_HasMethod()
        {
            var opExplicitMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.Name == "op_Explicit")
                .Where(f => f.ReturnType == _type)
                .SingleOrDefault(f => f.GetParameters().Single().ParameterType == typeof(int));

            opExplicitMethod.ShouldNotBeNull();
        }

        [TestMethod]
        public void ExplicitFromInt_Values_CastCorrectly()
        {
            var fields = GetEnumMembers().ToArray();

            var opExplicitMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.Name == "op_Explicit")
                .Where(f => f.ReturnType == _type)
                .Single(f => f.GetParameters().Single().ParameterType == typeof(int));

            var map = new Dictionary<string, int>
            {
                {"Good", 0},
                {"Bad", 1},
                {"Ugly", 2}
            };

            foreach (var kvp in map)
            {
                var field = fields.First(f => f.Name == kvp.Key);
                var expected = field.GetValue(null);
                var actual = opExplicitMethod.Invoke(null, new object[] { kvp.Value });
                actual.ShouldBe(expected, "Value of " + kvp.Key + " has incorrect integer mapping");
            }
        }

        [TestMethod]
        public void ExplicitFromInt_InvalidValue_ThrowsInvalidCastException()
        {
            var opExplicitMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.Name == "op_Explicit")
                .Where(f => f.ReturnType == _type)
                .Single(f => f.GetParameters().Single().ParameterType == typeof(int));

            try
            {
                opExplicitMethod.Invoke(null, new object[] { 3 });
            }
            catch (TargetInvocationException ex)
            {
                var innerException = ex.InnerException;
                innerException.ShouldNotBeNull();
                innerException.ShouldBeOfType<InvalidCastException>();
                innerException.Message.ShouldContain("3");
                return;
            }

            Assert.Fail("Expected exception did not occur");
        }


    }

}
