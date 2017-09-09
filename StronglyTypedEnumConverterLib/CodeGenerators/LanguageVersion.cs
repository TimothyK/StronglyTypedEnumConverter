using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StronglyTypedEnumConverter
{

    public class LanguageVersion : IComparable<LanguageVersion>
    {
        public int Major { get; }
        public int Minor { get; }

        private LanguageVersion(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        #region Members

        public static readonly LanguageVersion CSharp50 = new LanguageVersion(5, 0);
        public static readonly LanguageVersion CSharp60 = new LanguageVersion(6, 0);
        public static readonly LanguageVersion CSharp70 = new LanguageVersion(7, 0);
        public static readonly LanguageVersion CSharp71 = new LanguageVersion(7, 1);
        public static LanguageVersion Max => All().OrderBy(x => x).Last();

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

        #region Comparison

        public int CompareTo(LanguageVersion other)
        {
            var results = new[]
            {
                Major.CompareTo(other.Major),
                Minor.CompareTo(other.Minor)
            };
            return results
                .SkipWhile(diff => diff == 0)
                .FirstOrDefault();
        }

        public static bool operator <(LanguageVersion lhs, LanguageVersion rhs) => lhs.CompareTo(rhs) < 0;

        public static bool operator >(LanguageVersion lhs, LanguageVersion rhs) => lhs.CompareTo(rhs) > 0;

        public static bool operator <=(LanguageVersion lhs, LanguageVersion rhs) => lhs.CompareTo(rhs) <= 0;
        public static bool operator >=(LanguageVersion lhs, LanguageVersion rhs) => lhs.CompareTo(rhs) >= 0;

        #endregion

    }
}
