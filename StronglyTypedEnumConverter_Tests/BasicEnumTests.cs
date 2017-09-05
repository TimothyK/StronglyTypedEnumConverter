using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public abstract class BasicEnumTests
    {
        private static Type _type;

        protected static void ClassInit(Action<GeneratorOptions> adjustOptions)
        {
            _type = CompiledStrongTypeFromEnumSourceCode(adjustOptions);
            EnumMembers = _type.GetEnumMembers();
            EnumValues = _type.GetEnumMemberValues();
        }

        private const string SourceCode = "enum CowboyType {Good,Bad,Ugly};";

        /// <summary>
        /// Compiles enum source code to an in-memory strongly typed Type
        /// </summary>
        /// <param name="adjustOptions"></param>
        /// <returns></returns>
        private static Type CompiledStrongTypeFromEnumSourceCode(Action<GeneratorOptions> adjustOptions)
        {
            var converter = new Converter();
            var stronglyTypedSourceCode = converter.Convert(SourceCode, adjustOptions);
            Console.WriteLine(stronglyTypedSourceCode);

            var compiler = new Compiler();
            var assembly = compiler.Compile(stronglyTypedSourceCode);

            var type = assembly.GetTypes().SingleOrDefault(t => !t.IsAnonymous());
            return type;
        }

        /// <summary>
        /// Returns the public static readonly fields that represent the Enum members
        /// </summary>
        /// <value></value>
        private static IEnumerable<FieldInfo> EnumMembers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private static object[] EnumValues { get; set; }

        [TestMethod]
        public void Class_SameNameAsEnum()
        {
            _type.Name.ShouldBe("CowboyType");
        }

        [TestMethod]
        public void Class_IsInternal()
        {
            Assert.IsFalse(_type.IsPublic);
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
            var fieldNames = EnumMembers
                .Select(f => f.Name)
                .ToArray();

            fieldNames.ShouldContain("Good");
            fieldNames.ShouldContain("Bad");
            fieldNames.ShouldContain("Ugly");
        }

        [TestMethod]
        public void Members_HaveUniqueValues()
        {
            var values = EnumValues;

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
            var expected = EnumValues;

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
            var fields = EnumMembers.ToArray();

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

            var fields = EnumMembers;

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
            var fields = EnumMembers.ToArray();

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
            var fields = EnumMembers.ToArray();

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
