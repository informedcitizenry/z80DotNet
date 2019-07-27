//-----------------------------------------------------------------------------
// Copyright (c) 2017-2019 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE for full license information.
// 
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNetAsm
{
    /// <summary>
    /// A class that represents information about an instruction, including its 
    /// size, CPU and opcode.
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// The instruction size (including operands).
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The opcode of the instruction.
        /// </summary>
        public int Opcode { get; set; }

        /// <summary>
        /// Gets or sets the CPU of this instruction.
        /// </summary>
        /// <value>The cpu.</value>
        public string CPU { get; set; }
    }

    /// <summary>
    /// Represents an operand format, including captured expressions
    /// </summary>
    public class OperandFormat
    {
        #region Constructors

        /// <summary>
        /// Constructs a new OperandFormat instance
        /// </summary>
        public OperandFormat()
        {
            FormatString = string.Empty;
            Evaluations = new List<long>();
            EvaluationSizes = new List<int>();
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Evaluate and add an expression element to the list of evaluated
        /// elements.
        /// </summary>
        /// <param name="element">The element to evaluate and add.</param>
        public void AddElement(string element)
        {
            var eval = Assembler.Evaluator.Eval(element);
            Evaluations.Add(eval);
            EvaluationSizes.Add(eval.Size());
        }

        #endregion

        #region Properties

        /// <summary>
        /// The format string of the operand
        /// </summary>
        public string FormatString;

        /// <summary>
        /// The captured evaluations
        /// </summary>
        public List<long> Evaluations { get; set; }

        /// <summary>
        /// The captured evaluation sizes
        /// </summary>
        public List<int> EvaluationSizes { get; set; }

        #endregion
    }
}