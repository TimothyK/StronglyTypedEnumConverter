using System;
using System.Text;

namespace StronglyTypedEnumConverter
{
    /// <summary>
    /// Generates the strongly typed name with priority to member addition
    /// </summary>
    /// <remarks>
    /// <para>
    /// To add a new member with this generated code should be easy.  
    /// Just add the member and put all the property values as parameters to the constructor.
    /// There is no need to edit the properties when adding the new member.  
    /// Adding a new member should be as easy as adding a single line of code (i.e. the member).
    /// </para>
    /// <para>
    /// Conversly, adding a new property to this strongly typed enum class would be more difficult.
    /// You would have to change each member constructor and add a parameter for the new property.
    /// That's in addition to adding the new property.
    /// </para>
    /// </remarks>
    internal class MemberCSharpCodeGenerator : CSharpCodeGenerator
    {
        public MemberCSharpCodeGenerator(Type enumType, LanguageVersion version) : base(enumType, version)
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
                result.AppendLine($" = new {TypeName}({NameOf(member.Name)}, {memberValue});");
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
