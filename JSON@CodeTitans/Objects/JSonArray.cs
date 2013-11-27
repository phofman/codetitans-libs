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

namespace CodeTitans.JSon.Objects
{
    /// <summary>
    /// Internal wrapper class that describes array of IJSonObjects and provides <see cref="IJSonObject"/> access interface.
    /// </summary>
    internal class JSonArray : IJSonObject, IJSonWritable
    {
        private readonly IList<IJSonObject> _data;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonArray(ICollection<Object> data)
        {
            IJSonObject[] typedData = new IJSonObject[data.Count];

            // convert and copy data as an array:
            int i = 0;
            foreach (Object d in data)
                typedData[i++] = (IJSonObject)d;

            // HINT: PH: 2012-06-12: using a local variable, which is declared
            // as an array, not IList<T> is required to avoid
            // NotSupportedException, when setting value inside a loop on Windows Mobile 5.0:
            _data = typedData;
        }

        /// <summary>
        /// Default constructor for inheriting classes.
        /// </summary>
        protected JSonArray(Boolean mutable)
        {
            if (!mutable)
                throw new ArgumentOutOfRangeException("mutable", "Invalid value, use a different constructor if immutable");

            _data = new List<IJSonObject>();
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        protected JSonArray(ICollection<Object> data, Boolean mutable)
        {
            if (!mutable)
                throw new ArgumentOutOfRangeException("mutable", "Invalid value, use a different constructor if immutable");

            _data = new List<IJSonObject>(data.Count);
            // convert data:
            foreach (Object d in data)
                _data.Add((IJSonObject) d);
        }

        /// <summary>
        /// Private constructor for cloning only.
        /// </summary>
        private JSonArray(ICollection<IJSonObject> data)
        {
            IJSonObject[] typedCopy = new IJSonObject[data.Count];

            // convert and copy data as an array:
            int i = 0;
            foreach (IJSonObject d in data)
                typedCopy[i++] = d;

            // HINT: PH: 2012-06-12: using a local variable, which is declared
            // as an array, not IList<T> is required to avoid
            // NotSupportedException, when setting value inside a loop on Windows Mobile 5.0:
            _data = typedCopy;
        }

        /// <summary>
        /// Gets or sets the value of internal data.
        /// </summary>
        protected internal IList<IJSonObject> Data
        {
            get { return _data; }
        }

        #region IJSonObject Members

        String IJSonObject.StringValue
        {
            get { throw new InvalidOperationException(); }
        }

        int IJSonObject.Int32Value
        {
            get { throw new InvalidOperationException(); }
        }

        uint IJSonObject.UInt32Value
        {
            get { throw new InvalidOperationException(); }
        }

        long IJSonObject.Int64Value
        {
            get { throw new InvalidOperationException(); }
        }

        ulong IJSonObject.UInt64Value
        {
            get { throw new InvalidOperationException(); }
        }

        float IJSonObject.SingleValue
        {
            get { throw new InvalidOperationException(); }
        }

        double IJSonObject.DoubleValue
        {
            get { throw new InvalidOperationException(); }
        }

        public decimal DecimalValue
        {
            get { throw new InvalidOperationException(); }
        }

        DateTime IJSonObject.DateTimeValue
        {
            get { throw new InvalidOperationException(); }
        }

        TimeSpan IJSonObject.TimeSpanValue
        {
            get { throw new InvalidOperationException(); }
        }

        bool IJSonObject.BooleanValue
        {
            get { throw new InvalidOperationException(); }
        }

        Guid IJSonObject.GuidValue
        {
            get { throw new InvalidOperationException(); }
        }

        bool IJSonObject.IsNull
        {
            get { return false; }
        }

        bool IJSonObject.IsTrue
        {
            get { return false; }
        }

        bool IJSonObject.IsFalse
        {
            get { return true; }
        }

        bool IJSonObject.IsEnumerable
        {
            get { return true; }
        }

        public bool IsArray
        {
            get { return true; }
        }

        object IJSonObject.ObjectValue
        {
            get
            {
                object[] result = new object[_data.Count];

                for (int i = 0; i < _data.Count; i++)
                    result[i] = _data[i].ObjectValue;

                return result;
            }
        }

        /// <summary>
        /// Gets the DateTime value for given JSON object.
        /// </summary>
        public DateTime ToDateTimeValue(JSonDateTimeKind kind)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the value of given JSON object.
        /// </summary>
        object IJSonObject.ToValue(Type t)
        {
            return JSonObjectConverter.ToObject(this, t);
        }

        /// <summary>
        /// Get the value of given JSON object.
        /// </summary>
        T IJSonObject.ToObjectValue<T>()
        {
            return (T)JSonObjectConverter.ToObject(this, typeof(T));
        }

        int IJSonObject.Length
        {
            get { return _data.Count; }
        }

        IJSonObject IJSonObject.this[int index]
        {
            get { return _data[index]; }
        }

        int IJSonObject.Count
        {
            get { return _data.Count; }
        }

        IJSonObject IJSonObject.this[string name]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, IJSonObject defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, String defaultValue, Boolean asJSonSerializedObject]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, String defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Int32 defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[string name, uint defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Int64 defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[string name, ulong defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Single defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Double defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        public IJSonObject this[string name, Decimal defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, DateTime defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[string name, DateTime defaultValue, JSonDateTimeKind kind]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, TimeSpan defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Guid defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Boolean defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        bool IJSonObject.Contains(string name)
        {
            return false;
        }

        ICollection<string> IJSonObject.Names
        {
            get { return null; }
        }

        IEnumerable<KeyValuePair<string, IJSonObject>> IJSonObject.ObjectItems
        {
            get { return null; }
        }

        bool IJSonObject.IsMutable
        {
            get { return false; }
        }

        IJSonMutableObject IJSonObject.AsMutable
        {
            get { return (IJSonMutableObject)this; }
        }

        IJSonMutableObject IJSonObject.CreateMutableClone()
        {
            return JSonMutableObject.CreateArray(this);
        }

        IJSonObject IJSonObject.CreateImmutableClone()
        {
            return new JSonArray(_data);
        }

        public string ToString(string format)
        {
            bool indent = JSonObjectConverter.GetIndentAndVerifyToStringFormat(format);

            using (var writer = new JSonWriter(indent))
            {
                writer.CompactEnumerables = format == JSonObjectConverter.CompactEnumerables;

                writer.Write(_data);
                return writer.ToString();
            }
        }

        IEnumerable<IJSonObject> IJSonObject.ArrayItems
        {
            get { return _data; }
        }

        #endregion

        #region IJSonWritable Members

        void IJSonWritable.Write(IJSonWriter output)
        {
            output.Write(_data);
        }

        #endregion

        public override string ToString()
        {
            return ToString(null);
        }
    }
}
