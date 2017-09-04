using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class ByteEnumTests
    {

        private static Type _type;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _type = CompiledStrongTypeFromEnumSourceCode("enum CowboyType : byte {Good,Bad,Ugly};");
            EnumMembers = _type.GetEnumMembers();
            EnumValues = _type.GetEnumMemberValues();
        }


        /// <summary>
        /// Compiles enum source code to an in-memory strongly typed Type
        /// </summary>
        /// <param name="enumSourceCode"></param>
        /// <returns></returns>
        private static Type CompiledStrongTypeFromEnumSourceCode(string enumSourceCode)
        {
            var converter = new Converter();
            var options = new GeneratorOptions();
            var stronglyTypedSourceCode = converter.Convert(enumSourceCode, options);
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
        public void ExplicitToByte_HasMethod()
        {
            var opExplicitMethod = _type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.Name == "op_Explicit")
                .Where(f => f.ReturnType == typeof(byte))
                .SingleOrDefault(f => f.GetParameters().Single().ParameterType == _type);

            opExplicitMethod.ShouldNotBeNull();
        }
    }
}
