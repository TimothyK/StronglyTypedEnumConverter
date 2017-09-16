using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class OptionTests
    {
        /// <summary>
        /// Compiles enum source code to an in-memory strongly typed Type
        /// </summary>
        /// <param name="enumSourceCode"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static Type CompiledStrongTypeFromEnumSourceCode(string enumSourceCode, GeneratorOptions options)
        {
            var converter = new Converter();
            var stronglyTypedSourceCode = converter.Convert(enumSourceCode, options);
            Console.WriteLine(stronglyTypedSourceCode);

            var compiler = new Compiler();
            var assembly = compiler.Compile(stronglyTypedSourceCode, options.LanguageVersion);

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
        public void NoUnderlying_NoExplicitMethod()
        {
            var options = new GeneratorOptions
            {
                UnderlyingValue = false,
                ImplementComparable = true
            };

            var type = CompiledStrongTypeFromEnumSourceCode("enum CowboyType {Good,Bad,Ugly};", options);

            type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.Name == "op_Explicit")
                .ShouldBeEmpty();
        }
    }
}
