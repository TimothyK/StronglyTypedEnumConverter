using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StronglyTypedEnumConverter
{

    public class LanguageVersion
    {
        public int Major { get; }
        public int Minor { get; }

        private LanguageVersion(int major, int minor)
        {
            Major = major;
            Minor = minor;
            _value = major * 10 + minor;
        }

        #region Members

        public static readonly LanguageVersion CSharp50 = new LanguageVersion(5, 0);
        public static readonly LanguageVersion CSharp60 = new LanguageVersion(6, 0);
        public static readonly LanguageVersion CSharp70 = new LanguageVersion(7, 0);
        public static readonly LanguageVersion CSharp71 = new LanguageVersion(7, 1);
        public static LanguageVersion Max => All().OrderBy(x => (int) x).Last();

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

        public override string ToString() => $"C# {Major}.{Minor}";

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
