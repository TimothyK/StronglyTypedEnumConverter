using System;
using System.Linq;
using System.Text;

namespace StronglyTypedEnumConverter
{

    /// <summary>
    /// Generates the strongly typed name with priority to property addition
    /// </summary>
    /// <remarks>
    /// <para>
    /// To add a new property with this generated code should be easy.  
    /// Just add the property.  Create a static dictionary so to define the property value for each of the existing members.
    /// </para>
    /// <para>
    /// Conversly, adding a new member to this strongly typed enum class would be more difficult.
    /// You would have to the mapping dictionary for every property to ensure the new member has a property value defined.
    /// That's in addition to adding the new member.
    /// </para>
    /// </remarks>
    internal class PropertyCSharpCodeGenerator : CSharpCodeGenerator
    {
        public PropertyCSharpCodeGenerator(Type enumType, GeneratorOptions options) : base(enumType, options)
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
                .Select(memberName => $"{{{memberName}, {NameOf(memberName)}}}")
                .ToArray();
            result.Append(Indent(2));
            result.AppendLine(string.Join($",\r\n{Indent(2)}", toStringMappings));
            result.AppendLine($"{Indent(1)}}};");
            result.AppendLine();

            result.Append($"{Indent(1)}public override string ToString()");
            result.AppendLine(ExpressionBody("ToStringMap[this]"));

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

            result.Append($"{Indent(1)}public static explicit operator {UnderlyingTypeName}({TypeName} value)");
            result.AppendLine(ExpressionBody("UnderlyingMap[value]"));

            return result.ToString();
        }
    }
}
