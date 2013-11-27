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
using CodeTitans.Helpers;
using CodeTitans.JSon.Objects.Mutable;

namespace CodeTitans.JSon.Objects
{
    internal class JSonDecimalInt64Object : JSonDecimalObject, IJSonWritable
    {
        private Int64 _data;
        private string _stringRepresentation;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonDecimalInt64Object(Int64 data)
        {
            _data = data;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonDecimalInt64Object(UInt64 data)
        {
            _data = (Int64) data;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonDecimalInt64Object(Int64 data, string stringRepresentation)
        {
            _data = data;
            _stringRepresentation = stringRepresentation;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonDecimalInt64Object(DateTime date, JSonDateTimeKind kind)
        {
            _data = DateTimeHelper.ToNumber(date, kind);
            _stringRepresentation = JSonWriter.ToString(date);
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonDecimalInt64Object(TimeSpan time)
        {
            _data = time.Ticks;
            _stringRepresentation = JSonWriter.ToString(time);
        }

        /// <summary>
        /// Gets or sets the value of internal data.
        /// </summary>
        protected internal Int64 Data
        {
            get { return _data; }
            set
            {
                _data = value;
                _stringRepresentation = null;
            }
        }

        protected override IJSonMutableObject GetMutableCopy()
        {
            return new JSonMutableDecimalInt64Object(_data, _stringRepresentation);
        }

        protected override IJSonObject GetImmutableCopy()
        {
            return new JSonDecimalInt64Object(_data, _stringRepresentation);
        }

        /// <summary>
        /// Gets or sets the string representation of this object.
        /// </summary>
        protected internal string StringRepresentation
        {
            get { return _stringRepresentation; }
            set { _stringRepresentation = value; }
        }

        protected override string GetStringValue()
        {
            if (_stringRepresentation != null)
                return _stringRepresentation;

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
            return _data;
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
            return _data != 0;
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