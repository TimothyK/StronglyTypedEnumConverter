//Copyright (c) 2014 Timothy Klenke
//Licensed under The MIT License

using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.Text;
using StronglyTypedEnumConverter.CodeGenerators;

namespace StronglyTypedEnumConverter
{
    public class Converter
    {
        public string Convert(string clrEnumDef)
        {
            var factory = LanguageAbstractFactory.Create(clrEnumDef);

            var enumAssembly = CompileCode(clrEnumDef, factory.CodeProvider());
            var enumType = enumAssembly
                .GetTypes()
                .Single(t => t.IsEnum);

            var gen = factory.CodeGenerator(enumType);

            var result = new StringBuilder();

            result.AppendLine(gen.UsingStatement("System"));
            result.AppendLine(gen.UsingStatement("System.Collections.Generic"));
            result.AppendLine(gen.UsingStatement("System.Linq"));
            result.AppendLine(gen.UsingStatement("System.Reflection"));
            result.AppendLine();

            result.AppendLine(gen.StartClassDefinition());
            result.AppendLine(gen.PrivateConstructor());
            result.AppendLine(gen.StaticMembers());
            result.AppendLine(gen.AllMethod());
            result.AppendLine(gen.ToStringMethod());
            result.AppendLine(gen.FromStringMethod());
            result.AppendLine(gen.CastToUnderlyingOperator());
            result.AppendLine(gen.CastFromUnderlyingOperator());
            result.AppendLine(gen.EndClassDefinition());

            return result.ToString();
        }

        private static Assembly CompileCode(string sourceCode, CodeDomProvider codeProvider)
        {            
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };

            var compilerOut = codeProvider.CompileAssemblyFromSource(parameters, sourceCode);

            if (compilerOut.Errors.Count == 0)
                return compilerOut.CompiledAssembly;

            throw new ArgumentException("Could Not Compile Code.  There were " + compilerOut.Errors.Count +
                                        " errors.  The first was " + compilerOut.Errors[0]);
        }
    }
}
