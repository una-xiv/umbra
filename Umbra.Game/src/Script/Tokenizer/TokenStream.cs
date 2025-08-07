using System.Text;

namespace Umbra.Game.Script;

internal class TokenStream(Token[] tokens)
{
    private int _index = 0;

    public Token? Peek(int offset = 0)
    {
        var peekIndex = _index + offset;

        return peekIndex < tokens.Length ? tokens[peekIndex] : null;
    }

    public Token? Consume()
    {
        if (_index >= tokens.Length) return null;

        var token = tokens[_index];
        _index++;

        return token;
    }

    public Token Consume(TokenType type)
    {
        if (_index >= tokens.Length)
            throw new ParseException($"Unexpected end of token stream. Expected {type.ToString()}", "", 0);

        var token = tokens[_index];

        if (token.Type != type)
            throw new ParseException($"Unexpected token type. Expected {type.ToString()}, but got {token.Type.ToString()}", token.Text, token.Start);

        _index++;

        return token;
    }

    public Token ConsumeOneOf(List<TokenType> types)
    {
        if (_index >= tokens.Length)
            throw new ParseException($"Unexpected end of token stream. Expected one of {string.Join(", ", types)}", "", 0);

        var token = tokens[_index];

        if (!types.Contains(token.Type))
            throw new ParseException($"Unexpected token type. Expected one of {string.Join(", ", types)}, but got {token.Type.ToString()}", token.Text, token.Start);

        _index++;

        return token;
    }
    
    public void Rewind()
    {
        _index = 0;
    }

    public override string ToString()
    {
        StringBuilder sb = new("TokenStream:\n");
        
        for (int i = 0; i < tokens.Length; i++)
        {
            sb.AppendLine($"[{i}] {tokens[i]}");
        }
        
        return sb.ToString();
    }
}
