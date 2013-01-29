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

using System.IO;
using System;
using System.Text;

namespace CodeTitans.Helpers
{
    /// <summary>
    /// Interface allowing to read characters.
    /// </summary>
#if DEBUG && CODETITANS_LIB_CORE
    public
#else
    internal
#endif
    interface IStringReader
    {
        /// <summary>
        /// Reads next character from the input source.
        /// Automaticaly updates current char, EOF and traced position.
        /// </summary>
        char ReadNext();

        /// <summary>
        /// Reads a set of characters until end-of-line in input source or till the end.
        /// </summary>
        string ReadLine();

        /// <summary>
        /// Gets the last-read character.
        /// </summary>
        char CurrentChar
        { get; }

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
        /// Gets the current line.
        /// </summary>
        int Line
        { get; }

        /// <summary>
        /// Gets the character offset within the line.
        /// </summary>
        int LineOffset
        { get; }
    }

    /// <summary>
    /// Class that wraps reading of a string.
    /// </summary>
    internal class StringReaderWrapper : IStringReader
    {
        private readonly string _text;
        private int _line;
        private int _lineOffset;
        private readonly int _length;
        private bool _detectedNextLine;
        private int _readerOffset;

        public StringReaderWrapper(string text)
        {
            _text = text ?? string.Empty;
            _length = _text.Length;
            _line = 0;
            _lineOffset = -1;
            _readerOffset = -1;
        }

        public char ReadNext()
        {
            if (!IsEof)
                _readerOffset++;

            MoveToNextLine();

            if (_readerOffset < _length)
            {
                char result = _text[_readerOffset];

                _detectedNextLine = result == '\n';
                _lineOffset++;

                return result;
            }

            return char.MinValue;
        }

        public string ReadLine()
        {
            if (IsEof)
                return null;

            MoveToNextLine();

            StringBuilder result = new StringBuilder();
            _readerOffset++;

            while (_readerOffset < _length)
            {
                _lineOffset++;

                char c = _text[_readerOffset];

                if (c == '\r' || c == '\n')
                {
                    char nextC = _readerOffset + 1 < _length ? _text[_readerOffset + 1] : char.MinValue;

                    if (c == '\r' && nextC == '\n')
                    {
                        _readerOffset++;
                        _lineOffset++;
                    }

                    _detectedNextLine = c == '\n' || nextC == '\n';
                    return result.ToString();
                }

                result.Append(c);
                _readerOffset++;
            }

            return result.Length > 0 ? result.ToString() : null;
        }

        public char CurrentChar
        {
            get { return _readerOffset >= 0 && _readerOffset < _length ? _text[_readerOffset] : char.MinValue; }
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(_text); }
        }

        public bool IsEof
        {
            get { return _readerOffset >= _length; }
        }

        public int Line
        {
            get { return _line; }
        }

        public int LineOffset
        {
            get { return _lineOffset; }
        }

        private void MoveToNextLine()
        {
            if (_detectedNextLine)
            {
                _detectedNextLine = false;
                _lineOffset = -1;
                _line++;
            }
        }
    }

    internal class TextReaderWrapper : IStringReader
    {
        private readonly TextReader _reader;
        private int _line;
        private int _lineOffset;
        private char _currentChar;
        private bool _eof;
        private readonly bool _isEmpty;
        private bool _detectedNextLine;

        public TextReaderWrapper(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            _reader = reader;
            _line = 0;
            _lineOffset = -1;
            _currentChar = char.MinValue;
            _eof = false;
            _isEmpty = _reader.Peek() == -1;
        }

        public char ReadNext()
        {
            int data = _reader.Read();

            MoveToNextLine();

            _eof = data == -1;

            if (!_eof)
            {
                _currentChar = (char) data;

                _detectedNextLine = _currentChar == '\n';
                _lineOffset++;
            }
            else
            {
                _currentChar = char.MinValue;
            }

            return _currentChar;
        }

        public string ReadLine()
        {
            MoveToNextLine();

            if (_eof)
                return null;

            StringBuilder result = new StringBuilder();
            while (true)
            {
                int data = _reader.Read();

                _eof = data == -1;
                if (_eof)
                {
                    _currentChar = char.MinValue;
                    break;
                }

                _currentChar = (char) data;
                _lineOffset++;

                if (_currentChar == '\r' || _currentChar == '\n')
                {
                    if (_currentChar == '\r' && _reader.Peek() == '\n')
                    {
                        _reader.Read();
                        _currentChar = '\n';
                        _lineOffset++;
                    }

                    _detectedNextLine = true;
                    return result.ToString();
                }

                result.Append(_currentChar);
            }

            return result.Length > 0 ? result.ToString() : null;
        }

        public char CurrentChar
        {
            get { return _currentChar; }
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public bool IsEof
        {
            get { return _eof; }
        }

        public int Line
        {
            get { return _line; }
        }

        public int LineOffset
        {
            get { return _lineOffset; }
        }

        private void MoveToNextLine()
        {
            if (_detectedNextLine)
            {
                _detectedNextLine = false;
                _lineOffset = -1;
                _line++;
            }
        }
    }
}
