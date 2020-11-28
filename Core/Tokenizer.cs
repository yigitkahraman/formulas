using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archmage.Common {
    public enum Token {
        EOF,
        Add,
        Subtract,
        Multiply,
        Divide,
        OpenParens,
        CloseParens,
        Comma,
        Identifier,
        Number,
    }
    public class Tokenizer {
        private TextReader _reader;
        private char _currentChar;
        private Token _token;
        private float _number;
        private string _identifier;

        public Token Token { get { return _token; } }
        public float Number { get { return _number; } }
        public string Identifier { get { return _identifier; } }

        public Tokenizer(TextReader reader) {
            _reader = reader;
            NextChar();
            NextToken();
        }

        private void NextChar() {
            var ch = _reader.Read();
            _currentChar = ch < 0 ? '\0' : (char)ch;
        }

        public void NextToken() {
            while(char.IsWhiteSpace(_currentChar)) NextChar();
            switch(_currentChar) {
                case '\0':
                    _token = Token.EOF;
                    return;
                case '+':
                    NextChar();
                    _token = Token.Add;
                    return;
                case '-':
                    NextChar();
                    _token = Token.Subtract;
                    return;
                case '*':
                    NextChar();
                    _token = Token.Multiply;
                    return;
                case '/':
                    NextChar();
                    _token = Token.Divide;
                    return;
                case '(':
                    NextChar();
                    _token = Token.OpenParens;
                    return;
                case ')':
                    NextChar();
                    _token = Token.CloseParens;
                    return;
                case ',':
                    NextChar();
                    _token = Token.Comma;
                    return;
            }
            if(char.IsDigit(_currentChar) || _currentChar == '.') {
                var builder = new StringBuilder();
                var hasDecimal = false;
                while(char.IsDigit(_currentChar) || (!hasDecimal && _currentChar == '.')) {
                    builder.Append(_currentChar);
                    hasDecimal = _currentChar == '.';
                    NextChar();
                }
                _number = float.Parse(builder.ToString(), CultureInfo.InvariantCulture);
                _token = Token.Number;
                return;
            }
            if(char.IsLetter(_currentChar) || _currentChar == '_') {
                var builder = new StringBuilder();
                while(char.IsLetterOrDigit(_currentChar) || _currentChar == '_') {
                    builder.Append(_currentChar);
                    NextChar();
                }
                _identifier = builder.ToString();
                _token = Token.Identifier;
                return;
            }
            throw new InvalidDataException($"Unexpected character: {_currentChar}");
        }
    }
}
