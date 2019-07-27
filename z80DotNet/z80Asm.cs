//-----------------------------------------------------------------------------
// Copyright (c) 2017-2019 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE for full license information.
// 
//-----------------------------------------------------------------------------

using DotNetAsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace z80DotNet
{
    /// <summary>
    /// A class that assembles Z80 assembly code.
    /// </summary>
    public partial class z80Asm : AssemblerBase, ILineAssembler
    {
        #region Constructors

        /// <summary>
        /// Constructs a new instance of a <see cref="T:z80DotNet.z80Asm"/> line assembler.
        /// </summary>
        /// <param name="controller">The <see cref="T:DotNetAsm.IAssemblyController"/> associated to
        /// this line assembler</param>
        public z80Asm(IAssemblyController controller) 
        {
            _registers = new HashSet<string>(Assembler.Options.StringComparar)
            {
                // 8-bit registers 
                "a", "b", "c", "d", "e", "h", "i", "ixl", "ixh", "iyl", "iyh", "l", "r",
                // word registers
                "af", "af'", "bc", "de", "hl", "ix", "iy", "sp",
                // indirects
                "(bc)", "(c)", "(de)", "(hl)", "(ix)", "(iy)", "(sp)"
            };

            _flags = new HashSet<string>(Assembler.Options.StringComparar)
            {
                "c", "m", "nc", "nz", "p", "pe", "po", "z"
            };

            ConstructInstructionTable();

            Reserved.DefineType("Mnemonics",
                    "adc", "add", "ccf", "cpd", "cpdr", "cpi", "cpir", "cpl", 
                    "daa", "dec", "di", "ei", "ex", "exx", "halt", "in", "inc",
                    "ind", "indr", "ini", "inir", "ld", "ldd", "lddr", "ldi",
                    "ldir","neg","nop", "otdr", "otir", "out", "outd", "outi",
                    "pop","push","reti","retn","rl", "rla", "rlc", "rlca", "rld",
                    "rr", "rra", "rrc", "rrca", "rrd", "rst", "sbc", "scf",
                    "sla", "sll", "slr", "sra", "srl", "xor"
                );

            Reserved.DefineType("Bits", 
                    "bit","res","set"
                );

            Reserved.DefineType("Shifts", 
                    "rl", "rla", "rlc", "rld", "rr", "rra", "rrc", "rrd",
                    "sla", "sll", "slr", "sra", "srl"
                );

            Reserved.DefineType("ImpliedA", 
                    "and", "cp", "or", "sub", "xor"
                );

            Reserved.DefineType("Interrupt", 
                    "im"
                );

            Reserved.DefineType("Branches", 
                    "call", "jp", "jr", "ret"
                );

            Reserved.DefineType("Relatives", 
                    "djnz", "jr"
                );

            controller.AddSymbol("a");
            controller.AddSymbol("af");
            controller.AddSymbol("af'");
            controller.AddSymbol("b");
            controller.AddSymbol("bc");
            controller.AddSymbol("c");
            controller.AddSymbol("d");
            controller.AddSymbol("de");
            controller.AddSymbol("e");
            controller.AddSymbol("h");
            controller.AddSymbol("hl");
            controller.AddSymbol("i");
            controller.AddSymbol("ix");
            controller.AddSymbol("ixh");
            controller.AddSymbol("ixl");
            controller.AddSymbol("iy");
            controller.AddSymbol("iyl");
            controller.AddSymbol("l");
            controller.AddSymbol("r");
            controller.AddSymbol("sp");

        }

        #endregion

        #region Methods

        (OperandFormat fmt, Instruction instruction) ParseToInstruction(SourceLine line)
        {
            var mnemonic = line.Instruction.ToLower();
            var operand = line.Operand;
            var fmt = new OperandFormat();
            var formatBuilder = new StringBuilder(mnemonic);
            
            if (!string.IsNullOrEmpty(operand))
            {
                formatBuilder.Append(' ');
                if (Regex.IsMatch(operand, @"af\s*,\s*af'"))
                {
                    formatBuilder.Append("af,af'");
                }
                else
                {
                    var csv = operand.CommaSeparate();
                    var csvCount = csv.Count;
                    for (int i = 0; i < csvCount; i++)
                    {
                        var element = csv[i];
                        if (csvCount == 2 && element.Equals("a", Assembler.Options.StringComparison) &&
                             csv[1].Equals("a", Assembler.Options.StringComparison) &&
                                 Reserved.IsOneOf("ImpliedA", line.Instruction))
                        {
                            formatBuilder.Append("a");
                            break;
                        }
                        if (i == 0 && mnemonic.Equals("rst"))
                        {
                            formatBuilder.Append($"${Assembler.Evaluator.Eval(element, sbyte.MinValue, byte.MaxValue):x2}");
                        }
                        else if (i == 0 && (Reserved.IsOneOf("Interrupt", mnemonic) || Reserved.IsOneOf("Bits", mnemonic)))
                        {
                            formatBuilder.Append(Assembler.Evaluator.Eval(element, 0, 7));
                        }
                        else if ((i == 0 && _flags.Contains(element)) ||
                                 _registers.Contains(element) ||
                                 (mnemonic.Equals("out") && 
                                  i == csvCount - 1 && 
                                  element.Equals("0")))
                        {
                            formatBuilder.Append(element.ToLower());
                        }
                        else if (element[0] == '(' && element[element.Length - 1] == ')')
                        {
                            int j = 1;
                            while (char.IsWhiteSpace(element[j])) j++;
                            if (j >= element.Length - 1)
                                return (null, null);
                            var substr = element.Substring(j, 2);
                            if (substr.Equals("ix", Assembler.Options.StringComparison) ||
                                substr.Equals("iy", Assembler.Options.StringComparison))
                            {
                                if (element.Length < 6)
                                    return (null, null);
                                // (ix+##)
                                formatBuilder.Append($"({substr.ToLower()}+${{0:x2}})");
                                j += 2;
                                fmt.AddElement(element.Substring(j, element.Length - j - 1));
                            }
                            else
                            {
                                
                                if (fmt.Evaluations.Count > 0)
                                {
                                    formatBuilder.Append("${1:x2}");
                                }
                                else
                                {
                                    if (element.GetNextParenEnclosure().Equals(element))
                                        formatBuilder.Append("(${0:x2})");
                                    else
                                        formatBuilder.Append("${0:x2}");
                                }
                                fmt.AddElement(element);
                            }
                        }
                        else
                        {
                            if (fmt.Evaluations.Count > 0)
                                formatBuilder.Append("${1:x2}");
                            else
                                formatBuilder.Append("${0:x2}");
                            fmt.AddElement(element);
                        }
                        if (i < csvCount - 1)
                            formatBuilder.Append(',');
                    }
                }  
            }
            string finalFormat = formatBuilder.ToString();
            int sz = 0;
            Instruction instruction;
            while (!_z80instructions.TryGetValue(finalFormat, out instruction))
            {
                if (fmt.Evaluations.Count == 0 || fmt.EvaluationSizes[0] > 2 || sz++ > 1)
                    return (null, null); // couldn't find it
                finalFormat = finalFormat.Replace("x2", "x4");
                fmt.EvaluationSizes[0] = 2;
            }
            fmt.FormatString = finalFormat;
            return (fmt, instruction);
        }

        public void AssembleLine(SourceLine line)
        {
            if (Assembler.Output.PCOverflow)
            {
                Assembler.Log.LogEntry(line,
                                        ErrorStrings.PCOverflow,
                                        Assembler.Output.LogicalPC.ToString());
                return;
            }
            (OperandFormat fmt, Instruction instruction) = ParseToInstruction(line);

            if (fmt == null)
            {
                Assembler.Log.LogEntry(line, ErrorStrings.AddressingModeNotSupported, line.Instruction + " " + line.Operand);
                return;
            }
            
            List<long> evals = new List<long>(fmt.Evaluations), evalDisplays = new List<long>(fmt.Evaluations);
            int opcodeSize = (instruction.Opcode == 0x00CB) ? 2 : ((long)instruction.Opcode).Size();
            if (Reserved.IsOneOf("Relatives", line.Instruction))
            {
                int pcOffs = Assembler.Output.LogicalPC + instruction.Size;
                try
                {
                    evals[0] = Convert.ToSByte(Assembler.Output.GetRelativeOffset((int)evals[0], pcOffs));
                    evalDisplays[0] &= 0xFFFF;
                    fmt.EvaluationSizes[0] = 1;
                }
                catch
                {
                    throw new OverflowException(evals[0].ToString());
                }
            }
            else if (Regex.IsMatch(fmt.FormatString, @"\(i(x|y)\+"))
            {
                if (fmt.EvaluationSizes.Count == 0 || evals[0] < sbyte.MinValue || evals[0] > sbyte.MaxValue)
                {
                    Assembler.Log.LogEntry(line, ErrorStrings.AddressingModeNotSupported, line.Instruction);
                    return;
                }
                if (evals[0] < 0)
                {
                    fmt.FormatString = fmt.FormatString.Replace("+", "-");
                    evalDisplays[0] = Math.Abs(evals[0]);
                }
                if (evalDisplays.Count > 1)
                    evalDisplays[1] &= 0xFF;
            }
            else
            {
                int instructionSize = opcodeSize;
                for (var i = 0; i < evals.Count; i++)
                {
                    var operandsize = fmt.EvaluationSizes[i];
                    if (evalDisplays[i] < 0)
                    {
                        if (fmt.EvaluationSizes[i] > 1)
                            evalDisplays[i] &= 0xFFFF;
                        else
                            evalDisplays[i] &= 0xFF;

                    }
                    instructionSize += operandsize;
                }
                if (instructionSize > instruction.Size)
                {
                    Assembler.Log.LogEntry(line, ErrorStrings.AddressingModeNotSupported, line.Instruction);
                    return;
                }
            }
            var assembly = new List<byte>();
            
            if ((instruction.Opcode & 0xFF) == 0xCB || (instruction.Opcode & 0xFF00) == 0xCB00)
            {
                assembly.AddRange(Assembler.Output.Add(instruction.Opcode, 2));
                if ((instruction.Opcode & 0xFF) != 0xCB)
                {
                    assembly.AddRange(Assembler.Output.Add(evals[0], 1));
                    assembly.AddRange(Assembler.Output.Add(instruction.Opcode >> 16, 1));
                }
            }
            else
            {
                assembly.AddRange(Assembler.Output.Add(instruction.Opcode, opcodeSize));
                for (var i = 0; i < evals.Count; i++)
                    assembly.AddRange(Assembler.Output.Add(evals[i], fmt.EvaluationSizes[i]));
            }
            line.Assembly = assembly;
            line.Disassembly = string.Format(fmt.FormatString, evalDisplays.Cast<object>().ToArray()); ;
        }

        public int GetInstructionSize(SourceLine line)
        {
            (OperandFormat fmt, Instruction instruction) = ParseToInstruction(line);

            if (instruction != null)
                return instruction.Size;
            return 0;
        }

        public bool AssemblesInstruction(string instruction) => Reserved.IsReserved(instruction);

        public override bool IsReserved(string token) => Reserved.IsReserved(token);

        #endregion
    }
}