#region License
/*
    Copyright (c) 2010, Pawe³ Hofman (CodeTitans)
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
    internal class JSonDecimalSingleObject : JSonDecimalObject, IJSonWritable
    {
        private Single _data;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonDecimalSingleObject(Single data)
        {
            _data = data;
        }

        /// <summary>
        /// Gets or sets the value of internal data.
        /// </summary>
        protected Single Data
        {
            get { return _data; }
            set { _data = value; }
        }

        protected override IJSonMutableObject GetMutableCopy()
        {
            return new JSonMutableDecimalSingleObject(_data);
        }

        protected override IJSonObject GetImmutableCopy()
        {
            return new JSonDecimalSingleObject(_data);
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
            return _data;
        }

        protected override double GetDoubleValue()
        {
            return _data;
        }

        protected override decimal GetDecimalValue()
        {
            return new Decimal(_data);
        }

        protected override bool GetBooleanValue()
        {
            return Math.Abs(_data) > EpsilonSingle;
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