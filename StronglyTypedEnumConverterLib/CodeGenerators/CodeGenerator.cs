using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StronglyTypedEnumConverter
{
    internal abstract class CodeGenerator
    {
        private readonly Type _enumType;

        protected CodeGenerator(Type enumType)
        {
            _enumType = enumType;
        }

        protected string TypeName => _enumType.Name;

        protected IEnumerable<FieldInfo> Members => _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

        protected IEnumerable<string> MemberNames
        {
            get { return Members.Select(m => m.Name); }
        }

        protected Type UnderlyingType => _enumType.GetEnumUnderlyingType();

        public abstract string UsingStatement(string nameSpace);
        public abstract string RegionStart(string regionName);
        public abstract string RegionEnd();
        public abstract string StartClassDefinition();
        public abstract string PrivateConstructor();
        public abstract string StaticMembers();
        public abstract string AllMethod();
        public abstract string ToStringMethod();
        public abstract string FromStringMethod();
        public abstract string CastToUnderlyingOperator();
        public abstract string CastFromUnderlyingOperator();
        public abstract string EndClassDefinition();
    }
}