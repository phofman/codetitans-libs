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

#if !CODETITANS_LIB_CORE
namespace CodeTitans.Bayeux
#else
namespace CodeTitans.Core.Net
#endif
{
    /// <summary>
    /// Class providing better suited responses for Bayeux protocol implemenation.
    /// It carries the IJSonObject along and uses it in place of AsString representation, what is helpful if part of response must be update at-hoc.
    /// </summary>
    public sealed class RecordedBayeuxDataSourceResponse : RecordedDataSourceResponse
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public RecordedBayeuxDataSourceResponse()
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public RecordedBayeuxDataSourceResponse(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets or sets the response content as string.
        /// If there is none, the AsJSonObject is used.
        /// </summary>
        public override string AsString
        {
            get
            {
                if (base.AsString != null)
                    return base.AsString;

                return AsJSon != null ? AsJSon.ToString("c") : null;
            }
            set { base.AsString = value; }
        }

        /// <summary>
        /// Gets or sets the JSON representation of the response.
        /// </summary>
        public IJSonObject AsJSon
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the 'channel' field of the bayeux message.
        /// </summary>
        public string Channel
        {
            get { return GetChannel(AsJSon); }
        }

        /// <summary>
        /// Gets the 'channel' field of the bayeux message or null if not invalid format.
        /// </summary>
        internal static string GetChannel(IJSonObject o)
        {
            if (o == null)
                return null;

            if (!o.IsEnumerable)
                return null;

            if (o.IsArray)
            {
                foreach (var item in o.ArrayItems)
                {
                    if (item.IsEnumerable && !item.IsArray)
                        return item["channel", (string)null].StringValue;
                }

                // an empty array or no JSON objects items of this array...
                return null;
            }

            return o["channel", (string)null].StringValue;
        }
    }
}
