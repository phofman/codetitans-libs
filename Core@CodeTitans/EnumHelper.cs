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

using CodeTitans.Helpers;
using System;
using System.ComponentModel;

namespace CodeTitans.Core
{
    /// <summary>
    /// Helper class extending Enum functionality, common for all supported platforms.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Parses the given enum type to search for item with matching field's name.
        /// Additional names can be added by marking enum fields with DescriptionAttribute or DisplayNameAttribute.
        /// If name is not found, the ArgumentOutOfRangeException is thrown.
        /// </summary>
        public static T Parse<T>(string text) where T : struct
        {
            T value;

            if (TryParse(text, out value))
                return value;

            throw new ArgumentOutOfRangeException("text", "Invalid value: '" + text + "' for enum " + typeof(T).Name);
        }

        /// <summary>
        /// Tries to parse the given enum type to search for item with matching field's name.
        /// Additional names can be added by marking enum fields with DescriptionAttribute or DisplayNameAttribute.
        /// Returns 'true', when item with matching name was found, otherwise 'false'.
        /// </summary>
        public static bool TryParse<T>(string text, out T value) where T : struct
        {
            foreach (var fieldInfo in ReflectionHelper.GetFields(typeof(T)))
            {
#if !PocketPC
                // search using 'description':
                var descriptionAttributes = ReflectionHelper.GetCustomAttributes<DescriptionAttribute>(fieldInfo, false);

                foreach (var attribute in descriptionAttributes)
                    if (string.Compare(attribute.Description, text, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        value = (T)fieldInfo.GetValue(null);
                        return true;
                    }
#endif

#if !PocketPC && !NET_2_COMPATIBLE && !WINDOWS_PHONE && !SILVERLIGHT && !WINDOWS_STORE
                // search using 'display-name':
                var displayAttributes = (DisplayNameAttribute[]) fieldInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);

                foreach (var attribute in displayAttributes)
                    if (string.Compare(attribute.DisplayName, text, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        value = (T) fieldInfo.GetValue(null);
                        return true;
                    }
            }

            return Enum.TryParse(text, true, out value);

#elif !WINDOWS_PHONE && SILVERLIGHT
            }

            return Enum.TryParse(text, true, out value);
#else
            }

            try
            {
                value = (T)Enum.Parse(typeof(T), text, true);
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
#endif
        }

#if !NET_2_COMPATIBLE
        /// <summary>
        /// Gets the alternative name for the enum type value.
        /// If multiple DescriptionAttributes or DisplayNameAttributes are used, than the name is really random.
        /// </summary>
        public static string GetDisplayName(this Enum e)
        {
            if (e == null)
                return null;

            string description = e.ToString();

            var fieldInfo = ReflectionHelper.GetField(e.GetType(), description);
            var descriptionAttributes = ReflectionHelper.GetCustomAttributes<DescriptionAttribute>(fieldInfo, false);

            // if there is at least one description attribute, return the value:
            foreach (var attr in descriptionAttributes)
                return attr.Description;

#if !WINDOWS_PHONE && !SILVERLIGHT && !WINDOWS_STORE
            var displayAttributes = (DisplayNameAttribute[]) fieldInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            return displayAttributes.Length > 0 ? displayAttributes[0].DisplayName : null;
#else
            return null;
#endif
        }
#endif // !NET_2_COMPATIBLE
    }
}
