using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StronglyTypedEnumConverter
{
    internal abstract class CSharpCodeGenerator : CodeGenerator
    {
        public CSharpCodeGenerator(Type enumType) : base(enumType)
        {
        }


        public override string UsingStatement(string nameSpace)
        {
            return $"using {nameSpace};";
        }

        public override string RegionStart(string regionName)
        {
            var result = new StringBuilder();

            //result.AppendLine();
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

            result.AppendLine($"class {TypeName}");
            result.AppendLine("{");

            return result.ToString();
        }

        public override string PrivateConstructor()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}private {TypeName}() {{ }}");

            return result.ToString();
        }

        private static string Indent(int count)
        {
            return new string(' ', count*4);
        }

        public override string StaticMembers()
        {
            var result = new StringBuilder();

            foreach (var memberName in MemberNames)
            {
                result.Append($"{Indent(1)}public static readonly {TypeName} {memberName}");
                result.AppendLine($" = new {TypeName}();");
            }

            return result.ToString();
        }

        public override string AllMethod()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}public static IEnumerable<{TypeName}> All()");
            result.AppendLine($"{Indent(1)}{{");            
            result.AppendLine($"{Indent(2)}return All<{TypeName}>();");
            result.AppendLine($"{Indent(1)}}}");

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

        public override string ToStringMethod()
        {
            var result = new StringBuilder();

            result
                .AppendLine($"{Indent(1)}private static readonly Dictionary<{TypeName}, string> ToStringMap = new Dictionary<{TypeName}, string>");
            result.AppendLine($"{Indent(1)}{{");
            var toStringMappings = MemberNames
                .Select(memberName => $"{{{memberName}, \"{memberName}\"}}")
                .ToArray();
            result.Append(Indent(2));
            result.AppendLine(string.Join($",\r\n{Indent(2)}", toStringMappings));
            result.AppendLine($"{Indent(1)}}};");
            result.AppendLine();

            result.AppendLine($"{Indent(1)}public override string ToString()");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}return ToStringMap[this];");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }

        public override string FromStringMethod()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}public static {TypeName} FromString(string value)");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}if (value == null) throw new ArgumentNullException(\"value\");");
            result.AppendLine();
            result.AppendLine($"{Indent(2)}var result = All().FirstOrDefault(x => x.ToString() == value);");
            result.AppendLine($"{Indent(2)}if (result != null) return result;");
            result.AppendLine();
            result.AppendLine($"{Indent(2)}throw new ArgumentOutOfRangeException(\"value\", value, \"Invalid {TypeName}\");");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }

        public override string CastToUnderlyingOperator()
        {
            var result = new StringBuilder();

            var underlyingTypeName = Aliases[UnderlyingType];

            result.Append($"{Indent(1)}private static readonly Dictionary<{TypeName}, {underlyingTypeName}> UnderlyingMap")
                .AppendLine($" = new Dictionary<{TypeName}, {underlyingTypeName}>");
            result.AppendLine($"{Indent(1)}{{");
            var castIntMappings = Members
                .Select(member => $"{{{member.Name}, {Convert.ChangeType(member.GetValue(null), UnderlyingType)}}}")
                .ToArray();
            result.Append(Indent(2));
            result.AppendLine(string.Join($",\r\n{Indent(2)}", castIntMappings));
            result.AppendLine($"{Indent(1)}}};");
            result.AppendLine();

            result.AppendLine($"{Indent(1)}public static explicit operator {underlyingTypeName}({TypeName} value)");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}return UnderlyingMap[value];");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }

        public override string CastFromUnderlyingOperator()
        {
            var result = new StringBuilder();

            var underlyingTypeName = Aliases[UnderlyingType];
            result.AppendLine($"{Indent(1)}public static explicit operator {TypeName}({underlyingTypeName} value)");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}var result = All().FirstOrDefault(x => ({underlyingTypeName}) x == value);");
            result.AppendLine($"{Indent(2)}if (result != null) return result;");
            result.AppendLine();
            result.AppendLine($"{Indent(2)}throw new InvalidCastException(\"The value \" + value + \" is not a valid {TypeName}\");");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }

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
