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
using CodeTitans.JSon;

namespace CodeTitans.Bayeux.Responses
{
    /// <summary>
    /// Generic handshake response.
    /// </summary>
    internal class HandshakeResponse : BayeuxResponse
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public HandshakeResponse(IJSonObject data)
            : base (data)
        {
        }

        #region Properties

        /// <summary>
        /// Type of the connection.
        /// </summary>
        public BayeuxConnectionTypes SupportedConnectionTypes
        { get; set; }

        #endregion

        protected override void ReadOptionalFields(IJSonObject input)
        {
            // reset field values:
            SupportedConnectionTypes = BayeuxConnectionTypes.None;

            // read additional data:
            if (input.Contains("supportedConnectionTypes"))
            {
                IJSonObject supportedTypes = input["supportedConnectionTypes"];

                if (supportedTypes == null)
                    throw new MissingMemberException("Missing 'supportedConnectionTypes' field");

                if (!supportedTypes.IsArray)
                    throw new FormatException("Expected supportedConnectionTypes to be an array");

                BayeuxConnectionTypes types = BayeuxConnectionTypes.None;

                foreach (IJSonObject connectionType in supportedTypes.ArrayItems)
                {
                    types |= BayeuxConnectionTypesHelper.Parse(connectionType.StringValue);
                }
            }
        }

        protected override void WriteOptionalFields(IJSonWriter output)
        {
            output.WriteMember("supportedConnectionTypes");
            output.WriteArrayBegin();
            foreach (string type in BayeuxConnectionTypesHelper.ToCollection(SupportedConnectionTypes))
                output.WriteValue(type);
            output.WriteArrayEnd();
        }
    }
}
