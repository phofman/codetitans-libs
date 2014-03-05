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

using CodeTitans.JSon;

namespace CodeTitans.Bayeux
{
    /// <summary>
    /// Class wrapping credentials info for bayeux handshake request.
    /// </summary>
    public sealed class BayeuxCredentials : IJSonWritable
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Gets the user's name.
        /// </summary>
        public string UserName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the password to access the remote service.
        /// </summary>
        public string Password
        {
            get;
            private set;
        }

        #region Implementation of IJSonWritable

        /// <summary>
        /// Serializes an object as a JSON formatted string.
        /// </summary>
        public void Write(IJSonWriter output)
        {
            output.WriteObjectBegin();
            {
                output.WriteMember("user", UserName);
                output.WriteMember("credentials", Password);
            }
            output.WriteObjectEnd();
        }

        #endregion
    }
}
