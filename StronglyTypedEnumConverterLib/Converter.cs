using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace StronglyTypedEnumConverter
{
    public class Converter
    {
        public string Convert(string clrEnumDef)
        {
            var enumAssembly = CompileCode(clrEnumDef);

            var enumType = enumAssembly.GetTypes()[0];

            var compileUnit = GenerateClassCompileUnit(enumType);

            var result = GenerateCodeFromCompileUnit(compileUnit);
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

        private static StringBuilder GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit)
        {
            var result = new StringBuilder();
            var compiler = new Microsoft.CSharp.CSharpCodeProvider();
            
            using (var stream = new StringWriter(result))
                compiler.GenerateCodeFromCompileUnit(compileUnit, stream, new CodeGeneratorOptions());
            
            return result;
        }

        private CodeCompileUnit GenerateClassCompileUnit(Type enumType)
        {
            var compileUnit = new CodeCompileUnit();
            var ns = new CodeNamespace(string.Empty);
            ns.Types.Add(ClassDef(enumType));
            compileUnit.Namespaces.Add(ns);
            return compileUnit;
        }

        private CodeTypeDeclaration ClassDef(Type enumType)
        {
            var classDef = new CodeTypeDeclaration(enumType.Name) {IsClass = true};
            return classDef;

        }
    }
}
