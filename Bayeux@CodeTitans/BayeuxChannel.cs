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

namespace CodeTitans.Bayeux
{
    /// <summary>
    /// Wrapper class over Bayeux channel name.
    /// </summary>
    public sealed class BayeuxChannel
    {
        /// <summary>
        /// Special marks that are allowed in channel name.
        /// </summary>
        public const string MetaMarks = "-_!~()$@/";

        private readonly string _channel;
        private readonly string[] _segments;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxChannel(string channel)
        {
            if (string.IsNullOrEmpty(channel))
                throw new ArgumentNullException("channel");

            if (!IsValid(channel))
                throw new ArgumentException("Channel seems to be invalid due to standard validation checks", "channel");

            _channel = channel;
            _segments = channel.Split('/');
        }

        #region Properties

        /// <summary>
        /// Gets the full name of the channel.
        /// </summary>
        public string FullName
        {
            get { return _channel; }
        }

        /// <summary>
        /// Gets the segments of the channel.
        /// </summary>
        public ICollection<string> Segments
        {
            get { return _segments; }
        }

        #endregion

        /// <summary>
        /// Gets the string representation of this object.
        /// </summary>
        public override string ToString()
        {
            return _channel;
        }

        /// <summary>
        /// Returns 'true' if given text is a valid Bayeux channel name.
        /// It should consist of number letters or digits separated by '/'.
        /// </summary>
        public static bool IsValid(string channel)
        {
            if (string.IsNullOrEmpty(channel))
                return false;

            foreach (char c in channel)
            {
                if (!char.IsLetterOrDigit(c) && MetaMarks.IndexOf(c) == -1)
                    return false;
            }

            return true;
        }
    }
}
