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
    /// Generic exception thrown by a JSON reader or writer.
    /// </summary>
    [Serializable]
    public class JSonException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public JSonException()
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonException(string message, Exception innerException)
            : base (message, innerException)
        {
        }

#if !PocketPC && !WINDOWS_PHONE && !SILVERLIGHT && !WINDOWS_STORE
        /// <summary>
        /// Constructor required by serialization.
        /// </summary>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        protected JSonException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            // PH: NOTE: as for now, I don't see a reason, why to also serialize other properties of this class beside message...
        }
#endif
    }
}
