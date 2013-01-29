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

namespace CodeTitans.JSon
{
    /// <summary>
    /// Exception class thrown when 
    /// </summary>
    [Serializable]
    public sealed class JSonMemberMissingException : JSonException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public JSonMemberMissingException()
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberMissingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberMissingException(string message, Type objectType, string memberName)
            : base (message)
        {
            ObjectType = objectType;
            MemberName = memberName;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberMissingException(string message, Exception exception)
            : base (message, exception)
        {
        }

#if !PocketPC && !WINDOWS_PHONE && !SILVERLIGHT && !WINDOWS_STORE
        /// <summary>
        /// Init constructor.
        /// </summary>
        private JSonMemberMissingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base (info, context)
        {
        }
#endif

        #region Properties

        /// <summary>
        /// Gets the type of the object that is being processed.
        /// </summary>
        public Type ObjectType
        { get; private set; }

        /// <summary>
        /// Gets the name of missing member.
        /// </summary>
        public string MemberName
        { get; private set; }

        #endregion
    }
}
