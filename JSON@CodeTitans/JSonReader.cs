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
using System.Globalization;
using System.IO;
using System.Text;
using CodeTitans.Helpers;
using CodeTitans.JSon.Objects;
using CodeTitans.JSon.ReaderHelpers;

namespace CodeTitans.JSon
{
    /// <summary>
    /// Reader class capable of parsing texts and providing them as JSON objects (or arrays of objects).
    /// </summary>
    public sealed class JSonReader : IJSonReader
    {
        /// <summary>
        /// Value of JSON 'null' keyword string.
        /// </summary>
        public const string NullString = "null";
        /// <summary>
        /// Value of JSON 'true' keyword string.
        /// </summary>
        public const string TrueString = "true";
        /// <summary>
        /// Value of JSON 'false' keyword string.
        /// </summary>
        public const string FalseString = "false";

        private static readonly TokenDataChar[] AvailableTokens= new[]
                                {
                                    new TokenDataChar('{', JSonReaderTokenType.ObjectStart),
                                    new TokenDataChar('}', JSonReaderTokenType.ObjectEnd),
                                    new TokenDataChar('[', JSonReaderTokenType.ArrayStart),
                                    new TokenDataChar(']', JSonReaderTokenType.ArrayEnd),
                                    new TokenDataChar(',', JSonReaderTokenType.Comma),
                                    new TokenDataChar(':', JSonReaderTokenType.Colon),
                                    new TokenDataChar('"', JSonReaderTokenType.String),
                                    new TokenDataChar('-', JSonReaderTokenType.Number)
                                };

        // HINT: all definitions should be lowercase!
        private static readonly TokenDataString[] AvailableKeywords = new[]
                                {
                                    new TokenDataString(NullString, JSonReaderTokenType.Keyword, DBNull.Value, new JSonStringObject(null)),
                                    new TokenDataString(FalseString, JSonReaderTokenType.Keyword, false, new JSonBooleanObject(false)),
                                    new TokenDataString(TrueString, JSonReaderTokenType.Keyword, true, new JSonBooleanObject(true))
                                };

        private IStringReader _input;
        private IObjectFactory _factory;
        private Stack<JSonReaderTokenInfo> _tokens;
        private bool _getTokenFromStack;
        private readonly bool _allowPartialRead;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JSonReader()
        {
        }

        /// <summary>
        /// Init constructor.
        /// Sets an indication if it is allowed to read multiple JSON objects from the same input source, if they are placed one after another.
        /// </summary>
        public JSonReader(bool allowPartialRead)
        {
            _allowPartialRead = allowPartialRead;
        }

        /// <summary>
        /// Returns reader to the original state.
        /// </summary>
        private void Reset(IStringReader input, IObjectFactory objectFactory)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (objectFactory == null)
                throw new ArgumentNullException("objectFactory");

            _input = input;
            _tokens = new Stack<JSonReaderTokenInfo>();
            _getTokenFromStack = false;
            _factory = objectFactory;
        }

        /// <summary>
        /// Reads next token from the input source.
        /// </summary>
        private JSonReaderTokenInfo ReadNextToken()
        {
            return ReadNextToken(true);
        }

        /// <summary>
        /// Reads next token from the input source with a possibility to treat already known character as the one read.
        /// Option required mostly for JSON elements that don't have a closing tokens (i.e.: ']' for arrays) and used for number and keywords.
        /// </summary>
        private JSonReaderTokenInfo ReadNextToken(bool fromCurrentChar)
        {
            // token was already read in advanced and put on the stack:
            if (_getTokenFromStack)
            {
                _getTokenFromStack = false;

                // reached end of input stream:
                if (_input.IsEof)
                    return new JSonReaderTokenInfo(string.Empty, JSonReaderTokenType.EndOfText, _input.Line, _input.LineOffset);

                if (_tokens.Count == 0)
                    throw new JSonReaderException("Lack of tokens", (JSonReaderTokenInfo)null);

                return _tokens.Peek();
            }

            if (fromCurrentChar)
            {
                // clear the whitespace characters:
                StringHelper.ReadWhiteChars(_input);
            }

            // reached end of input stream:
            if (_input.IsEof)
                return new JSonReaderTokenInfo(string.Empty, JSonReaderTokenType.EndOfText, _input.Line, _input.LineOffset);

            char tokenChar = _input.CurrentChar;
            string tokenString = tokenChar.ToString();
            JSonReaderTokenType tokenType = JSonReaderTokenType.Unknown;

            // check if this is one of the already known tokens...
            foreach (var tokenDef in AvailableTokens)
                if (tokenDef.Token == tokenChar)
                {
                    tokenType = tokenDef.Type;
                    break;
                }

            // is this the beginning of a keyword?
            if (tokenType == JSonReaderTokenType.Unknown && char.IsLetter(tokenChar))
                tokenType = JSonReaderTokenType.Keyword;

            // is this the beginning of the number?
            if (tokenType == JSonReaderTokenType.Unknown && (char.IsDigit(tokenChar) || tokenChar == '-'))
                tokenType = JSonReaderTokenType.Number;

            // if this is still unknown element...
            if (tokenType == JSonReaderTokenType.Unknown)
                throw new JSonReaderException("Invalid token found", tokenString, _input.Line, _input.LineOffset);

            JSonReaderTokenInfo nextToken = new JSonReaderTokenInfo(tokenString, tokenType, _input.Line, _input.LineOffset);

            _tokens.Push(nextToken);

            // return it:
            return nextToken;
        }

        private JSonReaderTokenInfo PopTopToken()
        {
            if (_tokens.Count == 0)
                throw new JSonReaderException("Lack of tokens", (JSonReaderTokenInfo)null);

            return _tokens.Pop();
        }

        /// <summary>
        /// Converts an input string into a dictionary, array, string or number, depending on the JSON string structure.
        /// </summary>
        private object Read()
        {
            // if there is nothing to read:
            if (_input.IsEmpty)
                return null;

            object result = null;
            JSonReaderTokenInfo currentToken;

            // analize the top level elements,
            // this could be an array, object, keyword, number, string:
            while ((currentToken = ReadNextToken()).Type != JSonReaderTokenType.EndOfText)
            {
                switch(currentToken.Type)
                {
                    case JSonReaderTokenType.ArrayStart:
                        if (result != null)
                            throw new JSonReaderException("Invalid second top level token", currentToken);

                        result = ReadArray();
                        break;

                    case JSonReaderTokenType.ArrayEnd:
                        throw new JSonReaderException("Lack of array opening token", currentToken);

                    case JSonReaderTokenType.ObjectStart:
                        if (result != null)
                            throw new JSonReaderException("Invalid second top level token", currentToken);

                        result = ReadObject();
                        break;

                    case JSonReaderTokenType.ObjectEnd:
                        throw new JSonReaderException("Lack of object opening token", currentToken);

                    case JSonReaderTokenType.Keyword:
                        if (result != null)
                            throw new JSonReaderException("Invalid second top level token", currentToken);

                        result = ReadKeyword();
                        break;

                    case JSonReaderTokenType.String:
                        if (result != null)
                            throw new JSonReaderException("Invalid second top level token", currentToken);

                        result = ReadString();
                        break;

                    case JSonReaderTokenType.Number:
                        if (result != null)
                            throw new JSonReaderException("Invalid second top level token", currentToken);

                        result = ReadNumber();
                        break;

                    default:
                        throw new JSonReaderException("Only one object allowed on top level", currentToken);
                }

                if (_allowPartialRead)
                    return result;
            }

            if (_tokens.Count != 0)
                throw new JSonReaderException("Missing JSON tokens to close all item definitions", _tokens.Peek());

            return result;
        }

        /// <summary>
        /// Adds new element to an array.
        /// </summary>
        private static void AddValue(ICollection<object> result, object value, int commas, JSonReaderTokenInfo currentToken)
        {
            if (result.Count != commas)
                throw new JSonReaderException("Missing commas between array objects", currentToken);

            result.Add(value);
        }

        /// <summary>
        /// Read an array from input stream.
        /// </summary>
        private object ReadArray()
        {
            List<object> result = new List<object>();
            JSonReaderTokenInfo currentToken;
            int commas = 0;

            while ((currentToken = ReadNextToken()).Type != JSonReaderTokenType.EndOfText)
            {
                if (currentToken.Type == JSonReaderTokenType.ArrayEnd)
                {
                    PopTopToken();

                    // if number of commas is greater than number of added elements,
                    // then value was not passed between:
                    if (result.Count > 0 && commas != result.Count - 1)
                        throw new JSonReaderException("Too many commas at closing array token, expected none", currentToken);
                    break;
                }

                switch (currentToken.Type)
                {
                    case JSonReaderTokenType.ArrayStart:
                        // read embedded array:
                        AddValue(result, ReadArray(), commas, currentToken);
                        break;

                    case JSonReaderTokenType.ObjectStart:
                        // read embedded object:
                        AddValue(result, ReadObject(), commas, currentToken);
                        break;

                    case JSonReaderTokenType.Keyword:
                        // add embedded value of reserved keyword:
                        AddValue(result, ReadKeyword(), commas, currentToken);
                        break;

                    case JSonReaderTokenType.Number:
                        // add embedded numeric value:
                        AddValue(result, ReadNumber(), commas, currentToken);
                        break;

                    case JSonReaderTokenType.String:
                        // add embedded string value:
                        AddValue(result, ReadString(), commas, currentToken);
                        break;

                    case JSonReaderTokenType.Comma:
                        // go to next array element:
                        currentToken = PopTopToken();
                        commas++;

                        // if number of commas is greater than number of added elements,
                        // then value was not passed between:
                        if (commas > result.Count)
                            throw new JSonReaderException("Missing value for array element", currentToken);
                        break;

                    default:
                        throw new JSonReaderException("Invalid token", currentToken);
                }
            }

            JSonReaderTokenInfo topToken = PopTopToken();
            if (topToken.Type != JSonReaderTokenType.ArrayStart || currentToken.Type == JSonReaderTokenType.EndOfText)
                throw new JSonReaderException("Missing close array token", topToken);

            return _factory.CreateArray(result);
        }

        /// <summary>
        /// Adds value to the dictionary.
        /// </summary>
        private static void AddValue(IDictionary<string, object> result, ref string name, object value, ref bool colonSpot, ref bool commaSpot, JSonReaderTokenInfo currentToken)
        {
            if (!colonSpot)
                throw new JSonReaderException("Missing colon between name and value definition", currentToken);

            if (!commaSpot && result.Count > 0)
                throw new JSonReaderException("Missing comma before name and value definition", currentToken);

            if (name == null)
                throw new JSonReaderException("Missing value for object element", currentToken);

            if (result.ContainsKey(name))
                throw new JSonReaderException("Duplicated name in object", currentToken);

            result.Add(name, value);
            name = null;
            colonSpot = false;
            commaSpot = false;
        }

        /// <summary>
        /// Reads a dictonary from an input stream.
        /// </summary>
        private object ReadObject()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            JSonReaderTokenInfo currentToken;
            string name = null;
            bool colonSpot = false;
            bool commaSpot = false;
            int commas = 0;

            while ((currentToken = ReadNextToken()).Type != JSonReaderTokenType.EndOfText)
            {
                if (currentToken.Type == JSonReaderTokenType.ObjectEnd)
                {
                    PopTopToken();

                    // if number of commas is greater than number of added elements,
                    // then value was not passed between:
                    if (commas > 0 && commas >= result.Count)
                        throw new JSonReaderException("Too many commas at closing object token, expected none", currentToken);
                    break;
                }

                switch (currentToken.Type)
                {
                    case JSonReaderTokenType.ArrayStart:

                        // read embedded array:
                        AddValue(result, ref name, ReadArray(), ref colonSpot, ref commaSpot, currentToken);
                        break;

                    case JSonReaderTokenType.ObjectStart:

                        // read embedded object:
                        AddValue(result, ref name, ReadObject(), ref colonSpot, ref commaSpot, currentToken);
                        break;

                    case JSonReaderTokenType.Keyword:

                        if (name == null)
                            throw new JSonReaderException("Keyword can not be an object element's name", currentToken);

                        // add embedded value of reserved keyword:
                        AddValue(result, ref name, ReadKeyword(), ref colonSpot, ref commaSpot, currentToken);
                        break;

                    case JSonReaderTokenType.Number:

                        // add embedded numeric value:
                        AddValue(result, ref name, ReadNumber(), ref colonSpot, ref commaSpot, currentToken);
                        break;

                    case JSonReaderTokenType.String:

                        if (!commaSpot && result.Count > 0)
                            throw new JSonReaderException("Missing comma before name and value definition", currentToken);

                        if (name == null)
                        {
                            name = ReadString().ToString();

                            if (result.ContainsKey(name))
                                throw new JSonReaderException("Duplicated name in object", currentToken);

                            break;
                        }

                        // add embedded string:
                        AddValue(result, ref name, ReadString(), ref colonSpot, ref commaSpot, currentToken);
                        break;

                    case JSonReaderTokenType.Colon:
                        PopTopToken();

                        if (colonSpot)
                            throw new JSonReaderException("Duplicated colon found in object", currentToken);

                        if (name == null)
                            throw new JSonReaderException("Unexpected colon, when name not given", currentToken);

                        colonSpot = true;
                        break;

                    case JSonReaderTokenType.Comma:

                        // go to next array element:
                        currentToken = PopTopToken();

                        if (commaSpot)
                            throw new JSonReaderException("Two commans in a row", currentToken);

                        commas++;
                        commaSpot = true;

                        // if number of commas is greater than number of added elements,
                        // then value was not passed between:
                        if (commas > result.Count)
                            throw new JSonReaderException("Missing value for object element", currentToken);
                        break;

                    default:
                        throw new JSonReaderException("Invalid token", currentToken);
                }
            }

            JSonReaderTokenInfo topToken = PopTopToken();
            if (topToken.Type != JSonReaderTokenType.ObjectStart || currentToken.Type == JSonReaderTokenType.EndOfText)
                throw new JSonReaderException("Missing close object token", topToken);

            return _factory.CreateObject(result);
        }

        /// <summary>
        /// Reads a keyword from the input stream.
        /// </summary>
        private object ReadKeyword()
        {
            JSonReaderTokenInfo topToken = PopTopToken();

            // top token contains the first letter of current keyword:
            StringBuilder buffer = new StringBuilder(topToken.Text);
            int lastLine = _input.Line;
            int lastOffset = _input.LineOffset;

            StringHelper.ReadKeywordChars(_input, buffer);

            // since keyword has no closing token (as arrays or strings), it might
            // happen that we read too many chars... so put that new char as a token on the
            // stack and instruct reader that token is already there...
            ReadNextToken(char.IsWhiteSpace(_input.CurrentChar));
            _getTokenFromStack = true;

            string keyword = buffer.ToString().ToLower();

            foreach (var k in AvailableKeywords)
                if (k.Token == keyword)
                    return _factory.CreateKeyword(k);

            // token has not been found:
            throw new JSonReaderException("Unknown keyword", keyword, lastLine, lastOffset);
        }

        /// <summary>
        /// Reads the string from input stream.
        /// </summary>
        private object ReadString()
        {
            int lastLine;
            int lastOffset;
            StringBuilder buffer = new StringBuilder();
            StringBuilder number = new StringBuilder();
            StringHelperStatusCode result = StringHelper.ReadStringChars(_input, buffer, number, true, out lastLine, out lastOffset);

            // throw exceptions for selected errors:
            switch (result)
            {
                case StringHelperStatusCode.UnexpectedEoF:
                    throw new JSonReaderException("Unexpected end of text while reading a string", PopTopToken());
                case StringHelperStatusCode.UnexpectedNewLine:
                    throw new JSonReaderException("Unexpected new line character in the middle of a string", buffer.ToString(), _input.Line, _input.LineOffset);
                case StringHelperStatusCode.UnknownEscapedChar:
                    throw new JSonReaderException("Unknown escape combination", _input.CurrentChar.ToString(), lastLine, lastOffset);
                case StringHelperStatusCode.TooShortEscapedChar:
                    throw new JSonReaderException("Invalid escape definition of Unicode character", _input.CurrentChar.ToString(), lastLine, lastOffset);
                case StringHelperStatusCode.TooLongEscapedChar:
                    throw new JSonReaderException("Too long Unicode number definition", number.ToString(), lastLine, lastOffset);
            }

            // remove the beggining of the string token " from the top of the tokens stack
            PopTopToken();

            return _factory.CreateString(buffer.ToString());
        }

        /// <summary>
        /// Read number from an input stream.
        /// </summary>
        private object ReadNumber()
        {
            JSonReaderTokenInfo topToken = PopTopToken();

            // top token contains the first letter of current number:
            StringBuilder buffer = new StringBuilder(topToken.Text);

            StringHelper.ReadDecimalNumberChars(_input, buffer);

            // verify what kind of character is just after the number, if it's a letter then it is an error,
            // the only allowed are white-chars and JSON object separators (comma, ']', '}')!
            if (!_input.IsEof)
            {
                if (_input.CurrentChar != ',' && !char.IsWhiteSpace(_input.CurrentChar) && _input.CurrentChar != ']' && _input.CurrentChar != '}')
                {
                    buffer.Append(_input.CurrentChar);
                    throw new JSonReaderException("Invalid number", buffer.ToString(), _input.Line, _input.LineOffset - buffer.Length);
                }
            }

            // since number has no closing token (as arrays or strings), it might
            // happen that we read too many chars... so put that new char as a token on the
            // stack and instruct reader that token is already there...
            ReadNextToken(char.IsWhiteSpace(_input.CurrentChar));
            _getTokenFromStack = true;

            string number = buffer.ToString();
            object result = _factory.CreateNumber(number);

            if (result != null)
                return result;

            // number had some invalid format:
            throw new JSonReaderException(string.Concat("Invalid number format, value: \"", number, "\", expected format: ", _factory.Format),
                                          number, _input.Line, _input.LineOffset - number.Length);
        }

        #region IJSonReader Members

        /// <summary>
        /// Converts a JSON string from given input into a tree of .NET arrays, dictionaries, strings and decimals.
        /// </summary>
        public object Read(TextReader input)
        {
            return Read(input, JSonReaderNumberFormat.Default);
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of .NET arrays, dictionaries, strings and decimals.
        /// </summary>
        public object Read(string input)
        {
            return Read(input, JSonReaderNumberFormat.Default);
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of .NET arrays, dictionaries, strings and decimals.
        /// </summary>
        public object Read(TextReader input, JSonReaderNumberFormat format)
        {
            Reset(StringHelper.CreateReader(input), FclObjectFactory.Create(format));
            return Read();
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of .NET arrays, dictionaries, strings and decimals.
        /// </summary>
        public object Read(string input, JSonReaderNumberFormat format)
        {
            Reset(StringHelper.CreateReader(input), FclObjectFactory.Create(format));
            return Read();
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing IJSonReadable interface as those objects exposes
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonObject ReadAsJSonObject(TextReader input)
        {
            return ReadAsJSonObject(input, JSonReaderNumberFormat.Default);
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing IJSonReadable interface as those objects exposes
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonObject ReadAsJSonObject(string input)
        {
            return ReadAsJSonObject(input, JSonReaderNumberFormat.Default);
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonObject ReadAsJSonObject(TextReader input, JSonReaderNumberFormat format)
        {
            Reset(StringHelper.CreateReader(input), JSonObjectFactory.Create(format));
            return Read() as IJSonObject;
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonObject ReadAsJSonObject(string input, JSonReaderNumberFormat format)
        {
            Reset(StringHelper.CreateReader(input), JSonObjectFactory.Create(format));
            return Read() as IJSonObject;
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonMutableObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonMutableObject ReadAsJSonMutableObject(TextReader input)
        {
            return ReadAsJSonMutableObject(input, JSonReaderNumberFormat.Default);
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonMutableObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonMutableObject ReadAsJSonMutableObject(string input)
        {
            return ReadAsJSonMutableObject(input, JSonReaderNumberFormat.Default);
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonMutableObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonMutableObject ReadAsJSonMutableObject(TextReader input, JSonReaderNumberFormat format)
        {
            Reset(StringHelper.CreateReader(input), JSonMutableObjectFactory.Create(format));
            return Read() as IJSonMutableObject;
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonMutableObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonMutableObject ReadAsJSonMutableObject(string input, JSonReaderNumberFormat format)
        {
            Reset(StringHelper.CreateReader(input), JSonMutableObjectFactory.Create(format));
            return Read() as IJSonMutableObject;
        }

        #endregion
    }
}
