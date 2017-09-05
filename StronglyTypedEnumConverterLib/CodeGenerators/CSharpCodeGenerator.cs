﻿using System;
using System.Linq;
using System.Text;

namespace StronglyTypedEnumConverter
{
    class CSharpCodeGenerator : CodeGenerator
    {
        public CSharpCodeGenerator(Type enumType) : base(enumType)
        {
        }


        public override string UsingStatement(string nameSpace)
        {
            return "using " + nameSpace + ";";
        }

        public override string StartClassDefinition()
        {
            var result = new StringBuilder();

            result.AppendLine("internal class " + TypeName);
            result.AppendLine("{");

            return result.ToString();
        }

        public override string PrivateConstructor()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "private " + TypeName + "() { }");

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
                result.Append(Indent(1) + "public static readonly " + TypeName + " " + memberName);
                result.AppendLine(" = new " + TypeName + "();");
            }

            return result.ToString();
        }

        public override string AllMethod()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "public static IEnumerable<" + TypeName + "> All()");
            result.AppendLine(Indent(1) + "{");
            foreach (var memberName in MemberNames)
                result.AppendLine(Indent(2) + "yield return " + memberName + ";");
            result.AppendLine(Indent(1) + "}");

            return result.ToString();
        }

        public override string ToStringMethod()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "public override string ToString()");
            result.AppendLine(Indent(1) + "{");
            result.AppendLine(Indent(2) + "var map = new Dictionary<" + TypeName + ", string>");
            result.AppendLine(Indent(2) + "{");
            var toStringMappings = MemberNames
                .Select(memberName => "{" + memberName + ", \"" + memberName + "\"}")
                .ToArray();
            result.Append(Indent(3));
            result.AppendLine(string.Join(",\r\n" + Indent(3), toStringMappings));
            result.AppendLine(Indent(2) + "};");
            result.AppendLine();
            result.AppendLine(Indent(2) + "return map[this];");
            result.AppendLine(Indent(1) + "}");

            return result.ToString();
        }

        public override string FromStringMethod()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "public static " + TypeName + " FromString(string value)");
            result.AppendLine(Indent(1) + "{");
            result.AppendLine(Indent(2) + "if (value == null) throw new ArgumentNullException(\"value\");");
            result.AppendLine();
            result.AppendLine(Indent(2) + "var result = All().FirstOrDefault(x => x.ToString() == value);");
            result.AppendLine(Indent(2) + "if (result != null) return result;");
            result.AppendLine();
            result.AppendLine(Indent(2) + "throw new ArgumentOutOfRangeException(\"value\", value, \"Invalid " + TypeName + "\");");
            result.AppendLine(Indent(1) + "}");

            return result.ToString();
        }

        public override string CastToIntOperator()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "public static explicit operator int(" + TypeName + " value)");
            result.AppendLine(Indent(1) + "{");
            result.AppendLine(Indent(2) + "var map = new Dictionary<" + TypeName + ", int>");
            result.AppendLine(Indent(2) + "{");
            var castIntMappings = Members
                .Select(member => "{" + member.Name + ", " + (int)member.GetValue(null) + "}")
                .ToArray();
            result.Append(Indent(3));
            result.AppendLine(string.Join(",\r\n" + Indent(3), castIntMappings));
            result.AppendLine(Indent(2) + "};");
            result.AppendLine();
            result.AppendLine(Indent(2) + "return map[value];");
            result.AppendLine(Indent(1) + "}");

            return result.ToString();
        }

        public override string CastFromIntOperator()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "public static explicit operator " + TypeName + "(int value)");
            result.AppendLine(Indent(1) + "{");
            result.AppendLine(Indent(2) + "var result = All().FirstOrDefault(x => (int) x == value);");
            result.AppendLine(Indent(2) + "if (result != null) return result;");
            result.AppendLine();
            result.AppendLine(Indent(2) + "throw new InvalidCastException(\"The value \" + value + \" is not a valid " + TypeName + "\");");
            result.AppendLine(Indent(1) + "}");

            return result.ToString();
        }

        public override string EndClassDefinition()
        {
            return "}";
        }
    }
}
