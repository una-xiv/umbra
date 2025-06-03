namespace Umbra.Game.Script;

internal class Parser(TokenStream tokens)
{
    public static ScriptContext Parse(TokenStream tokens)
    {
        return new Parser(tokens).Parse();
    }

    private readonly ScriptContext _context = new();

    private ScriptContext Parse()
    {
        CollectDependencies();
        ParseAny();
        
        return _context;
    }

    private void ParseAny()
    {
        while (true) {
            Token? token = tokens.Peek();
            if (!token.HasValue) return;

            switch (token.Value.Type) {
                case TokenType.Text:
                    _context.Nodes.Add(new TextNode(tokens.Consume(TokenType.Text)));
                    continue;
                case TokenType.Number:
                    _context.Nodes.Add(new NumberNode(tokens.Consume(TokenType.Number)));
                    continue;
                case TokenType.OpenBracket:
                    ParseBracketedExpression();
                    continue;
                default:
                    throw new ParseException($"Unexpected token ({token.Value.Type}) encountered at script top-level.", token.Value.Text, token.Value.Start);
            }
        }
    }

    private void ParseBracketedExpression()
    {
        Token openBracket    = tokens.Consume(TokenType.OpenBracket); // Consume '['
        Node  expressionNode = ParseConcatenation();

        if (expressionNode == null) {
            throw new ParseException("Expected a valid expression inside brackets.", openBracket.Text, openBracket.Start);
        }

        tokens.Consume(TokenType.CloseBracket);
        _context.Nodes.Add(expressionNode);
    }

    private Node ParseConcatenation()
    {
        Node left = ParseTernary(); // Next higher precedence

        while (tokens.Peek()?.Type == TokenType.Plus) {
            Token opToken = tokens.Consume(TokenType.Plus);
            Node  right   = ParseTernary();
            if (right == null) throw new ParseException("Expected expression after '+'.", opToken.Text, opToken.Start + opToken.Text.Length);
            left = new ConcatNode(opToken, left, right);
        }

        return left;
    }

    private Node ParseTernary()
    {
        Node condition = ParseComparison();

        if (tokens.Peek()?.Type == TokenType.QuestionMark) {
            Token qToken     = tokens.Consume(TokenType.QuestionMark);
            Node  trueBranch = ParseConcatenation();

            if (trueBranch == null)
                throw new ParseException("Expected a valid expression after '?'.", qToken.Text, qToken.Start + qToken.Text.Length);

            Node? falseBranch = null;
            if (tokens.Peek()?.Type == TokenType.Colon) {
                Token cToken = tokens.Consume(TokenType.Colon);
                falseBranch = ParseTernary();

                if (falseBranch == null)
                    throw new ParseException("Expected a valid expression after ':'.", cToken.Text, cToken.Start + cToken.Text.Length);
            }

            return new TernaryNode(qToken, condition, trueBranch, falseBranch);
        }

        return condition;
    }

    private Node ParseComparison()
    {
        Node left = ParsePipe(); // Next higher precedence

        while (IsComparisonOperator(tokens.Peek()?.Type)) {
            Token opToken = tokens.ConsumeOneOf([TokenType.Equals, TokenType.LessThan, TokenType.GreaterThan]);
            Node  right   = ParsePipe();

            if (right == null)
                throw new ParseException($"Expected expression after comparison operator '{opToken.Text}'.", opToken.Text, opToken.Start + opToken.Text.Length);

            left = new ComparisonNode(opToken, left, right);
        }

        return left;
    }

    private static bool IsComparisonOperator(TokenType? type)
    {
        return type is TokenType.GreaterThan or TokenType.LessThan or TokenType.Equals;
    }

    private Node ParsePipe()
    {
        Node left = ParsePrimary(); // Next higher precedence

        while (tokens.Peek()?.Type == TokenType.Pipe) {
            Token        opToken           = tokens.Consume(TokenType.Pipe);
            Token        functionNameToken = tokens.Consume(TokenType.Identifier);
            FunctionNode functionNode      = new FunctionNode(functionNameToken);

            left = new PipeNode(opToken, left, functionNode);
        }

        return left;
    }

    private Node ParsePrimary()
    {
        Token? token = tokens.Peek();
        if (!token.HasValue) {
            throw new ParseException("Unexpected end of tokens while parsing primary expression.", "", 0);
        }

        switch (token.Value.Type) {
            case TokenType.Identifier:
                return new IdentifierNode(tokens.Consume(TokenType.Identifier));
            case TokenType.Number:
                return new NumberNode(tokens.Consume(TokenType.Number));
            case TokenType.Text: // For string literals within expressions, e.g., "Text Here"
                return new TextNode(tokens.Consume(TokenType.Text));
            case TokenType.OpenBracket: // Parenthesized expression: [expr]
                tokens.Consume(TokenType.OpenBracket);
                Node expr = ParseConcatenation(); // Restart parsing with the lowest precedence for the expression within parentheses
                tokens.Consume(TokenType.CloseBracket);
                return expr;
            default:
                throw new ParseException($"Unexpected token type '{token.Value.Type}' ('{token.Value.Text}') encountered while parsing primary expression.", token.Value.Text, token.Value.Start);
        }
    }

    private void CollectDependencies()
    {
        Token? token;
        bool   inExpression   = false;
        bool   nextIsFunction = false;

        while ((token = tokens.Consume()) != null) {
            if (!inExpression && token.Value.Type == TokenType.OpenBracket) {
                inExpression = true;
                continue;
            }

            if (inExpression && token.Value.Type == TokenType.CloseBracket) {
                inExpression = false;
                continue;
            }

            if (!nextIsFunction && token.Value.Type == TokenType.Pipe) {
                nextIsFunction = true;
                continue;
            }

            if (!nextIsFunction && inExpression && token.Value.Type == TokenType.Identifier) {
                _context.Dependencies.Add(token.Value.Text.ToLowerInvariant());
            }

            if (nextIsFunction) {
                if (inExpression && token.Value.Type == TokenType.Identifier) {
                    _context.Functions.Add(token.Value.Text.ToLowerInvariant());
                }

                nextIsFunction = false;
            }
        }

        tokens.Rewind();
    }
}
