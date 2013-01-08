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
using CodeTitans.JSon.Objects.Mutable;

namespace CodeTitans.JSon.Objects
{
    /// <summary>
    /// Internal wrapper class that describes JSON boolean value.
    /// </summary>
    internal class JSonBooleanObject : JSonDecimalObject, IJSonWritable
    {
        private bool _data;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonBooleanObject(Boolean value)
        {
            _data = value;
        }

        /// <summary>
        /// Gets or sets the value of internal data.
        /// </summary>
        protected Boolean Data
        {
            get { return _data; }
            set { _data = value; }
        }

        protected override IJSonMutableObject GetMutableCopy()
        {
            return new JSonMutableBooleanObject(_data);
        }

        protected override IJSonObject GetImmutableCopy()
        {
            return new JSonBooleanObject(_data);
        }

        protected override string GetStringValue()
        {
            return _data ? JSonReader.TrueString : JSonReader.FalseString;
        }

        protected override int GetInt32Value()
        {
            return _data ? 1: 0;
        }

        protected override uint GetUInt32Value()
        {
            return _data ? 1u : 0u;
        }

        protected override long GetInt64Value()
        {
            return _data ? 1L : 0L;
        }

        protected override ulong GetUInt64Value()
        {
            return _data ? 1UL: 0UL;
        }

        protected override float GetSingleValue()
        {
            return _data ? 1.0f : 0.0f;
        }

        protected override double GetDoubleValue()
        {
            return _data ? 1.0d : 0.0d;
        }

        protected override decimal GetDecimalValue()
        {
            return _data ? decimal.One : decimal.Zero;
        }

        protected override bool GetBooleanValue()
        {
            return _data;
        }

        protected override object GetObjectValue()
        {
            return _data;
        }

        #region IJSonWritable Members

        void IJSonWritable.Write(IJSonWriter output)
        {
            // writes current decimal value as boolean:
            output.WriteValue(_data);
        }

        #endregion
    }
}
