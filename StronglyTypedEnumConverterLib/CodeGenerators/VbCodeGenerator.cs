using System;
using System.Linq;
using System.Text;

namespace StronglyTypedEnumConverter.CodeGenerators
{
    class VbCodeGenerator : CodeGenerator        
    {
        public VbCodeGenerator(Type enumType) : base(enumType)
        {
        }

        public override string UsingStatement(string nameSpace)
        {
            return "Imports " + nameSpace;
        }

        public override string StartClassDefinition()
        {
            var result = new StringBuilder();

            result.AppendLine("Class " + TypeName);

            return result.ToString();
        }

        public override string PrivateConstructor()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "Private Sub New()");
            result.AppendLine(Indent(1) + "End Sub");

            return result.ToString();
        }

        private static string Indent(int count)
        {
            return new string(' ', count * 4);
        }

        public override string StaticMembers()
        {
            var result = new StringBuilder();

            foreach (var memberName in MemberNames)
                result.AppendLine(Indent(1) + "Public Shared ReadOnly " + memberName + " As New " + TypeName + "()");

            return result.ToString();
        }

        public override string AllMethod()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "Public Shared Iterator Function All() As IEnumerable(Of " + TypeName + ")");
            foreach (var memberName in MemberNames)
                result.AppendLine(Indent(2) + "Yield " + memberName);
            result.AppendLine(Indent(1) + "End Function");

            return result.ToString();
        }

        public override string ToStringMethod()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "Public Overrides Function ToString() As String");
            result.AppendLine(Indent(2) + "Dim map = New Dictionary(Of " + TypeName + ", string)() From {");
            var toStringMappings = MemberNames
                .Select(memberName => "{" + memberName + ", \"" + memberName + "\"}")
                .ToArray();
            result.Append(Indent(3));
            result.AppendLine(string.Join(",\r\n" + Indent(3), toStringMappings));
            result.AppendLine(Indent(2) + "}");
            result.AppendLine();
            result.AppendLine(Indent(2) + "Return map(Me)");
            result.AppendLine(Indent(1) + "End Function");

            return result.ToString();
        }

        public override string FromStringMethod()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "Public Shared Function FromString(value As String) As " + TypeName);
            result.AppendLine(Indent(2) + "If value Is Nothing Then");
            result.AppendLine(Indent(3) + "Throw New ArgumentNullException(\"value\")");
            result.AppendLine(Indent(2) + "End If");
            result.AppendLine();
            result.AppendLine(Indent(2) + "Dim result = All().FirstOrDefault(Function(x) x.ToString() = value)");
            result.AppendLine(Indent(2) + "If result IsNot Nothing Then");
            result.AppendLine(Indent(3) + "Return result");
            result.AppendLine(Indent(2) + "End If");
            result.AppendLine();
            result.AppendLine(Indent(2) + "Throw New ArgumentOutOfRangeException(\"value\", value, \"Invalid " + TypeName + "\")");
            result.AppendLine(Indent(1) + "End Function");

            return result.ToString();
        }

        public override string CastToIntOperator()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "Public Shared Narrowing Operator CType(value As " + TypeName + ") As Integer");
            result.AppendLine(Indent(2) + "Dim map = New Dictionary(Of " + TypeName + ", Integer)() From {");
            var castIntMappings = Members
                .Select(member => "{" + member.Name + ", " + (int)member.GetValue(null) + "}")
                .ToArray();
            result.Append(Indent(3));
            result.AppendLine(string.Join(",\r\n" + Indent(3), castIntMappings));
            result.AppendLine(Indent(2) + "}");
            result.AppendLine();
            result.AppendLine(Indent(2) + "Return map(value)");
            result.AppendLine(Indent(1) + "End Operator");

            return result.ToString();
        }

        public override string CastFromIntOperator()
        {
            var result = new StringBuilder();

            result.AppendLine(Indent(1) + "Public Shared Narrowing Operator CType(value As Integer) As " + TypeName);
            result.AppendLine(Indent(2) + "Dim result = All().FirstOrDefault(Function(x) CInt(x) = value)");
            result.AppendLine(Indent(2) + "If result IsNot Nothing Then");
            result.AppendLine(Indent(3) + "Return result");
            result.AppendLine(Indent(2) + "End If");
            result.AppendLine();
            result.AppendLine(Indent(2) + "Throw New InvalidCastException(\"The value \" & value & \" is not a valid " + TypeName + "\")");
            result.AppendLine(Indent(1) + "End Operator");

            return result.ToString();
        }

        public override string EndClassDefinition()
        {
            return "End Class";
        }

    }
}
