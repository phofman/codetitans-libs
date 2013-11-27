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
using System.Globalization;

namespace CodeTitans.Helpers
{
    /// <summary>
    /// Class with numeric helper methods.
    /// </summary>
    internal static class NumericHelper
    {
        /// <summary>
        /// Tries to convert a string representation into a numeric value.
        /// </summary>
        public static bool TryParseDouble(string s, NumberStyles style, out double result)
        {
#if PocketPC
            try
            {
                result = double.Parse(s, style, CultureInfo.InvariantCulture);
                return true;
            }
            catch (ArgumentException)
            {
                result = 0;
                return false;
            }
            catch (FormatException)
            {
                result = 0;
                return false;
            }
            catch (OverflowException)
            {
                result = 0;
                return false;
            }

#else
            return double.TryParse(s, style, CultureInfo.InvariantCulture, out result);
#endif
        }

        /// <summary>
        /// Tries to convert a string representation into a numeric value.
        /// </summary>
        public static bool TryParseDecimal(string s, NumberStyles style, out decimal result)
        {
#if PocketPC
            try
            {
                result = decimal.Parse(s, style, CultureInfo.InvariantCulture);
                return true;
            }
            catch (ArgumentException)
            {
                result = Decimal.Zero;
                return false;
            }
            catch (FormatException)
            {
                result = Decimal.Zero;
                return false;
            }
            catch (OverflowException)
            {
                result = Decimal.Zero;
                return false;
            }

#else
            return decimal.TryParse(s, style, CultureInfo.InvariantCulture, out result);
#endif
        }

        public static bool TryParseInt32(string s, out Int32 result)
        {
#if PocketPC
            try
            {
                result = Int32.Parse(s);
                return true;
            }
            catch (ArgumentException)
            {
                result = 0;
                return false;
            }
            catch (FormatException)
            {
                result = 0;
                return false;
            }
            catch (OverflowException)
            {
                result = 0;
                return false;
            }
#else
            return Int32.TryParse(s, out result);
#endif
        }

        public static bool TryParseUInt32(string s, out UInt32 result)
        {
#if PocketPC
            try
            {
                result = UInt32.Parse(s);
                return true;
            }
            catch (ArgumentException)
            {
                result = 0;
                return false;
            }
            catch (FormatException)
            {
                result = 0;
                return false;
            }
            catch (OverflowException)
            {
                result = 0;
                return false;
            }
#else
            return UInt32.TryParse(s, out result);
#endif
        }

        public static bool TryParseInt64(string s, out Int64 result)
        {
#if PocketPC
            try
            {
                result = Int64.Parse(s);
                return true;
            }
            catch (ArgumentException)
            {
                result = 0;
                return false;
            }
            catch (FormatException)
            {
                result = 0;
                return false;
            }
            catch (OverflowException)
            {
                result = 0;
                return false;
            }
#else
            return Int64.TryParse(s, out result);
#endif
        }

        public static bool TryParseUInt64(string s, out UInt64 result)
        {
#if PocketPC
            try
            {
                result = UInt64.Parse(s);
                return true;
            }
            catch (ArgumentException)
            {
                result = 0;
                return false;
            }
            catch (FormatException)
            {
                result = 0;
                return false;
            }
            catch (OverflowException)
            {
                result = 0;
                return false;
            }
#else
            return UInt64.TryParse(s, out result);
#endif
        }

        public static bool TryParseHexInt32(string s, out Int32 result)
        {
#if PocketPC
            try
            {
                result = int.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                return true;
            }
            catch (ArgumentException)
            {
                result = 0;
                return false;
            }
            catch (FormatException)
            {
                result = 0;
                return false;
            }
            catch (OverflowException)
            {
                result = 0;
                return false;
            }
#else
            return int.TryParse(s, NumberStyles.HexNumber, null, out result);
#endif
        }
    }
}
