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
    /// Exception thrown by <see cref="JSonReader"/> class.
    /// It indicates the token, that caused it and place inside the input string.
    /// </summary>
    [Serializable]
    public sealed class JSonReaderException : JSonException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public JSonReaderException()
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonReaderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonReaderException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonReaderException(string message, string token, int line, int offset, Exception innerException)
            : base(PrepareMessage(message, token, line, offset), innerException)
        {
            InvalidToken = token;
            Line = line;
            Offset = offset;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonReaderException(string message, string token, int line, int offset)
            : this(message, token, line, offset, null)
        {
        }

#if !PocketPC && !WINDOWS_PHONE && !SILVERLIGHT && !WINDOWS_STORE
        /// <summary>
        /// Init constructor.
        /// </summary>
        private JSonReaderException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base (info, context)
        {
        }
#endif

        /// <summary>
        /// Internal init constuctor.
        /// </summary>
        internal JSonReaderException(string message, JSonReaderTokenInfo token)
            : base(PrepareMessage(message, token))
        {
            InvalidToken = token != null ? token.Text : string.Empty;
            Line = token != null ? token.Line : 0;
            Offset = token != null ? token.Offset : 0;
        }

        #region Properties

        /// <summary>
        /// Token caused the exception to be thrown.
        /// </summary>
        public string InvalidToken
        { get; private set; }

        /// <summary>
        /// Line in input source, where invalid token was spot.
        /// </summary>
        public int Line
        { get; private set; }

        /// <summary>
        /// Char offset inside the line of source code, where invalid token was spot.
        /// </summary>
        public int Offset
        { get; private set; }

        #endregion

        #region Message Preparation

        private static string PrepareMessage(string message, string token, int line, int offset)
        {
            if (!string.IsNullOrEmpty(message))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} ('{1}' at {2}:{3})", message, token, line, offset);
            }

            return string.Format(CultureInfo.InvariantCulture, "Token '{0}' at {1}:{2}", token, line, offset);
        }

        private static string PrepareMessage(string message, JSonReaderTokenInfo token)
        {
            if (token != null)
                return PrepareMessage(message, token.Text, token.Line, token.Offset);

            return PrepareMessage(message, string.Empty, 0, 0);
        }

        #endregion
    }
}
