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
using System.Globalization;
using CodeTitans.JSon.Objects.Mutable;

namespace CodeTitans.JSon.Objects
{
    internal class JSonDecimalDecimalObject : JSonDecimalObject, IJSonWritable
    {
        private Decimal _data;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonDecimalDecimalObject(Decimal data)
        {
            _data = data;
        }

        /// <summary>
        /// Gets or sets the value of internal data.
        /// </summary>
        protected Decimal Data
        {
            get { return _data; }
            set { _data = value; }
        }

        protected override IJSonMutableObject GetMutableCopy()
        {
            return new JSonMutableDecimalDecimalObject(_data);
        }

        protected override IJSonObject GetImmutableCopy()
        {
            return new JSonDecimalDecimalObject(_data);
        }

        protected override string GetStringValue()
        {
            return _data.ToString(CultureInfo.InvariantCulture);
        }

        protected override int GetInt32Value()
        {
            return (int)_data;
        }

        protected override uint GetUInt32Value()
        {
            return (uint)_data;
        }

        protected override long GetInt64Value()
        {
            return (long)_data;
        }

        protected override ulong GetUInt64Value()
        {
            return (ulong)_data;
        }

        protected override float GetSingleValue()
        {
            return (float)_data;
        }

        protected override double GetDoubleValue()
        {
            return (double)_data;
        }

        protected override decimal GetDecimalValue()
        {
            return _data;
        }

        protected override bool GetBooleanValue()
        {
            return Math.Abs(_data) > EpsilonDecimal;
        }

        protected override object GetObjectValue()
        {
            return _data;
        }

        #region IJSonWritable Members

        void IJSonWritable.Write(IJSonWriter output)
        {
            output.WriteValue(_data);
        }

        #endregion
    }
}