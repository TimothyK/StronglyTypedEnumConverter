using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StronglyTypedEnumConverter
{
    internal class PropertyCSharpCodeGenerator : CSharpCodeGenerator
    {
        public PropertyCSharpCodeGenerator(Type enumType) : base(enumType)
        {
        }

        public override string PrivateConstructor()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}private {TypeName}() {{ }}");

            return result.ToString();
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

        public override string CastToUnderlyingOperator()
        {
            var result = new StringBuilder();

            result.Append($"{Indent(1)}private static readonly Dictionary<{TypeName}, {UnderlyingTypeName}> UnderlyingMap")
                .AppendLine($" = new Dictionary<{TypeName}, {UnderlyingTypeName}>");
            result.AppendLine($"{Indent(1)}{{");
            var castIntMappings = Members
                .Select(member => $"{{{member.Name}, {Convert.ChangeType(member.GetValue(null), UnderlyingType)}}}")
                .ToArray();
            result.Append(Indent(2));
            result.AppendLine(string.Join($",\r\n{Indent(2)}", castIntMappings));
            result.AppendLine($"{Indent(1)}}};");
            result.AppendLine();

            result.AppendLine($"{Indent(1)}public static explicit operator {UnderlyingTypeName}({TypeName} value)");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}return UnderlyingMap[value];");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }
    }
}
