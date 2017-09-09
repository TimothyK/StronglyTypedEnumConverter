using System;
using System.Collections.Generic;
using System.Text;

namespace StronglyTypedEnumConverter
{
    internal abstract class CSharpCodeGenerator : CodeGenerator
    {
        private readonly LanguageVersion _version;

        protected CSharpCodeGenerator(Type enumType, LanguageVersion version) : base(enumType)
        {
            _version = version;
        }

        protected static string Indent(int count)
        {
            return new string(' ', count*4);
        }

        protected string NameOf(string value)
        {
            if (_version >= LanguageVersion.CSharp60)
                return $"nameof({value})";

            return $"\"{value}\"";
        }

        protected string ExpressionBody(string returnValue)
        {
            if (_version >= LanguageVersion.CSharp60)
                return $" => {returnValue};";

            var result = new StringBuilder();
            result.AppendLine();
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}return {returnValue};");
            result.Append($"{Indent(1)}}}");

            return result.ToString();
        }

        public override string UsingStatement(string nameSpace)
        {
            return $"using {nameSpace};";
        }

        public override string RegionStart(string regionName)
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}#region {regionName}");

            return result.ToString();
        }

        public override string RegionEnd()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}#endregion");
            result.AppendLine();

            return result.ToString();
        }

        public override string StartClassDefinition()
        {
            var result = new StringBuilder();

            result.AppendLine($"internal class {TypeName}");
            result.AppendLine("{");

            return result.ToString();
        }


        public override string AllMethod()
        {
            var result = new StringBuilder();

            result.Append($"{Indent(1)}public static IEnumerable<{TypeName}> All()");
            result.AppendLine(ExpressionBody($"All<{TypeName}>()"));

            result.AppendLine();
            result.AppendLine($"{Indent(1)}private static IEnumerable<T> All<T>()");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}var type = typeof(T);");
            result.AppendLine($"{Indent(2)}return type.GetFields(BindingFlags.Public | BindingFlags.Static)");
            result.AppendLine($"{Indent(3)}.Where(field => field.IsInitOnly)");
            result.AppendLine($"{Indent(3)}.Where(field => field.FieldType == type)");
            result.AppendLine($"{Indent(3)}.Select(field => field.GetValue(null))");
            result.AppendLine($"{Indent(3)}.Cast<T>();");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }

        

        public override string FromStringMethod()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}public static {TypeName} FromString(string value)");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}if (value == null) throw new ArgumentNullException({NameOf("value")});");
            result.AppendLine();
            result.AppendLine($"{Indent(2)}var result = All().FirstOrDefault(x => x.ToString() == value);");
            result.AppendLine($"{Indent(2)}if (result != null) return result;");
            result.AppendLine();
            result.AppendLine($"{Indent(2)}throw new ArgumentOutOfRangeException({NameOf("value")}, value, \"Invalid {TypeName}\");");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }

        

        public override string CastFromUnderlyingOperator()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}public static explicit operator {TypeName}({UnderlyingTypeName} value)");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}var result = All().FirstOrDefault(x => ({UnderlyingTypeName}) x == value);");
            result.AppendLine($"{Indent(2)}if (result != null) return result;");
            result.AppendLine();
            result.AppendLine($"{Indent(2)}throw new InvalidCastException(\"The value \" + value + \" is not a valid {TypeName}\");");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }

        protected string UnderlyingTypeName => Aliases[UnderlyingType];

        /// <summary>
        /// C# Alias for all .NET Types
        /// </summary>
        /// <remarks>
        /// Credit Jon Skeet:
        /// https://stackoverflow.com/questions/1362884/is-there-a-way-to-get-a-types-alias-through-reflection
        /// </remarks>
        private static readonly Dictionary<Type, string> Aliases =
            new Dictionary<Type, string>
            {
                { typeof(byte), "byte" },
                { typeof(sbyte), "sbyte" },
                { typeof(short), "short" },
                { typeof(ushort), "ushort" },
                { typeof(int), "int" },
                { typeof(uint), "uint" },
                { typeof(long), "long" },
                { typeof(ulong), "ulong" },
                { typeof(float), "float" },
                { typeof(double), "double" },
                { typeof(decimal), "decimal" },
                { typeof(object), "object" },
                { typeof(bool), "bool" },
                { typeof(char), "char" },
                { typeof(string), "string" },
                { typeof(void), "void" }
            };   

        public override string EndClassDefinition()
        {
            return "}";
        }
    }
}
