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
            var options = new GeneratorOptions();
            return Convert(clrEnumDef, options);
        }

        public string Convert(string clrEnumDef, Action<GeneratorOptions> adjustOptions)
        {
            var options = new GeneratorOptions();
            adjustOptions?.Invoke(options);
            return Convert(clrEnumDef, options);
        }


        public string Convert(string clrEnumDef, GeneratorOptions options)
        {
            var factory = LanguageAbstractFactory.Create(options);

            var enumAssembly = CompileCode(clrEnumDef, factory.CodeProvider());
            var enumType = enumAssembly
                .GetTypes()
                .Single(t => t.IsEnum);

            var gen = factory.CodeGenerator(enumType, options);

            var result = new StringBuilder();

            result.AppendLine(gen.UsingStatement("System"));
            result.AppendLine(gen.UsingStatement("System.Collections.Generic"));
            result.AppendLine(gen.UsingStatement("System.Linq"));
            result.AppendLine();

            result.Append(gen.StartNamespace());
            
            result.Append(gen.StartClassDefinition());
            result.AppendLine(gen.PrivateConstructor());

            result.AppendLine(gen.RegionStart("All"));
            result.Append(gen.AllField());  //Must be before Members.  Otherwise NullRef in constructor when _all.Add is called.
            result.AppendLine(gen.AllMethod());
            result.AppendLine();
            result.Append(gen.RegionEnd());

            result.AppendLine(gen.RegionStart("Members"));
            result.AppendLine(gen.StaticMembers());
            result.Append(gen.RegionEnd());

            result.AppendLine(gen.RegionStart("To/From String"));
            result.AppendLine(gen.ToStringMethod());
            result.AppendLine(gen.FromStringMethod());
            result.Append(gen.RegionEnd());

            if (options.DbValue)
            {
                result.AppendLine(gen.RegionStart("DbValue"));
                result.AppendLine(gen.ToDbValueMethod());
                result.AppendLine(gen.FromDbValueMethod());
                result.Append(gen.RegionEnd());
            }

            if (options.UnderlyingValue)
            {
                result.AppendLine(gen.RegionStart("Cast to/from Underlying Type"));
                result.AppendLine(gen.CastToUnderlyingOperator());
                result.AppendLine(gen.CastFromUnderlyingOperator());
                result.Append(gen.RegionEnd());
                
            }

            if (options.ImplementComparable)
            {
                result.AppendLine(gen.RegionStart("IComparable"));
                result.AppendLine(gen.CompareTo());
                result.AppendLine(gen.LessThan());
                result.AppendLine(gen.LessThanOrEqual());
                result.AppendLine(gen.GreaterThan());
                result.AppendLine(gen.GreaterThanOrEqual());
                result.AppendLine();
                result.Append(gen.RegionEnd());
            }

            result.Append(gen.EndClassDefinition());
            result.Append(gen.EndNamespace());

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
