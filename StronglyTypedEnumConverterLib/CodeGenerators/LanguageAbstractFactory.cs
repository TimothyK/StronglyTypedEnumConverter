using System;
using System.CodeDom.Compiler;

namespace StronglyTypedEnumConverter.CodeGenerators
{
    internal abstract class LanguageAbstractFactory
    {
        public static LanguageAbstractFactory Create(GeneratorOptions options)
        {               
            return new CSharpAbstractFactory(options);
        }
        
        public abstract CodeGenerator CodeGenerator(Type enumType, LanguageVersion version);
        public abstract CodeDomProvider CodeProvider();
    }

    class CSharpAbstractFactory : LanguageAbstractFactory
    {
        private readonly GeneratorOptions _options;

        public CSharpAbstractFactory(GeneratorOptions options)
        {
            _options = options;
        }

        public override CodeGenerator CodeGenerator(Type enumType, LanguageVersion version)
        {
            return _options.AdditionPriority == AdditionPriority.Members
                ? (CodeGenerator) new MemberCSharpCodeGenerator(enumType, version)
                : new PropertyCSharpCodeGenerator(enumType, version);
        }

        public override CodeDomProvider CodeProvider()
        {
            return new Microsoft.CSharp.CSharpCodeProvider();
        }
    }

}
