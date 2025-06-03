using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Umbra.Common;

namespace Umbra.Game.Script;

public class UmbraScript
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

    private ScriptContext Context { get; }

    private UmbraScript(ScriptContext ctx)
    {
        Context      = ctx;
        Dependencies = ctx.Dependencies.ToImmutableHashSet();
        Functions    = ctx.Functions.ToImmutableHashSet();
    }

    /// <summary>
    /// Evaluates the script using the provided functions and placeholders.
    /// </summary>
    /// <remarks>
    /// The keys in the `functions` and `placeholders` dictionaries should be
    /// written in lowercase, as the script is case-insensitive for ease of use.
    /// </remarks>
    /// <param name="functions">A dictionary of filter functions.</param>
    /// <param name="placeholders">A dictionary of placeholders.</param>
    /// <returns></returns>
    public string Evaluate(
        Dictionary<string, Func<string, string>> functions,
        Dictionary<string, string>               placeholders
    )
    {
        // TODO: Implement script evaluation logic using the AST in the Context
        //       object. The caller is responsible for providing the correct
        //       functions and placeholders, as well as caching the result and
        //       re-evaluating the script when any of the dependencies change.
        
        return "";
    }

    public override string ToString()
    {
        return Context.ToString();
    }
}
