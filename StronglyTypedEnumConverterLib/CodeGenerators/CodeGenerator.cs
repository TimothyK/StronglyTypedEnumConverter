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

        protected string Namespace => !string.IsNullOrWhiteSpace(_enumType.Namespace) ? _enumType.Namespace : "Project1";
        protected string TypeName => _enumType.Name;

        protected IEnumerable<FieldInfo> Members => _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

        protected IEnumerable<string> MemberNames
        {
            get { return Members.Select(m => m.Name); }
        }

        protected string DbValue(string memberName)
        {
            var firstLetter = memberName.Substring(0, 1);
            return MemberNames.Count(name => name.StartsWith(firstLetter)) == 1 
                ? firstLetter 
                : memberName;

        }

        protected Type UnderlyingType => _enumType.GetEnumUnderlyingType();

        public abstract string UsingStatement(string nameSpace);
        public abstract string StartNamespace();
        public abstract string RegionStart(string regionName);
        public abstract string RegionEnd();
        public abstract string StartClassDefinition();
        public abstract string PrivateConstructor();
        public abstract string StaticMembers();
        public abstract string AllField();
        public abstract string AllMethod();
        public abstract string ToStringMethod();
        public abstract string FromStringMethod();
        public abstract string ToDbValueMethod();
        public abstract string FromDbValueMethod();        
        public abstract string CastToUnderlyingOperator();
        public abstract string CastFromUnderlyingOperator();
        public abstract string CompareTo();
        public abstract string LessThan();
        public abstract string LessThanOrEqual();
        public abstract string GreaterThan();
        public abstract string GreaterThanOrEqual();
        public abstract string EndClassDefinition();
        public abstract string EndNamespace();
    }
}