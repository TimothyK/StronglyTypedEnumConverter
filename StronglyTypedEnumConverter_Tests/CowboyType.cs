using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

class CowboyType
{

    private CowboyType() { }

    #region Members

    public static readonly CowboyType Good = new CowboyType();
    public static readonly CowboyType Bad = new CowboyType();
    public static readonly CowboyType Ugly = new CowboyType();

    #endregion


    #region All

    public static IEnumerable<CowboyType> All()
    {
        return All<CowboyType>();
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

    #endregion


    #region Cast to/from Underlying Type

    private static readonly Dictionary<CowboyType, int> UnderlyingMap = new Dictionary<CowboyType, int>
    {
        {Good, 0},
        {Bad, 1},
        {Ugly, 2}
    };

    public static explicit operator int(CowboyType value)
    {
        return UnderlyingMap[value];
    }

    public static explicit operator CowboyType(int value)
    {
        var result = All().FirstOrDefault(x => (int)x == value);
        if (result != null) return result;

        throw new InvalidCastException("The value " + value + " is not a valid CowboyType");
    }

    #endregion

}
