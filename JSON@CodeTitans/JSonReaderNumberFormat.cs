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

namespace CodeTitans.JSon
{
    /// <summary>
    /// Enumeration defining, what data type should be used, to hold information about numbers.
    /// This can have an mostly on rounding and some impact on memory footprint.
    /// However, when a given format is forced, and the value number can't be parsed to fit into that format,
    /// the whole JSON parsing will faild with an exception (i.e. forcing AsInt32 and receiving decimal value).
    /// </summary>
    public enum JSonReaderNumberFormat
    {
        /// <summary>
        /// Default behaviour. If number can be stored as Int64 or UInt64, this type is used, otherwise Double.
        /// </summary>
        Default,
        /// <summary>
        /// Force all numeric values parsed to be an Int32.
        /// </summary>
        AsInt32,
        /// <summary>
        /// Force all numeric values parsed to be an Int64.
        /// </summary>
        AsInt64,
        /// <summary>
        /// Force all numeric values to be Double.
        /// </summary>
        AsDouble,
        /// <summary>
        /// Force all numeric values to be Decimal.
        /// </summary>
        AsDecimal
    }
}
