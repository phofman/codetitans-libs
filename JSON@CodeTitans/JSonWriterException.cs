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

namespace CodeTitans.JSon
{
    /// <summary>
    /// Exception thrown when writing data into a JSON string.
    /// It indicates the state problems during last operation, i.e.: inconsistent calls like open object with a try to close as an array.
    /// </summary>
    [Serializable]
    public sealed class JSonWriterException : JSonException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public JSonWriterException()
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonWriterException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonWriterException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonWriterException(string message, JSonWriterTokenType expected, JSonWriterTokenType spot)
            : base(message)
        {
            ExpectedToken = expected;
            SpotToken = spot;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonWriterException(JSonWriterTokenType expected, JSonWriterTokenType spot)
            : this(expected, spot, true)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonWriterException(JSonWriterTokenType expected, JSonWriterTokenType spot, bool equals)
            : this(string.Format(CultureInfo.InvariantCulture, "Expected {0}: {1}, while spot: {2}.", equals ? "token" : "different token than", expected, spot), expected, spot)
        {
        }

#if !PocketPC && !WINDOWS_PHONE && !SILVERLIGHT && !WINDOWS_STORE
        /// <summary>
        /// Init constructor.
        /// </summary>
        private JSonWriterException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base (info, context)
        {
        }
#endif

        /// <summary>
        /// Expected token during writing.
        /// </summary>
        public JSonWriterTokenType ExpectedToken
        { get; private set; }

        /// <summary>
        /// Token spot at current writing operation.
        /// </summary>
        public JSonWriterTokenType SpotToken
        { get; private set; }
    }
}
