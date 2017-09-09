﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace StronglyTypedEnumConverter
{
    internal class Compiler
    {
        /// <summary>
        /// Compiles the source code into an in-memory Assembly
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public Assembly Compile(string sourceCode, LanguageVersion version)
        {

            var assemblyName = Path.GetRandomFileName();

            MetadataReference[] references = {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            };

            var options = new CSharpParseOptions(RoslynVersion(version));
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode,options);
            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(ms.ToArray());
                    return assembly;
                }

                var failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures)
                {
                    Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                }
                throw new ApplicationException("Could Not Compile Code");
            }
        }

        private static Microsoft.CodeAnalysis.CSharp.LanguageVersion RoslynVersion(LanguageVersion version)
        {
            if (version == LanguageVersion.CSharp50)
                return Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp5;
            if (version == LanguageVersion.CSharp60)
                return Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp6;
            if (version == LanguageVersion.CSharp70)
                return Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7;
            
            return Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7_1;
        }
    }
}
