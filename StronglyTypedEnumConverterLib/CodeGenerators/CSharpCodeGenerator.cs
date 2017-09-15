using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StronglyTypedEnumConverter
{
    internal abstract class CSharpCodeGenerator : CodeGenerator
    {
        private readonly GeneratorOptions _options;

        protected CSharpCodeGenerator(Type enumType, GeneratorOptions options) : base(enumType)
        {
            _options = options;
        }

        protected static string Indent(int count)
        {
            return new string(' ', count*4);
        }

        protected string NameOf(string value)
        {
            if (_options.LanguageVersion >= LanguageVersion.CSharp6)
                return $"nameof({value})";

            return $"\"{value}\"";
        }

        protected string ExpressionBody(string returnValue)
        {
            if (_options.LanguageVersion >= LanguageVersion.CSharp6)
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

            var superClasses = new List<string>();
            if (_options.ImplementComparable)
                superClasses.Add($"IComparable<{TypeName}>");

            result.Append($"internal class {TypeName}");
            if (superClasses.Any())
                result.Append(" : ").Append(string.Join(", ", superClasses));
            result.AppendLine();
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

        public override string CompareTo()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}public int CompareTo({TypeName} other)");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}var results = new[]");
            result.AppendLine($"{Indent(2)}{{");
            result.AppendLine($"{Indent(3)}(({UnderlyingTypeName}) this).CompareTo(({UnderlyingTypeName}) other)");
            result.AppendLine($"{Indent(2)}}};");
            result.AppendLine($"{Indent(2)}return results");
            result.AppendLine($"{Indent(3)}.SkipWhile(diff => diff == 0)");
            result.AppendLine($"{Indent(3)}.FirstOrDefault();");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();

        }

        public override string LessThan()
        {
            var result = new StringBuilder();

            result.Append($"{Indent(1)}public static bool operator <({TypeName} lhs, {TypeName} rhs)");
            result.AppendLine(ExpressionBody("lhs.CompareTo(rhs) < 0"));

            return result.ToString();
        }

        public override string LessThanOrEqual()
        {
            var result = new StringBuilder();

            result.Append($"{Indent(1)}public static bool operator <=({TypeName} lhs, {TypeName} rhs)");
            result.AppendLine(ExpressionBody("lhs.CompareTo(rhs) <= 0"));

            return result.ToString();
        }

        public override string GreaterThan()
        {
            var result = new StringBuilder();

            result.Append($"{Indent(1)}public static bool operator >({TypeName} lhs, {TypeName} rhs)");
            result.AppendLine(ExpressionBody("lhs.CompareTo(rhs) > 0"));

            return result.ToString();
        }

        public override string GreaterThanOrEqual()
        {
            var result = new StringBuilder();

            result.Append($"{Indent(1)}public static bool operator >=({TypeName} lhs, {TypeName} rhs)");
            result.AppendLine(ExpressionBody("lhs.CompareTo(rhs) >= 0"));

            return result.ToString();
        }

        public override string EndClassDefinition()
        {
            return "}";
        }
    }
}
