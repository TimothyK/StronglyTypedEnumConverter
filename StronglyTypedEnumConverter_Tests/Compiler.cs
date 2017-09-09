using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

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

            string assemblyName = Path.GetRandomFileName();

            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            };

            //var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            //MetadataReference[] references = new MetadataReference[]
            //{
            //    MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
            //    MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
            //    MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
            //    MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
            //};

            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));



            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());
                    return assembly;
                }

                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Diagnostic diagnostic in failures)
                {
                    Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                }
                throw new ApplicationException("Could Not Compile Code");
            }




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
