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
    /// Enumeration defining possible DateTime formats when serializing.
    /// </summary>
    public enum JSonWriterDateFormat
    {
        /// <summary>
        /// Serialize date in universal date-time pattern.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Serialize in JavaScript pattern ("/Date()/").
        /// </summary>
        JavaScript,
        /// <summary>
        /// Serialize date as a number of seconds since 1970-01-01.
        /// </summary>
        UnixEpochSeconds,
        /// <summary>
        /// Serialize date as a number of miliseconds since 1970-01-01.
        /// </summary>
        UnixEpochMilliseconds,
        /// <summary>
        /// Serizalize date as a number of ticks.
        /// </summary>
        Ticks
    }
}
