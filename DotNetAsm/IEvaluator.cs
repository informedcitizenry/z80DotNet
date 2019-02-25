//-----------------------------------------------------------------------------
// Copyright (c) 2017-2019 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE for full license information.
// 
//-----------------------------------------------------------------------------

using System;

namespace DotNetAsm
{
    /// <summary>
    /// Defines an interface for an expression evaluator that can evaluate mathematical 
    /// expressions from strings.
    /// </summary>
    public interface IEvaluator
    {
        /// <summary>
        /// Evaluates a text string as a mathematical expression.
        /// </summary>
        /// <param name="expression">The string representation of the mathematical expression.</param>
        /// <returns>The result of the expression evaluation.</returns>
        long Eval(string expression);

        /// <summary>
        /// Evaluates a text string as a mathematical expression.
        /// </summary>
        /// <param name="expression">The string representation of the mathematical expression.</param>
        /// <param name="minval">The minimum value of expression. If the evaluated value is
        /// lower, then an exception will occur.</param>
        /// <param name="maxval">The maximum value of the expression. If the evaluated value 
        /// is higher, then an exception will occur.</param>
        /// <returns>The result of the expression evaluation.</returns>
        long Eval(string expression, long minval, long maxval);

        /// <summary>
        /// Evaluates a text string as a conditional (boolean) evaluation.
        /// </summary>
        /// <param name="expression">The string representation of the conditional expression.</param>
        /// <returns><c>True</c> if the expression is true, otherwise <c>false</c>.</returns>
        bool EvalCondition(string expression);

        /// <summary>
        /// Defines a symbol lookup for the evaluator to translate symbols (such as 
        /// variables) in expressions.
        /// </summary>
        /// <param name="lookupFunc">The lookup function to define the symbol.</param>
        void DefineSymbolLookup(Func<string, string> lookupFunc);

        /// <summary>
        /// Defines a symbol lookup for the evaluator to translate symbols (such as 
        /// variables) in expressions.
        /// </summary>
        /// <param name="regex">A regex pattern for the symbol.</param>
        /// <param name="lookupfunc">The lookup function to define the symbol.</param>
        void DefineSymbolLookup(string regex, Func<string, string> lookupfunc);

        /// <summary>
        /// Determines if the specifed symbol is a constant to the evaluator and would be
        /// evaulated as such.
        /// </summary>
        /// <returns><c>true</c>, if the symbol is a constant, <c>false</c> otherwise.</returns>
        /// <param name="symbol">Symbol.</param>
        bool IsConstant(string symbol);
    }
}
