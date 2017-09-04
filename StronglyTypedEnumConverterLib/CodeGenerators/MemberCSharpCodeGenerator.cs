using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StronglyTypedEnumConverter
{
    internal class MemberCSharpCodeGenerator : CSharpCodeGenerator
    {
        public MemberCSharpCodeGenerator(Type enumType) : base(enumType)
        {
        }
    }
}
