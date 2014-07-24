using System;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;

namespace StronglyTypedEnumConverter.CodeGenerators
{
    internal abstract class LanguageAbstractFactory
    {
        public static LanguageAbstractFactory Create(string sourceCode)
        {
            if (IsVisualBasic(sourceCode))
                return new VbAbstractFactory() ;

            return new CSharpAbstractFactory();
        }

        private static bool IsVisualBasic(string sourceCode)
        {
            return Regex.IsMatch(sourceCode, "Enum .*End *Enum", RegexOptions.Singleline);
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

    class VbAbstractFactory : LanguageAbstractFactory
    {
        public override CodeGenerator CodeGenerator(Type enumType)
        {
            return new VbCodeGenerator(enumType);
        }

        public override CodeDomProvider CodeProvider()
        {
            return new Microsoft.VisualBasic.VBCodeProvider();
        }
    }
}
