using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StronglyTypedEnumConverter
{
    public class Converter
    {
        public string Convert(string clrEnumDef)
        {
            var enumAssembly = CompileCode(clrEnumDef);
            var enumType = enumAssembly.GetTypes().Single();            

            var memberNames = enumType
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => f.Name)
                .ToArray();

            var result = new StringBuilder();

            result.AppendLine("using System.Collections.Generic;");

            //Class definition
            result.AppendLine("class " + enumType.Name);
            result.AppendLine("{");

            //Constructor
            result.AppendLine("    private " + enumType.Name + "() { }");
            result.AppendLine();

            //Public static members
            foreach (var memberName in memberNames)
            {
                result.Append("    public static readonly " + enumType.Name + " " + memberName);
                result.AppendLine(" = new " + enumType.Name + "();");
            }
            result.AppendLine();

            //All method
            result.AppendLine("    public static IEnumerable<" + enumType.Name + "> All()");
            result.AppendLine("    {");
            foreach (var memberName in memberNames)
                result.AppendLine("        yield return " + memberName + ";");
            result.AppendLine("    }");
            result.AppendLine();

            //ToString method 
            result.AppendLine(Indent(1) + "public override string ToString()");
            result.AppendLine(Indent(1) + "{");
            result.AppendLine(Indent(2) + "var map = new Dictionary<" + enumType.Name + ", string>");
            result.AppendLine(Indent(2) + "{");            
            var toStringMappings = memberNames
                .Select(memberName => "{" + memberName + ", \"" + memberName + "\"}")
                .ToArray();
            result.Append(Indent(3));
            result.AppendLine(string.Join(",\r\n" + Indent(3), toStringMappings));
            result.AppendLine(Indent(2) + "};");
            result.AppendLine();
            result.AppendLine(Indent(2) + "return map[this];");
            result.AppendLine(Indent(1) + "}");
            result.AppendLine();

            //End of class definition
            result.AppendLine("}");

            return result.ToString();
        }

        private static string Indent(int count)
        {
            return new string(' ', count*4);
        }

        private static Assembly CompileCode(string sourceCode)
        {
            var compiler = new Microsoft.CSharp.CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };

            var compilerOut = compiler.CompileAssemblyFromSource(parameters, sourceCode);

            if (compilerOut.Errors.Count == 0)
                return compilerOut.CompiledAssembly;

            throw new ArgumentException("Could Not Compile Code.  There were " + compilerOut.Errors.Count +
                                        " errors.  The first was " + compilerOut.Errors[0]);
        }
    }
}
