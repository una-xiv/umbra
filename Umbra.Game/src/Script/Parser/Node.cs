namespace Umbra.Game.Script;

internal class Node(Token token)
{
    public Token Token { get; } = token;
    
    public virtual string ToString(int indent = 0)
    {
        return $"[{GetType().Name}]: {Token.Text}";
    }
}

internal class TextNode(Token token) : Node(token)
{
    public string Text { get; } = token.Text;
}

internal class NumberNode(Token token) : Node(token)
{
    public string Value { get; } = token.Text;
    
    public override string ToString(int indent = 0)
    {
        return $"[{GetType().Name}]: {Value}";
    }
}

internal class IdentifierNode(Token token) : Node(token)
{
    public string Name { get; } = token.Text.ToLowerInvariant();
    
    public override string ToString(int indent = 0)
    {
        return $"[{GetType().Name}]: {Name}";
    }
}

internal class FunctionNode(Token token) : Node(token)
{
    public string Name { get; } = token.Text.ToLowerInvariant();

    public override string ToString(int indent = 0)
    {
        return $"[{GetType().Name}]: {Name}";
    }
}

internal class ComparisonNode(Token token, Node left, Node right) : Node(token)
{
    public Node      Left     { get; } = left;
    public Node      Right    { get; } = right;
    public char      Operator { get; } = token.Text[0];
    public TokenType Type     { get; } = token.Type;

    public override string ToString(int indent = 0)
    {
        return $"[{GetType().Name}]: {Operator}\n" +
               $"{new string(' ', indent)}Left: {Left.ToString(indent + 1)}\n" +
               $"{new string(' ', indent)}Right: {Right.ToString(indent + 1)}";
    }
}

internal class ConcatNode(Token token, Node left, Node right) : Node(token)
{
    public Node Left  { get; } = left;
    public Node Right { get; } = right;
    
    public override string ToString(int indent = 0)
    {
        return $"[{GetType().Name}]:\n" +
               $"{new string(' ', indent)}Left: {Left.ToString(indent + 1)}\n" +
               $"{new string(' ', indent)}Right: {Right.ToString(indent + 1)}";
    }
}

internal class PipeNode(Token token, Node left, FunctionNode right) : Node(token)
{
    public Node         Left  { get; } = left;
    public FunctionNode Right { get; } = right;
    
    public override string ToString(int indent = 0)
    {
        return $"[{GetType().Name}]:\n" +
               $"{new string(' ', indent)}Left: {Left.ToString(indent + 1)}\n" +
               $"{new string(' ', indent)}Right: {Right.ToString(indent + 1)}";
    }
}

internal class TernaryNode(Token token, Node condition, Node trueBranch, Node? falseBranch = null) : Node(token)
{
    public Node  Condition   { get; } = condition;
    public Node  TrueBranch  { get; } = trueBranch;
    public Node? FalseBranch { get; } = falseBranch;
    
    public override string ToString(int indent = 0)
    {
        return $"[{GetType().Name}]:\n" +
               $"{new string(' ', indent)}Condition: {Condition.ToString(indent + 1)}\n" +
               $"{new string(' ', indent)}TrueBranch: {TrueBranch.ToString(indent + 1)}\n" +
               $"{new string(' ', indent)}FalseBranch: {FalseBranch?.ToString(indent + 1) ?? "null"}";
    }
}
