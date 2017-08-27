using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace StronglyTypedEnumConverter
{
    internal class Compiler
    {
        public Compiler()
        {
            References.Add("System.dll");
            References.Add("System.Core.dll");
        }

        public readonly List<string> References = new List<string>();

        /// <summary>
        /// Compiles the source code into an in-memory Assembly
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <returns></returns>
        public Assembly Compile(string sourceCode)
        {
            var compiler = new Microsoft.CSharp.CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };
            foreach (var reference in References)
                parameters.ReferencedAssemblies.Add(reference);

            var compilerOut = compiler.CompileAssemblyFromSource(parameters, sourceCode);

            if (compilerOut.Errors.Count == 0)
                return compilerOut.CompiledAssembly;

            foreach (var error in compilerOut.Errors)
                Console.Error.WriteLine(error.ToString());
            throw new ApplicationException("Could Not Compile Code");
        }
    }
}
