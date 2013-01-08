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
    /// Standard bayeux extension to pass user' name and password along with the handshake request.
    /// </summary>
    public sealed class BayeuxHandshakeExtension : IJSonWritable
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxHandshakeExtension(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) || !string.IsNullOrEmpty(password))
                Credentials = new BayeuxCredentials(userName, password);
        }

        #region Properties

        /// <summary>
        /// Gets the user's credentials when accessing the server.
        /// </summary>
        public BayeuxCredentials Credentials
        {
            get;
            private set;
        }

        #endregion

        #region Implementation of IJSonWritable

        /// <summary>
        /// Serializes an object as a JSON formatted string.
        /// </summary>
        public void Write(IJSonWriter output)
        {
            using (output.WriteObject())
            {
                if (Credentials != null)
                {
                    output.WriteMember("authentication");
                    Credentials.Write(output);
                }
            }
        }

        #endregion
    }
}
