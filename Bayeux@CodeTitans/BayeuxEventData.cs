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
using CodeTitans.JSon;

namespace CodeTitans.Bayeux
{
    /// <summary>
    /// Wrapper class that could be used as a JSON object.
    /// </summary>
    public class BayeuxEventData : IJSonWritable
    {
        private readonly Dictionary<string, object> _data;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxEventData()
        {
            _data = new Dictionary<string, object>();
        }

        /// <summary>
        /// Init constructor, where all the &lt;key,value&gt; pairs are passed at once.
        /// </summary>
        public BayeuxEventData(string key, params object[] data)
        {
            if (data == null || data.Length % 2 == 0)
                throw new ArgumentException("Invalid number of arguments");

            _data = new Dictionary<string, object>();

            Set(key, data[0]);
            for (int i = 1; i < data.Length; i += 2)
            {
                if (data[i] == null)
                    throw new ArgumentNullException("data[" + i + "]");

                Set(data[i].ToString(), data[i + 1]);
            }
        }

        /// <summary>
        /// Sets the given value.
        /// </summary>
        public void Set(string key, object value)
        {
            if (_data.ContainsKey(key))
                _data[key] = value;
            else
                _data.Add(key, value);
        }

        #region IJSonWritable Members

        /// <summary>
        /// Write as JSON.
        /// </summary>
        public void Write(IJSonWriter output)
        {
            // write as an object (as _data implements IDictionary interface):
            output.Write(_data);
        }

        #endregion
    }
}
