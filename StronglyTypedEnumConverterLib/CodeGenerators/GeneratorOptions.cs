
using System.Collections.Generic;

namespace StronglyTypedEnumConverter
{
    public class GeneratorOptions
    {
        public AdditionPriority AdditionPriority { get; set; } = AdditionPriority.Members;

        public bool DbValue { get; set; }
        public bool UnderlyingValue { get; set; } = true;

        public bool ImplementComparable { get; set; }

    }

    public enum AdditionPriority
    {
        Members,
        Properties
    }

    
}
