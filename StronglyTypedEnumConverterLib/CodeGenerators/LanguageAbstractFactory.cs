using System;
using System.CodeDom.Compiler;

namespace StronglyTypedEnumConverter.CodeGenerators
{
    internal abstract class LanguageAbstractFactory
    {
        public static LanguageAbstractFactory Create(string sourceCode)
        {
            return new CSharpAbstractFactory();
        }

        public abstract CodeGenerator CodeGenerator(Type enumType);
        public abstract CodeDomProvider CodeProvider();
    }

    class CSharpAbstractFactory : LanguageAbstractFactory
    {
        public override CodeGenerator CodeGenerator(Type enumType)
        {
            return new CSharpCodeGenerator(enumType);
        }

        public override CodeDomProvider CodeProvider()
        {
            return new Microsoft.CSharp.CSharpCodeProvider();
        }
    }
}
