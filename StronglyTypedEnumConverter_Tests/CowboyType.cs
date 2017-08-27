
using System;
using System.Collections.Generic;
using System.Linq;

namespace StronglyTypedEnumConverter
{
    class CowboyType
    {
        private CowboyType() 
        {
        }

        public static readonly CowboyType Good = new CowboyType();
        public static readonly CowboyType Bad = new CowboyType();
        public static readonly CowboyType Ugly = new CowboyType();

        public static IEnumerable<CowboyType> All()
        {
            yield return Good;
            yield return Bad;
            yield return Ugly;
        }

        private static readonly Dictionary<CowboyType, string> ToStringMap = new Dictionary<CowboyType, string>
        {
            {Good, "Good"},
            {Bad, "Bad"},
            {Ugly, "Ugly"}
        };


        public override string ToString()
        {
            return ToStringMap[this];
        }

        public static CowboyType FromString(string value)
        {
            if (value == null) throw new ArgumentNullException("value");

            var result = All().FirstOrDefault(x => x.ToString() == value);
            if (result != null) return result;

            throw new ArgumentOutOfRangeException("value", value, "Invalid CowboyType");
        }

        public static explicit operator int(CowboyType value)
        {
            var map = new Dictionary<CowboyType, int>
            {
                {Good, 0},
                {Bad, 1},
                {Ugly, 2}
            };

            return map[value];            
        }

        public static explicit operator CowboyType(int value)
        {
            var result = All().FirstOrDefault(x => (int) x == value);
            if (result != null) return result;

            throw new InvalidCastException("The value " + value + " is not a valid CowboyType");
        }

    }
}
