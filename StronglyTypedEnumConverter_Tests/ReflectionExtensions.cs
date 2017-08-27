using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace StronglyTypedEnumConverter
{
    internal static class ReflectionExtensions
    {

        /// <summary>
        /// Returns true if the type is anonymous
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAnonymous(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && (type.Name.StartsWith("<") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        /// <summary>
        /// Returns all static readonly fields of a type, where the type of the field is the same as the containing type.  
        /// These are "members" of our strongly typed enum class.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetEnumMembers(this Type type)
        {
            return type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.IsInitOnly)
                .Where(f => f.FieldType == type);
        }

        /// <summary>
        /// Gets the values of the static readonly fields (enum members) of the strongly typed enum class.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object[] GetEnumMemberValues(this Type type)
        {
            return type.GetEnumMembers()
                .Select(f => f.GetValue(null))
                .Where(x => x != null)
                .ToArray();
        }

    }
}
