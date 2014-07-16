using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace StronglyTypedEnumConverter
{
    public class Converter
    {
        public string Convert(string clrEnumDef)
        {
            var clrEnum = CompileCode(clrEnumDef);

            return string.Empty;
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
