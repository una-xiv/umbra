using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Umbra.Game.Script.Filters;

namespace Umbra.Game.Script;

public sealed class UmbraScript : IDisposable
{
    /// <summary>
    /// Parses the given script and returns the result.
    /// </summary>
    /// <param name="script">The script source.</param>
    /// <returns>The parsed result.</returns>
    public static UmbraScript Parse(string script)
    {
        TokenStream   ts  = Tokenizer.Tokenize(script);
        ScriptContext ctx = Parser.Parse(ts);

        return new(ctx);
    }

    /// <summary>
    /// Represents a list of placeholder identifiers that this script depends on.
    /// </summary>
    /// <remarks>
    /// This list should be used to track whether this script should be
    /// re-evaluated when any of the dependencies change.
    /// </remarks>
    public ImmutableHashSet<string> Dependencies { get; }

    /// <summary>
    /// Represents a list of functions that this script uses. The runtime uses
    /// this to verify that all functions are defined before executing the script.
    /// </summary>
    public ImmutableHashSet<string> Functions { get; }

    private ScriptContext Context       { get; }
    private string        CachedResult  { get; set; } = string.Empty;
    private bool          IsInvalidated { get; set; }

    private UmbraScript(ScriptContext ctx)
    {
        Context      = ctx;
        Dependencies = ctx.Dependencies.ToImmutableHashSet();
        Functions    = ctx.Functions.ToImmutableHashSet();

        PlaceholderRegistry.OnValueChanged += OnPlaceholderValueChanged;
    }

    public void Dispose()
    {
        PlaceholderRegistry.OnValueChanged -= OnPlaceholderValueChanged;
    }

    public string Value {
        get
        {
            if (IsInvalidated || string.IsNullOrEmpty(CachedResult)) {
                CachedResult = Evaluate();
            }

            return CachedResult;
        }
    }

    private void OnPlaceholderValueChanged(string name)
    {
        if (Dependencies.Contains(name)) {
            IsInvalidated = true;
        }
    }

    /// <summary>
    /// Evaluates the script using the provided functions and placeholders.
    /// </summary>
    /// <returns>The result of the evaluated script.</returns>
    private string Evaluate()
    {
        IsInvalidated = false;
        
        StringBuilder resultBuilder = new();

        Dictionary<string, string> placeholders = [];

        foreach (var name in Dependencies) {
            placeholders[name] = PlaceholderRegistry.Has(name)
                ? PlaceholderRegistry.Get(name)
                : $"[{name}]"; // Fallback to placeholder name if not found.
        }

        foreach (Node node in Context.Nodes) {
            resultBuilder.Append(EvaluateNode(node, placeholders));
        }

        return resultBuilder.ToString();
    }

    public override string ToString()
    {
        return Context.ToString();
    }

    private string EvaluateNode(
        Node?                      node,
        Dictionary<string, string> placeholders)
    {
        if (node == null) return "";

        switch (node) {
            case TextNode tn:
                return tn.Text;

            case NumberNode nn:
                return nn.Value; // Numbers are treated as strings initially

            case IdentifierNode idn:
                string placeholderKey = idn.Name.ToLowerInvariant();
                if (placeholders.TryGetValue(placeholderKey, out string? value)) {
                    return value;
                }

                throw new EvaluationException($"Placeholder '{idn.Name}' not found.");

            case ConcatNode cn:
                return EvaluateNode(cn.Left, placeholders) +
                       EvaluateNode(cn.Right, placeholders);

            case PipeNode pn:
                string inputValue = EvaluateNode(pn.Left, placeholders);
                if (pn.Right is { } fn) {
                    return FilterFunctionRegistry.Invoke(fn.Name.ToLowerInvariant(), inputValue);
                }

                throw new EvaluationException("Invalid right operand for PipeNode; expected FunctionNode.");

            case TernaryNode tern:
                bool conditionResult = EvaluateCondition(tern.Condition, placeholders);
                return conditionResult
                    ? EvaluateNode(tern.TrueBranch, placeholders)
                    : EvaluateNode(tern.FalseBranch, placeholders);

            case ComparisonNode comp:
                // Directly evaluating a comparison node yields "true" or "false"
                bool comparisonBool = PerformComparison(comp, placeholders);
                Logger.Info($"Result of comparison '{comp.Operator}': {comparisonBool}");
                return comparisonBool.ToString().ToLowerInvariant();

            default:
                throw new EvaluationException($"Unsupported AST node type: {node.GetType().Name}");
        }
    }

    private bool EvaluateCondition(
        Node                       conditionNode,
        Dictionary<string, string> placeholders
    )
    {
        string conditionValueStr = EvaluateNode(conditionNode, placeholders);

        // Define "truthiness"
        // 1. Explicit boolean strings
        if (bool.TryParse(conditionValueStr, out bool boolVal)) {
            return boolVal;
        }

        // 2. Numeric interpretation (non-zero is true)
        if (double.TryParse(conditionValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double numVal)) {
            return numVal != 0;
        }

        return conditionValueStr switch {
            "true" or "1" or "yes" => true,
            "false" or "0" or "no" => false,
            _                      => string.IsNullOrWhiteSpace(conditionValueStr)
        };
    }

    private bool PerformComparison(
        ComparisonNode             compNode,
        Dictionary<string, string> placeholders
    )
    {
        string leftStr  = EvaluateNode(compNode.Left, placeholders);
        string rightStr = EvaluateNode(compNode.Right, placeholders);

        // Attempt numeric comparison first
        bool isNumericLeft  = double.TryParse(leftStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double leftNum);
        bool isNumericRight = double.TryParse(rightStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double rightNum);

        if (isNumericLeft && isNumericRight) {
            return compNode.Type switch {
                TokenType.GreaterThan => leftNum > rightNum,
                TokenType.LessThan    => leftNum < rightNum,
                TokenType.Equals      => Math.Abs(leftNum - rightNum) < 0.01f,
                _                     => throw new EvaluationException($"Unsupported numeric comparison operator: {compNode.Operator}")
            };
        }

        return compNode.Type switch {
            TokenType.GreaterThan => string.Compare(leftStr, rightStr, StringComparison.OrdinalIgnoreCase) > 0,
            TokenType.LessThan    => string.Compare(leftStr, rightStr, StringComparison.OrdinalIgnoreCase) < 0,
            TokenType.Equals      => string.Equals(leftStr, rightStr, StringComparison.OrdinalIgnoreCase),
            _                     => throw new EvaluationException($"Unsupported string comparison operator: {compNode.Operator}")
        };
    }
}
