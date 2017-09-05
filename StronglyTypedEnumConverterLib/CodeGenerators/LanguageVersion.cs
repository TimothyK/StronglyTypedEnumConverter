using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StronglyTypedEnumConverter
{

    public class LanguageVersion
    {

        private LanguageVersion(string name, int value)
        {
            _name = name;
            _value = value;
        }

        #region Members

        public static readonly LanguageVersion CSharp50 = new LanguageVersion("C# 5.0", 50);
        public static readonly LanguageVersion CSharp60 = new LanguageVersion("C# 6.0", 60);
        public static readonly LanguageVersion CSharp70 = new LanguageVersion("C# 7.0", 70);

        #endregion


        #region All

        public static IEnumerable<LanguageVersion> All()
        {
            return All<LanguageVersion>();
        }

        private static IEnumerable<T> All<T>()
        {
            var type = typeof(T);
            return type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.IsInitOnly)
                .Where(field => field.FieldType == type)
                .Select(field => field.GetValue(null))
                .Cast<T>();
        }

        #endregion


        #region To/From String

        private readonly string _name;
        public override string ToString()
        {
            return _name;
        }

        public static LanguageVersion FromString(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var result = All().FirstOrDefault(x => x.ToString() == value);
            if (result != null) return result;

            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid LanguageVersion");
        }

        #endregion


        #region Cast to/from Underlying Type

        private readonly int _value;
        public static explicit operator int(LanguageVersion value)
        {
            return value._value;
        }

        public static explicit operator LanguageVersion(int value)
        {
            var result = All().FirstOrDefault(x => (int)x == value);
            if (result != null) return result;

            throw new InvalidCastException("The value " + value + " is not a valid LanguageVersion");
        }

        #endregion

        #region Ordinal Operators

        public static bool operator <(LanguageVersion lhs, LanguageVersion rhs) => (int) lhs < (int) rhs;

        public static bool operator >(LanguageVersion lhs, LanguageVersion rhs) => (int) lhs > (int) rhs;

        public static bool operator <=(LanguageVersion lhs, LanguageVersion rhs) => (int) lhs <= (int) rhs;
        public static bool operator >=(LanguageVersion lhs, LanguageVersion rhs) => (int) lhs >= (int) rhs;

        #endregion

    }
}
