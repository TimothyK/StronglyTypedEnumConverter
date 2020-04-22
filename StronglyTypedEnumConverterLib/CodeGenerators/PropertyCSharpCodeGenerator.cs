using System;
using System.Linq;

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
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine($"private {TypeName}() {{ }}");

            return code.ToString();
        }

        public override string StaticMembers()
        {
            var code = CreateCSharpBuilder();

            foreach (var memberName in MemberNames)
            {
                code.Indent(1).Append($"public static readonly {TypeName} {memberName}");
                code.AppendLine($" = new {TypeName}();");
            }

            return code.ToString();
        }

        public override string ToStringMethod()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1)
                .AppendLine($"private static readonly Dictionary<{TypeName}, string> ToStringMap = new Dictionary<{TypeName}, string>");
            code.Indent(1).AppendLine("{");
            var toStringMappings = MemberNames
                .Select(memberName => $"{{{memberName}, {NameOf(memberName)}}}")
                .ToArray();
            code.Indent(2);
            code.AppendLine(string.Join($",\r\n{Indent(2)}", toStringMappings));
            code.Indent(1).AppendLine("};");
            code.AppendLine();

            code.Indent(1).Append("public override string ToString()")
                .ExpressionBody("ToStringMap[this]");

            return code.ToString();
        }

        public override string ToDbValueMethod()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1)
                .AppendLine($"private static readonly Dictionary<{TypeName}, string> DbValueMap = new Dictionary<{TypeName}, string>");
            code.Indent(1).
                AppendLine("{");
            var dbValueMappings = MemberNames
                .Select(memberName => $"{{{memberName}, \"{DbValue(memberName)}\"}}")
                .ToArray();
            code.Indent(2);
            code.AppendLine(string.Join($",\r\n{Indent(2)}", dbValueMappings));
            code.Indent(1).AppendLine("};");
            code.AppendLine();

            code.Indent(1).Append("public string ToDbValue()")
                .ExpressionBody("DbValueMap[this]");

            return code.ToString();
        }

        public override string CastToUnderlyingOperator()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).Append($"private static readonly Dictionary<{TypeName}, {UnderlyingTypeName}> UnderlyingMap")
                .AppendLine($" = new Dictionary<{TypeName}, {UnderlyingTypeName}>");
            code.Indent(1).AppendLine("{");
            var castIntMappings = Members
                .Select(member => $"{{{member.Name}, {Convert.ChangeType(member.GetValue(null), UnderlyingType)}}}")
                .ToArray();
            code.Indent(2);
            code.AppendLine(string.Join($",\r\n{Indent(2)}", castIntMappings));
            code.Indent(1).AppendLine("};");
            code.AppendLine();

            code.Indent(1).Append($"public static explicit operator {UnderlyingTypeName}({TypeName} value)")
                .ExpressionBody("UnderlyingMap[value]");

            return code.ToString();
        }
    }
}
