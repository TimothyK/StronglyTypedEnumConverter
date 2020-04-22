using System;

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
        public MemberCSharpCodeGenerator(Type enumType, GeneratorOptions options) : base(enumType, options)
        {

        }

        public override string PrivateConstructor()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).Append($"private {TypeName}(string name");
            if (Options.DbValue)
                code.Append(", string dbValue");
            if (Options.UnderlyingValue)
                code.Append($", {UnderlyingTypeName} value");
            code.AppendLine(")");
            code.Indent(1).AppendLine("{");
            code.Indent(2).AppendLine("_name = name;");
            if (Options.DbValue)
                code.Indent(2).AppendLine("_dbValue = dbValue;");
            if (Options.UnderlyingValue)
                code.Indent(2).AppendLine("_value = value;");
            code.Indent(1).AppendLine("}");

            return code.ToString();
        }

        public override string StaticMembers()
        {
            var code = CreateCSharpBuilder();

            foreach (var member in Members)
            {
                var memberValue = Convert.ChangeType(member.GetValue(null), UnderlyingType);
                code.Indent(1).Append($"public static readonly {TypeName} {member.Name}");
                code.Append($" = new {TypeName}(");
                code.Append($"{NameOf(member.Name)}");
                if (Options.DbValue)
                    code.Append($", \"{DbValue(member.Name)}\"");
                if (Options.UnderlyingValue)
                    code.Append($", {memberValue}");
                code.AppendLine(");");
            }

            return code.ToString();
        }

        public override string ToStringMethod()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine("private readonly string _name;");

            code.Indent(1).Append("public override string ToString()")
                .ExpressionBody("_name");

            return code.ToString();
        }
        public override string ToDbValueMethod()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine("private readonly string _dbValue;");

            code.Indent(1).Append("public string ToDbValue()")
                .ExpressionBody("_dbValue");

            return code.ToString();
        }

        public override string CastToUnderlyingOperator()
        {
            var code = CreateCSharpBuilder();

            code.Indent(1).AppendLine($"private readonly {UnderlyingTypeName} _value;");

            code.Indent(1).Append($"public static explicit operator {UnderlyingTypeName}({TypeName} value)")
                .ExpressionBody("value._value");

            return code.ToString();
        }
    }
}
