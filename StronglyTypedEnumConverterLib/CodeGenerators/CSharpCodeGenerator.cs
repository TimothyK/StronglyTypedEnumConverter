using System;
using System.Collections.Generic;
using System.Linq;

namespace StronglyTypedEnumConverter
{
    internal abstract class CSharpCodeGenerator : CodeGenerator
    {
        protected readonly GeneratorOptions Options;

        protected CSharpCodeGenerator(Type enumType, GeneratorOptions options) : base(enumType)
        {
            Options = options;
        }

        protected CSharpBuilder CreateCSharpBuilder() => new CSharpBuilder();

        protected string Indent(int count)
        {
            var code = CreateCSharpBuilder();
            code.Indent(count);
            return code.ToString();
        }

        protected string NameOf(string value)
        {
            var code = CreateCSharpBuilder();
            code.NameOf(value);
            return code.ToString();
        }

        public override string UsingStatement(string nameSpace)
        {
            return $"using {nameSpace};";
        }

        public override string RegionStart(string regionName)
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine($"#region {regionName}");

            return code.ToString();
        }

        public override string RegionEnd()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine("#endregion");
            code.AppendLine();

            return code.ToString();
        }

        public override string StartNamespace()
        {
            var code = CreateCSharpBuilder();
            code.AppendLine($"namespace {Namespace}");
            code.AppendLine("{");

            return code.ToString();
        }

        public override string StartClassDefinition()
        {
            var code = CreateCSharpBuilder();

            var superClasses = new List<string>();
            if (Options.ImplementComparable)
                superClasses.Add($"IComparable<{TypeName}>");

            code.Indent(0).Append($"internal class {TypeName}");
            if (superClasses.Any())
                code.Append(" : ").Append(string.Join(", ", superClasses));
            code.AppendLine();
            code.Indent(0).AppendLine("{");

            return code.ToString();
        }

        public override string AllField()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine($"private static List<{TypeName}> _all = new List<{TypeName}>();");                

            return code.ToString();
        }

        public override string AllMethod()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).Append($"public static IEnumerable<{TypeName}> All()")
                .ExpressionBody($"_all");

            return code.ToString();
        }

        

        public override string FromStringMethod()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine($"public static {TypeName} FromString(string value)");
            code.Indent(1).AppendLine("{");
            code.Indent(2).AppendLine($"if (value == null) throw new ArgumentNullException({NameOf("value")});");
            code.AppendLine();
            code.Indent(2).AppendLine("var result = All().FirstOrDefault(x => x.ToString() == value);");
            code.Indent(2).AppendLine("if (result != null) return result;");
            code.AppendLine();
            code.Indent(2).AppendLine($"throw new ArgumentOutOfRangeException({NameOf("value")}, value, $\"Invalid {{{NameOf(TypeName)}}}\");");
            code.Indent(1).AppendLine("}");

            return code.ToString();
        }
        public override string FromDbValueMethod()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine($"public static {TypeName} FromDbValue(string value)");
            code.Indent(1).AppendLine("{");
            code.Indent(2).AppendLine($"if (value == null) throw new ArgumentNullException({NameOf("value")});");
            code.AppendLine();
            code.Indent(2).AppendLine("var result = All().FirstOrDefault(x => x.ToDbValue() == value);");
            code.Indent(2).AppendLine("if (result != null) return result;");
            code.AppendLine();
            code.Indent(2).AppendLine($"throw new ArgumentOutOfRangeException({NameOf("value")}, value, $\"Invalid {{{NameOf(TypeName)}}}\");");
            code.Indent(1).AppendLine("}");

            return code.ToString();
        }

        

        public override string CastFromUnderlyingOperator()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine($"public static explicit operator {TypeName}({UnderlyingTypeName} value)");
            code.Indent(1).AppendLine("{");
            code.Indent(2).AppendLine($"var result = All().FirstOrDefault(x => ({UnderlyingTypeName}) x == value);");
            code.Indent(2).AppendLine("if (result != null) return result;");
            code.AppendLine();
            code.Indent(2).AppendLine($"throw new InvalidCastException($\"The value {{value}} is not a valid {{{NameOf(TypeName)}}}\");");
            code.Indent(1).AppendLine("}");

            return code.ToString();
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
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine($"public int CompareTo({TypeName} other)");
            code.Indent(1).AppendLine("{");
            code.Indent(2).AppendLine("var results = new[]");
            code.Indent(2).AppendLine("{");
            if (Options.UnderlyingValue)
                code.Indent(3).AppendLine($"(({UnderlyingTypeName}) this).CompareTo(({UnderlyingTypeName}) other)");
            else
                code.Indent(3).AppendLine("ToString().CompareTo(other.ToString())");
            code.Indent(2).AppendLine("};");
            code.Indent(2).AppendLine("return results");
            code.Indent(3).AppendLine(".SkipWhile(diff => diff == 0)");
            code.Indent(3).AppendLine(".FirstOrDefault();");
            code.Indent(1).AppendLine("}");

            return code.ToString();

        }

        public override string LessThan()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).Append($"public static bool operator <({TypeName} lhs, {TypeName} rhs)")
                .ExpressionBody("lhs.CompareTo(rhs) < 0");

            return code.ToString();
        }

        public override string LessThanOrEqual()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).Append($"public static bool operator <=({TypeName} lhs, {TypeName} rhs)")
                .ExpressionBody("lhs.CompareTo(rhs) <= 0");

            return code.ToString();
        }

        public override string GreaterThan()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).Append($"public static bool operator >({TypeName} lhs, {TypeName} rhs)")
                .ExpressionBody("lhs.CompareTo(rhs) > 0");

            return code.ToString();
        }

        public override string GreaterThanOrEqual()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).Append($"public static bool operator >=({TypeName} lhs, {TypeName} rhs)")
                .ExpressionBody("lhs.CompareTo(rhs) >= 0");

            return code.ToString();
        }

        public override string EndClassDefinition()
        {
            var code = CreateCSharpBuilder();

            code.Indent(0).AppendLine("}");

            return code.ToString();
        }

        public override string EndNamespace()
        {
            var code = CreateCSharpBuilder();
            
            code.AppendLine("}");

            return code.ToString();
        }
    }
}
