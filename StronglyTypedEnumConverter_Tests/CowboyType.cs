using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

class CowboyType : IComparable<CowboyType>
{

    private CowboyType(string name, int value)
    {
        _name = name;
        _value = value;
    }

    #region Members

    public static readonly CowboyType Good = new CowboyType(nameof(Good), 0);
    public static readonly CowboyType Bad = new CowboyType(nameof(Bad), 1);
    public static readonly CowboyType Ugly = new CowboyType(nameof(Ugly), 2);

    #endregion


    #region All

    public static IEnumerable<CowboyType> All() => All<CowboyType>();

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

    public override string ToString() => _name;

    public static CowboyType FromString(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        var result = All().FirstOrDefault(x => x.ToString() == value);
        if (result != null) return result;

        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid CowboyType");
    }

    #endregion


    #region Cast to/from Underlying Type

    private readonly int _value;
    public static explicit operator int(CowboyType value) => value._value;

    public static explicit operator CowboyType(int value)
    {
        var result = All().FirstOrDefault(x => (int)x == value);
        if (result != null) return result;

        throw new InvalidCastException("The value " + value + " is not a valid CowboyType");
    }

    #endregion

    #region IComparable

    public int CompareTo(CowboyType other)
    {
        var results = new[]
        {
            ((int) this).CompareTo((int) other)
        };
        return results
            .SkipWhile(diff => diff == 0)
            .FirstOrDefault();
    }

    public static bool operator <(CowboyType lhs, CowboyType rhs) => lhs.CompareTo(rhs) < 0;

    public static bool operator >(CowboyType lhs, CowboyType rhs) => lhs.CompareTo(rhs) > 0;

    public static bool operator <=(CowboyType lhs, CowboyType rhs) => lhs.CompareTo(rhs) <= 0;
    public static bool operator >=(CowboyType lhs, CowboyType rhs) => lhs.CompareTo(rhs) >= 0;

    #endregion

}
