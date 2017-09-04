using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StronglyTypedEnumConverter
{
    internal class PropertyCSharpCodeGenerator : CSharpCodeGenerator
    {
        public PropertyCSharpCodeGenerator(Type enumType) : base(enumType)
        {
        }
    }
}
