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
    /// Kinds of DateTime conversions, when read from JSON string or decimal.
    /// </summary>
    public enum JSonDateTimeKind
    {
        /// <summary>
        /// Default behavior.
        /// </summary>
        Default,
        /// <summary>
        /// Expected decimal value with Unix Epoch time in seconds.
        /// In case of text formats fallback into Default behavior.
        /// </summary>
        UnixEpochSeconds,
        /// <summary>
        /// Expected decimal value with Unix Epoch time in milliseconds.
        /// In case of text formats fallback into Default behavior.
        /// </summary>
        UnixEpochMilliseconds,
        /// <summary>
        /// Expected decimal value with ticks.
        /// In case of text formats fallback into Default behavior.
        /// </summary>
        Ticks
    }
}
