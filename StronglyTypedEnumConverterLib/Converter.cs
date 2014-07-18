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



            //End of class definition
            result.AppendLine("}");

            return result.ToString();
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
