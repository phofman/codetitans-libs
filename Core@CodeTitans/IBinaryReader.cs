using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// Gets the offset within the stream.
        /// </summary>
        int Offset
        { get; }

        byte ReadByte();

        int ReadInt32();

        uint ReadUInt32();

        long ReadInt64();

        ulong ReadUInt64();

        double ReadDouble();

        /// <summary>
        /// Reads UTF-8 string until termination ('\0') character.
        /// </summary>
        string ReadStringAnsi();

        string ReadStringAnsi(int length);
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
            if (_offset + 1 <= _data.Length)
            {
                _offset++;
                return _data[_offset];
            }

            throw new EndOfStreamException();
        }

        public int ReadInt32()
        {
            if (_offset + 4 <= _data.Length)
            {
                int result = _data[_offset + 1] | ((_data[_offset + 2]) << 8) | ((_data[_offset + 3]) << 16) | ((_data[_offset + 4]) << 24);

                _offset += 4;
                return result;
            }

            throw new EndOfStreamException();
        }

        public uint ReadUInt32()
        {
            if (_offset + 4 <= _data.Length)
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
            if (_offset + 8 <= _data.Length)
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
            if (_offset + 8 <= _data.Length)
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
            return BitConverter.Int64BitsToDouble(ReadInt64());
        }

        public string ReadStringAnsi()
        {
            if (_offset + 1 <= _data.Length)
            {
                _offset++;

                int index = _offset;

                while (index < _data.Length && _data[index] != 0)
                {
                    index++;
                }

                // any text found?
                if (index == _offset)
                    return string.Empty;

                var result = Encoding.UTF8.GetString(_data, _offset, index - _offset);

                _offset = index;
                return result;
            }

            throw new EndOfStreamException();
        }

        public string ReadStringAnsi(int length)
        {
            if (_offset + 1 <= _data.Length)
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
}
