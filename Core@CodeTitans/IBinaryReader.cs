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
using System.IO;
using System.Text;

namespace CodeTitans.Helpers
{
    /// <summary>
    /// Interface allowing to read data from binary streams.
    /// </summary>
#if DEBUG && CODETITANS_LIB_CORE
    public
#else
    internal
#endif
    interface IBinaryReader
    {
        /// <summary>
        /// Gets an indication if given source is empty.
        /// </summary>
        bool IsEmpty
        { get; }

        /// <summary>
        /// Gets an indication if end-of-file has been reached.
        /// </summary>
        bool IsEof
        { get; }

        /// <summary>
        /// Gets the offset within the source stream.
        /// </summary>
        int Offset
        { get; }

        /// <summary>
        /// Reads a single byte from the source stream.
        /// </summary>
        byte ReadByte();

        /// <summary>
        /// Reads specified number of bytes from the source stream.
        /// </summary>
        byte[] ReadBytes(int length);

        /// <summary>
        /// Reads Int32 value (little-endian) from the source stream.
        /// </summary>
        int ReadInt32();

        /// <summary>
        /// Reads UInt32 value (little-endian) from the source stream.
        /// </summary>
        uint ReadUInt32();

        /// <summary>
        /// Reads Int64 value (little-endian) from the source stream.
        /// </summary>
        long ReadInt64();

        /// <summary>
        /// Reads UInt64 value (little-endian) from the source stream.
        /// </summary>
        ulong ReadUInt64();

        /// <summary>
        /// Reads double value (little-endian) from the source stream.
        /// </summary>
        double ReadDouble();

        /// <summary>
        /// Reads UTF-8 string until termination ('\0') character or length limit.
        /// </summary>
        string ReadStringUTF8(int length);
    }

    /// <summary>
    /// Reads numbers and string out of a byte array.
    /// All data is read using little-endian.
    /// </summary>
    internal class ArrayReaderWrapper : IBinaryReader
    {
        private readonly byte[] _data;
        private int _offset;

        public ArrayReaderWrapper(byte[] data)
        {
            _data = data ?? new byte[0];
            _offset = -1;
        }

        public bool IsEmpty
        {
            get { return _data.Length == 0; }
        }

        public bool IsEof
        {
            get { return _offset >= _data.Length; }
        }

        public int Offset
        {
            get { return _offset; }
        }

        public byte ReadByte()
        {
            if (_offset + 1 < _data.Length)
            {
                _offset++;
                return _data[_offset];
            }

            throw new EndOfStreamException();
        }

        public byte[] ReadBytes(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");
            if (length == 0)
                return new byte[0];

            if (_offset + length < _data.Length)
            {
                byte[] result = new byte[length];
                Array.Copy(_data, _offset + 1, result, 0, length);
                _offset += length;
                return result;
            }

            throw new EndOfStreamException();
        }

        public int ReadInt32()
        {
            if (_offset + 4 < _data.Length)
            {
                int result = _data[_offset + 1] | ((_data[_offset + 2]) << 8) | ((_data[_offset + 3]) << 16) | ((_data[_offset + 4]) << 24);

                _offset += 4;
                return result;
            }

            throw new EndOfStreamException();
        }

        public uint ReadUInt32()
        {
            if (_offset + 4 < _data.Length)
            {
                // UInt32 - little-endian always
                uint result = _data[_offset + 1] | (uint) ((_data[_offset + 2]) << 8) | (uint) ((_data[_offset + 3]) << 16) | (uint) ((_data[_offset + 4]) << 24);

                _offset += 4;
                return result;
            }

            throw new EndOfStreamException();
        }

        public long ReadInt64()
        {
            if (_offset + 8 < _data.Length)
            {
                uint l1 = _data[_offset + 1] | (uint)((_data[_offset + 2]) << 8) | (uint)((_data[_offset + 3]) << 16) | (uint)((_data[_offset + 4]) << 24);
                uint l2 = _data[_offset + 5] | (uint)((_data[_offset + 6]) << 8) | (uint)((_data[_offset + 7]) << 16) | (uint)((_data[_offset + 8]) << 24);

                _offset += 8;
                return l1 | ((long)l2 << 32);
            }

            throw new EndOfStreamException();
        }

        public ulong ReadUInt64()
        {
            if (_offset + 8 < _data.Length)
            {
                uint l1 = _data[_offset + 1] | (uint)((_data[_offset + 2]) << 8) | (uint)((_data[_offset + 3]) << 16) | (uint)((_data[_offset + 4]) << 24);
                uint l2 = _data[_offset + 5] | (uint)((_data[_offset + 6]) << 8) | (uint)((_data[_offset + 7]) << 16) | (uint)((_data[_offset + 8]) << 24);

                _offset += 8;
                return l1 | ((ulong)l2 << 32);
            }

            throw new EndOfStreamException();
        }

        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }

        public string ReadStringUTF8(int length)
        {
            if (length == 0)
                return string.Empty;

            if (_offset + 1 < _data.Length)
            {
                _offset++;

                int resultLength = 0;
                int index = _offset;

                while (resultLength < length && index < _data.Length && _data[index] != 0)
                {
                    resultLength++;
                    index++;
                }

                // any text found?
                if (resultLength == 0)
                    return string.Empty;

                var result = Encoding.UTF8.GetString(_data, _offset, resultLength);

                _offset = index;
                return result;
            }

            throw new EndOfStreamException();
        }
    }

    internal class BinaryReaderWrapper : IBinaryReader
    {
        private const int BufferDefaultSize = 32;

        private readonly BinaryReader _reader;
        private int _bufferLength;
        private byte[] _buffer;
        private readonly int _length;
        private int _offset;
        private readonly bool _isEmpty;


        public BinaryReaderWrapper(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            _offset = -1;
            _reader = reader;
            _length = (int) _reader.BaseStream.Length;
            _isEmpty = _reader.PeekChar() == -1;
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public bool IsEof
        {
            get { return _offset >= _length; }
        }

        public int Offset
        {
            get { return _offset; }
        }

        public byte ReadByte()
        {
            var result = _reader.ReadByte();

            _offset++;
            return result;
        }

        public byte[] ReadBytes(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            var result = _reader.ReadBytes(length);
            _offset += length;
            return result;
        }

        public int ReadInt32()
        {
            var result = _reader.ReadInt32();

            _offset += 4;
            return result;
        }

        public uint ReadUInt32()
        {
            var result = _reader.ReadUInt32();

            _offset += 4;
            return result;
        }

        public long ReadInt64()
        {
            var result = _reader.ReadInt64();

            _offset += 8;
            return result;
        }

        public ulong ReadUInt64()
        {
            var result = _reader.ReadUInt64();

            _offset += 8;
            return result;
        }

        public double ReadDouble()
        {
            var result = _reader.ReadDouble();

            _offset += 8;
            return result;
        }

        public string ReadStringUTF8(int length)
        {
            if (length == 0)
                return string.Empty;

            if (_buffer == null)
                _buffer = new byte[BufferDefaultSize];
            _bufferLength = 0;

            do
            {
                byte item = _reader.ReadByte();
                _offset++;

                if (item == 0)
                    break;

                _buffer[_bufferLength++] = item;

                if (_bufferLength == _buffer.Length)
                    Array.Resize(ref _buffer, _buffer.Length + BufferDefaultSize);

                if (_bufferLength == length)
                    break;
            }
            while (true);

            return Encoding.UTF8.GetString(_buffer, 0, _bufferLength);
        }
    }
}
