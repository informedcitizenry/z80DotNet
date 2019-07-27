//-----------------------------------------------------------------------------
// Copyright (c) 2017-2019 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE for full license information.
// 
//-----------------------------------------------------------------------------

using DotNetAsm;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace z80DotNet
{
    /// <summary>
    /// A class that represents information about an opcode, including its diasssembly format,
    /// size, and its index (or code).
    /// </summary>
    public class Opcode
    {
        /// <summary>
        /// The opcode size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The index of the opcode
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the CPU of this opcode.
        /// </summary>
        /// <value>The cpu.</value>
        public string CPU { get; set; }
    }

    /// <summary>
    /// Represents an operand format, including captured expressions
    /// </summary>
    public class OperandFormat
    {
        public OperandFormat()
        {
            FormatString = string.Empty;
            Evaluations = new List<long>();
            EvaluationSizes = new List<int>();
        }

        /// <summary>
        /// The format string of the operand
        /// </summary>
        public string FormatString;

        public List<long> Evaluations;

        public List<int> EvaluationSizes;

        public void AddExpression(string expression)
        {
            var eval = Assembler.Evaluator.Eval(expression);
            Evaluations.Add(eval);
            EvaluationSizes.Add(eval.Size());
        }
    }
}
