namespace Umbra.Game.Script;

internal struct Token(TokenType type, string text, int start = 0, int end = 0)
{
    public readonly TokenType Type  = type;
    public readonly string    Text  = text;
    public readonly int       Start = start;
    public readonly int       End   = end;

    public override string ToString()
    {
        return $"[Token: Type={Type}, Text=\"{Text}\", Start={Start}, End={End}]";
    }
}

internal enum TokenType
{
    Text,
    Number,
    Identifier,
    OpenBracket,
    CloseBracket,
    Pipe,
    QuestionMark,
    Colon,
    Plus,
    Equals,
    LessThan,
    GreaterThan,
}
