using System;
using System.Collections.Generic;
using System.IO;

namespace Archmage.Common {
    public class Parser {
        private Tokenizer _tokenizer;

        public Parser(Tokenizer tokenizer) {
            _tokenizer = tokenizer;
        }

        public Node ParseExpression() {
            var expression = ParseAddSubstract();
            if(_tokenizer.Token != Token.EOF) throw new SyntaxException($"Unexpected characters at the end of expression.");
            return expression;
        }

        private Node ParseMultiplyDivide() {
            var left = ParseUnary();
            while(true) {
                Func<float, float, float> operation = null;
                if(_tokenizer.Token == Token.Multiply) {
                    operation = (a, b) => a * b;
                } else if(_tokenizer.Token == Token.Divide) {
                    operation = (a, b) => a / b;
                }
                if(operation == null) return left;
                _tokenizer.NextToken();
                var right = ParseUnary();
                left = new NodeBinary(left, right, operation);
            }
        }

        private Node ParseAddSubstract() {
            var left = ParseMultiplyDivide();
            while(true) {
                Func<float, float, float> operation = null;
                if(_tokenizer.Token == Token.Add) {
                    operation = (a, b) => a + b;
                } else if(_tokenizer.Token == Token.Subtract) {
                    operation = (a, b) => a - b;
                }
                if(operation == null) return left;
                _tokenizer.NextToken();
                var right = ParseMultiplyDivide();
                left = new NodeBinary(left, right, operation);
            }
        }

        private Node ParseUnary() {
            if(_tokenizer.Token == Token.Add) {
                _tokenizer.NextToken();
                return ParseUnary();
            } else if(_tokenizer.Token == Token.Subtract) {
                _tokenizer.NextToken();
                var right = ParseUnary();
                return new NodeUnary(right, (a) => -a);
            } else return ParseLeaf();
        }

        private Node ParseLeaf() {
            if(_tokenizer.Token == Token.Number) {
                var node = new NodeNumber(_tokenizer.Number);
                _tokenizer.NextToken();
                return node;
            }
            if(_tokenizer.Token == Token.OpenParens) {
                _tokenizer.NextToken();
                var node = ParseAddSubstract();
                if(_tokenizer.Token != Token.CloseParens) throw new SyntaxException("Missing close parenthesis.");
                _tokenizer.NextToken();
                return node;
            }
            if(_tokenizer.Token == Token.Identifier) {
                var name = _tokenizer.Identifier;
                _tokenizer.NextToken();
                if(_tokenizer.Token != Token.OpenParens) {
                    return new NodeVariable(name);
                } else {
                    _tokenizer.NextToken();
                    var arguments = new List<Node>();
                    while(true) {
                        arguments.Add(ParseAddSubstract());
                        if(_tokenizer.Token == Token.Comma) {
                            _tokenizer.NextToken();
                            continue;
                        }
                        break;
                    }
                    if(_tokenizer.Token != Token.CloseParens) throw new SyntaxException("Missing close parenthesis.");
                    _tokenizer.NextToken();
                    return new NodeFunctionCall(name, arguments.ToArray());
                }

            }
            throw new SyntaxException($"Unexpected token: {_tokenizer.Token}");
        }

        public static Node Parse(string str) {
            return Parse(new Tokenizer(new StringReader(str)));
        }

        public static Node Parse(Tokenizer tokenizer) {
            return new Parser(tokenizer).ParseExpression();
        }
    }

    public class SyntaxException : Exception {
        public SyntaxException(string message) : base(message) { }
    }
}
