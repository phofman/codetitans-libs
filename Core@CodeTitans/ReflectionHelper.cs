#region License
/*
    Copyright (c) 2010, Paweł Hofman (CodeTitans)
    All Rights Reserved.

    Licensed under the Apache License version 2.0.
    For more information please visit:

    http://codetitans.codeplex.com/license
        or
    http://www.apache.org/licenses/


    For latest source code, documentation, samples
    and more information please visit:

    http://codetitans.codeplex.com/
*/
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeTitans.Helpers
{
    /// <summary>
    /// Class with helper methods of accessing metadata through reflection on different platforms.
    /// </summary>
    internal static class ReflectionHelper
    {
        /// <summary>
        /// Returns a custom attribute of required type or null.
        /// </summary>
        public static T GetCustomAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
        {
#if WINDOWS_STORE
            return propertyInfo.GetCustomAttribute<T>();
#else
            return (T)GetCustomAttribute(propertyInfo, typeof(T));
#endif
        }

        /// <summary>
        /// Returns a custom attribute of required type or null.
        /// </summary>
        public static Attribute GetCustomAttribute(PropertyInfo propertyInfo, Type attributeType)
        {
#if WINDOWS_STORE
            return propertyInfo.GetCustomAttribute(attributeType);
#else
            return Attribute.GetCustomAttribute(propertyInfo, attributeType);
#endif
        }

        /// <summary>
        /// Returns a custom attribute of required type or null.
        /// </summary>
        public static T GetCustomAttribute<T>(Type type) where T : Attribute
        {
#if WINDOWS_STORE
            return type.GetTypeInfo().GetCustomAttribute<T>();
#else
            return (T)Attribute.GetCustomAttribute(type, typeof(T));
#endif
        }

        /// <summary>
        /// Returns a custom attribute of required type or null.
        /// </summary>
        public static T GetCustomAttribute<T>(FieldInfo fieldInfo) where T : Attribute
        {
#if WINDOWS_STORE
            return fieldInfo.GetCustomAttribute<T>();
#else
            return (T)Attribute.GetCustomAttribute(fieldInfo, typeof(T));
#endif
        }

        /// <summary>
        /// Returns a custom attributes enumeration of required type or null.
        /// </summary>
        public static IEnumerable<T> GetCustomAttributes<T>(FieldInfo fieldInfo, bool inherit) where T : Attribute
        {
#if WINDOWS_STORE
            return fieldInfo.GetCustomAttributes<T>(inherit);
#else
            return (IEnumerable<T>) fieldInfo.GetCustomAttributes(typeof(T), inherit);
#endif
        }

        /// <summary>
        /// Gets an indication, if type is generics.
        /// </summary>
        public static bool IsGenericType(Type type)
        {
#if WINDOWS_STORE
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        /// <summary>
        /// Gets the first generic argument of a given generic type.
        /// </summary>
        public static Type GetFirstGenericArgument(Type type)
        {
#if WINDOWS_STORE
            return type.GenericTypeArguments[0];
#else
            return type.GetGenericArguments()[0];
#endif
        }

#if WINDOWS_STORE
        /// <summary>
        /// Gets a collection of fields for a given type.
        /// </summary>
        public static IEnumerable<FieldInfo> GetFields(Type type)
        {
            return type.GetRuntimeFields();
        }

        /// <summary>
        /// Gets a collection of properties for a given type.
        /// </summary>
        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetRuntimeProperties();
        }

        /// <summary>
        /// Gets a field of specified name for a given type.
        /// </summary>
        public static FieldInfo GetField(Type type, string name)
        {
            return type.GetRuntimeField(name);
        }
#else
        /// <summary>
        /// Gets a collection of fields for a given type.
        /// </summary>
        public static IEnumerable<FieldInfo> GetFields(Type type)
        {
            return type.GetFields();
        }

        /// <summary>
        /// Gets a collection of fields for a given type.
        /// </summary>
        public static IEnumerable<FieldInfo> GetFields(Type type, BindingFlags flags)
        {
            return type.GetFields(flags);
        }

        /// <summary>
        /// Gets a collection of properties for a given type.
        /// </summary>
        public static IEnumerable<PropertyInfo> GetProperties(Type type, BindingFlags flags)
        {
            return type.GetProperties(flags);
        }

        /// <summary>
        /// Gets a field of specified name for a given type.
        /// </summary>
        public static FieldInfo GetField(Type type, string name)
        {
            return type.GetField(name);
        }
#endif
    }
}
