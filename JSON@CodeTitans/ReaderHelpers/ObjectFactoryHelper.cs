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
using CodeTitans.Helpers;

namespace CodeTitans.JSon.ReaderHelpers
{
    /// <summary>
    /// Helper class for ObjectFactories.
    /// </summary>
    internal static class ObjectFactoryHelper
    {
        /// <summary>
        /// Tries to parse a string into a number.
        /// First it tries to parse as Int64 or UInt64 and if the number 
        /// </summary>
        internal static object ParseNumber(IObjectFactory factory, string number)
        {
            Int64 resultInt64;
            UInt64 resultUInt64;
            double resultDouble;

            // if the number starts with '-' sign, then it might be Int64, also when it fits the range of Int64 values, we prefere Int64 to be used;
            // otherwise try UInt64, finally if both reading failed, assume it's Double:

            if (number.Length > 0 && number.IndexOf('.') == -1 && number.IndexOf('e') == -1 && number.IndexOf('E') == -1)
            {
                if (NumericHelper.TryParseInt64(number, out resultInt64) && string.Compare(resultInt64.ToString(CultureInfo.InvariantCulture), number, StringComparison.Ordinal) == 0)
                    return factory.CreateNumber(resultInt64);

                if (number[0] != '-' && NumericHelper.TryParseUInt64(number, out resultUInt64) &&
                    string.Compare(resultUInt64.ToString(CultureInfo.InvariantCulture), number, StringComparison.Ordinal) == 0)
                    return factory.CreateNumber(resultUInt64);
            }

            if (NumericHelper.TryParseDouble(number, NumberStyles.Float, out resultDouble))
                return factory.CreateNumber(resultDouble);

            return null;
        }

        /// <summary>
        /// Parse string as 'Decimal' and return a factory specific number wrapper.
        /// </summary>
        internal static object ParseDecimal(IObjectFactory factory, string data)
        {
            Decimal number;

            if (NumericHelper.TryParseDecimal(data, NumberStyles.Float, out number))
                return factory.CreateNumber(number);
            return null;
        }

        /// <summary>
        /// Parse string as 'Double' and return a factory specific number wrapper.
        /// </summary>
        internal static object ParseDouble(IObjectFactory factory, string data)
        {
            Double number;

            if (NumericHelper.TryParseDouble(data, NumberStyles.Float, out number))
                return factory.CreateNumber(number);
            return null;
        }

        /// <summary>
        /// Parse string as 'Int64' and return a factory specific number wrapper.
        /// </summary>
        internal static object ParseInt64(IObjectFactory factory, string data)
        {
            Int64 number;

            if (NumericHelper.TryParseInt64(data, out number))
                return factory.CreateNumber(number);
            return null;
        }

        /// <summary>
        /// Parse string as 'Int32' and return a factory specific number wrapper.
        /// </summary>
        internal static object ParseInt32(IObjectFactory factory, string data)
        {
            Int32 number;

            if (NumericHelper.TryParseInt32(data, out number))
                return factory.CreateNumber(number);
            return null;
        }
    }
}
