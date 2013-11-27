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
using System.Collections.Generic;
using System.Globalization;

namespace CodeTitans.Bayeux
{
    /// <summary>
    /// Wrapper class over Bayeux error.
    /// </summary>
    public sealed class BayeuxError
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxError(string errorString)
        {
            if (errorString == null)
                throw new ArgumentNullException("errorString");

            int colonIndex = errorString.IndexOf(':');

            if (colonIndex < 0)
            {
                // no colon, whole text is a message only:
                Message = errorString;
                Arguments = new string[0];
            }
            else
            {
                // first colon should be after 3 digits error code:
                if (colonIndex != 3)
                    throw new FormatException("Invalid length of error code");

                // try to parse value, if it's not a number, exception will be thrown:
                Code = Int32.Parse(errorString.Substring(0, 3), CultureInfo.InvariantCulture);

                if (errorString.Length > 4)
                {
                    colonIndex = errorString.IndexOf(':', 4);
                    if (colonIndex < 0)
                        throw new FormatException("Second colon missing");

                    Arguments = colonIndex == 4 ? new string[0] : errorString.Substring(4, colonIndex - 4).Split(',');
                    Message = errorString.Substring(colonIndex + 1);
                }
                else
                {
                    Arguments = new string[0];
                }
            }
        }

        #region Properties

        /// <summary>
        /// Gets error message.
        /// </summary>
        public string Message
        { get; private set; }

        /// <summary>
        /// Gets the error code associated with this message.
        /// </summary>
        public int Code
        { get; private set; }

        /// <summary>
        /// Gets the list of arguments associated with this error.
        /// </summary>
        public IList<string> Arguments
        { get; private set; }

        #endregion
    }
}
