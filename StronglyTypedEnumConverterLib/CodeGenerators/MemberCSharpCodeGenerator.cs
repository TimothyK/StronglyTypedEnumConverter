using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StronglyTypedEnumConverter
{
    internal class MemberCSharpCodeGenerator : CSharpCodeGenerator
    {
        public MemberCSharpCodeGenerator(Type enumType) : base(enumType)
        {

        }

        public override string PrivateConstructor()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}private {TypeName}(string name, {UnderlyingTypeName} value)");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}_name = name;");
            result.AppendLine($"{Indent(2)}_value = value;");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }

        public override string StaticMembers()
        {
            var result = new StringBuilder();

            foreach (var member in Members)
            {
                var memberValue = Convert.ChangeType(member.GetValue(null), UnderlyingType);
                result.Append($"{Indent(1)}public static readonly {TypeName} {member.Name}");
                result.AppendLine($" = new {TypeName}(\"{member.Name}\", {memberValue});");
            }

            return result.ToString();
        }

        public override string ToStringMethod()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}private readonly string _name;");

            result.AppendLine($"{Indent(1)}public override string ToString()");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}return _name;");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }

        public override string CastToUnderlyingOperator()
        {
            var result = new StringBuilder();

            result.AppendLine($"{Indent(1)}private readonly {UnderlyingTypeName} _value;");

            result.AppendLine($"{Indent(1)}public static explicit operator {UnderlyingTypeName}({TypeName} value)");
            result.AppendLine($"{Indent(1)}{{");
            result.AppendLine($"{Indent(2)}return value._value;");
            result.AppendLine($"{Indent(1)}}}");

            return result.ToString();
        }
    }
}
