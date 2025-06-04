using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Logger = Umbra.Common.Logger;

namespace Umbra.Game.Script;

internal class Tokenizer
{
    public static TokenStream Tokenize(string input)
    {
        return string.IsNullOrEmpty(input)
            ? new TokenStream([])
            : new TokenStream(new Tokenizer(input).Parse());
    }

    private readonly string      _input;
    private readonly int         _length;
    private readonly List<Token> _tokens;

    private int _column;

    private Tokenizer(string input)
    {
        _input  = input;
        _length = input.Length;
        _tokens = [];
    }

    private Token[] Parse()
    {
        while (_column < _length) {
            if (IsEnd()) break;
            if (IsPlaceholder() && ParsePlaceholder()) continue;
            if (ParseString()) continue;

            throw new ParseException(
                "Unexpected character encountered while parsing script.",
                _input[_column].ToString(),
                _column
            );
        }

        return _tokens.ToArray();
    }

    private bool ParseString(bool forceQuotes = false)
    {
        if (IsEnd()) return false;
        
        StringBuilder sb = new();

        int  start    = _column;
        bool isQuoted = !IsEnd() && _input[_column] == '"' || _input[_column] == '\'';
        char delimiter = isQuoted ? _input[_column] : '\0';

        if (!isQuoted && forceQuotes) return false;
        if (isQuoted) _column++;

        while (_column < _length) {
            if (IsEnd()) break;
            
            if (!isQuoted && IsPlaceholder()) {
                break;
            }
            
            if (!isQuoted && (_input[_column] == '"' || _input[_column] == '\'')) {
                break;
            }
            
            if (isQuoted && _input[_column] == '\\' && _column + 1 < _length && _input[_column + 1] == delimiter) {
                sb.Append(delimiter);
                _column++; // Skip the closing quote
                break;
            }
            
            if (isQuoted && _input[_column] == delimiter) {
                _column++; // Skip the closing quote
                break;
            }
            
            sb.Append(_input[_column]);
            _column++;
        }

        if (sb.Length > 0) {
            _tokens.Add(new Token(TokenType.Text, sb.ToString(), start, _column));
            return true;
        }

        return false;
    }
    
    private bool ParseIdentifier()
    {
        StringBuilder sb = new();
        int start = _column;

        while (_column < _length && (char.IsLetterOrDigit(_input[_column]) || _input[_column] == '_' || _input[_column] == '.')) {
            sb.Append(_input[_column]);
            _column++;
        }

        if (sb.Length > 0) {
            _tokens.Add(new Token(TokenType.Identifier, sb.ToString(), start, _column));
            return true;
        }

        return false;
    }

    private bool ParseCharacter(TokenType type, char character)
    {
        if (IsEnd() || _input[_column] != character) {
            return false;
        }
        
        _tokens.Add(new Token(type, character.ToString(), _column, ++_column));
        
        return true;
    }

    private bool ParseNumber()
    {
        // Parse digits and optional decimal point.
        StringBuilder sb = new();
        
        int start = _column;
        
        while (_column < _length && (char.IsDigit(_input[_column]) || _input[_column] == '.')) {
            sb.Append(_input[_column]);
            _column++;
        }
        
        if (sb.Length > 0) {
            // Check if the number is valid (e.g., not just a decimal point).
            if (sb.ToString() == "." || sb.ToString() == "..") {
                Logger.Warning($"Invalid number format at position {start}: '{sb}'");
                return false;
            }
            
            // Check if there are two decimal points.
            if (sb.ToString().Count(c => c == '.') > 1) {
                Logger.Warning($"Invalid number format at position {start}: '{sb}'");
                return false;
            }
            
            _tokens.Add(new Token(TokenType.Number, sb.ToString(), start, _column));
            return true;
        }

        return false;
    }

    private bool ParsePlaceholder()
    {
        _column++; // Skip the opening bracket.
        SkipWhitespace();

        string? placeholder = PeekUntil(_column, c => c == ']')?.Trim();
        if (placeholder == null) return false;
        
        _tokens.Add(new(TokenType.OpenBracket, "[", _column - 1, _column));

        while (_column < _length) {
            SkipWhitespace();
            if (IsEnd()) break;
            if (ParseNumber()) continue;
            if (ParseIdentifier()) continue;
            if (ParseString(true)) continue;
            if (ParseCharacter(TokenType.CloseBracket, ']')) break;
            if (ParseCharacter(TokenType.Pipe, '|')) continue;
            if (ParseCharacter(TokenType.QuestionMark, '?')) continue;
            if (ParseCharacter(TokenType.Colon, ':')) continue;
            if (ParseCharacter(TokenType.Plus, '+')) continue;
            if (ParseCharacter(TokenType.Equals, '=')) continue;
            if (ParseCharacter(TokenType.LessThan, '<')) continue;
            if (ParseCharacter(TokenType.GreaterThan, '>')) continue;
            
            // If we see another opening bracket, we assume it's a nested placeholder.
            if (_input[_column] == '[') {
                if (ParsePlaceholder()) continue;
            }
            
            // Invalid character in placeholder.
            Logger.Warning($"Unmatched character '{_input[_column]}' in placeholder at position {_column}.");
            _column++;
        }
        
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsPlaceholder()
    {
        if (IsEnd() || _input[_column] != '[') {
            return false;
        }
        
        return !string.IsNullOrEmpty(PeekUntil(_column + 1, c => c == ']')?.Trim());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsEnd()
    {
        return _column >= _length;
    }

    private void SkipWhitespace()
    {
        while (_column < _length && char.IsWhiteSpace(_input[_column])) {
            _column++;
        }
    }

    private string? PeekUntil(int offset, Func<char, bool> until)
    {
        int start = offset;
        
        while (offset < _length) {
            if (until(_input[offset])) {
                return _input.Substring(start, offset - start);
            }
            
            offset++;
        }

        return null;
    }
}
