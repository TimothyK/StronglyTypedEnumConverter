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

            var gen = new CodeGenerator(enumType);

            var result = new StringBuilder();

            result.AppendLine(gen.UsingStatement("System"));
            result.AppendLine(gen.UsingStatement("System.Collections.Generic"));
            result.AppendLine(gen.UsingStatement("System.Linq"));
            result.AppendLine();

            result.AppendLine(gen.StartClassDefinition());
            result.AppendLine(gen.PrivateConstructor());
            result.AppendLine(gen.StaticMembers());
            result.AppendLine(gen.AllMethod());
            result.AppendLine(gen.ToStringMethod());
            result.AppendLine(gen.FromStringMethod());
            result.AppendLine(gen.CastToIntOperator());
            result.AppendLine(gen.CastFromIntOperator());
            result.AppendLine(gen.EndClassDefinition());

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
