//-----------------------------------------------------------------------------
// Copyright (c) 2017-2019 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE for full license information.
// 
//-----------------------------------------------------------------------------

using DotNetAsm;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace z80DotNet
{
    /// <summary>
    /// A class that assembles Z80 assembly code.
    /// </summary>
    public partial class z80Asm : AssemblerBase, ILineAssembler
    {
        #region Members

        IAssemblyController Controller;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance of a <see cref="T:z80DotNet.z80Asm"/> line assembler.
        /// </summary>
        /// <param name="controller">The <see cref="T:DotNetAsm.IAssemblyController"/> associated to
        /// this line assembler</param>
        public z80Asm(IAssemblyController controller) 
        {
            Controller = controller;

            _opcodes = _opcodes.OrderBy(o => o.Index).ToArray();

            _builders = new FormatBuilder[]
            {
                // a[,a]
                new FormatBuilder(@"^([a-ehl])(\s*,\s*([a-ehl]))?$()","{0}{1}",string.Empty,string.Empty,1,2,4,4, Assembler.Options.RegexOption),
                // a,i / i,a
                new FormatBuilder(@"^([air])\s*,\s*([air])$()","{0},{1}",string.Empty,string.Empty,1,2,3,3,Assembler.Options.RegexOption),
                // (c),a
                new FormatBuilder(@"^\(\s*(c)\s*\)(\s*,\s*[a-ehl])?$()","{0}",string.Empty,string.Empty,0,3,3,3,Assembler.Options.RegexOption),
                // a,(c)
                new FormatBuilder(@"^[a-ehl]\s*,\s*\(\s*(c)\s*\)$()","{0}",string.Empty,string.Empty,0,2,2,2,Assembler.Options.RegexOption),
                // [a,](hl)
                new FormatBuilder(@"^(([a-ehl])\s*,\s*)?\(\s*(bc|de|hl|ix|iy)\s*\)$()","{0}",string.Empty,string.Empty,0,2,4,4, Assembler.Options.RegexOption),
                // (hl),a
                new FormatBuilder(@"^\(\s*(bc|de|hl)\s*\)\s*,\s*([a-ehl])$()","({0}),{1}",string.Empty,string.Empty,1,2,3,3,Assembler.Options.RegexOption),
                // (sp)[,hl]
                new FormatBuilder(@"^\(\s*(ix|iy|sp)\s*\)(\s*,\s*(hl|ix|iy))?$()", "{0}", string.Empty, string.Empty, 0,4,4,4, Assembler.Options.RegexOption),
                // hl
                new FormatBuilder(@"^(bc|de|hl|ix|iy)$()","{0}",string.Empty,string.Empty,1,2,2,2,Assembler.Options.RegexOption),
                // af[,af']
                new FormatBuilder(@"^af(\s*,\s*af')?$()","{0}", string.Empty,string.Empty,0,2,2,2,Assembler.Options.RegexOption),
                // hl,bc
                new FormatBuilder(@"^(hl|ix|iy|sp)(\s*,\s*(bc|de|hl|sp|ix|iy))?$()","{0}",string.Empty,string.Empty,0,4,4,4, Assembler.Options.RegexOption),
                // de,hl
                new FormatBuilder(@"^(de|sp)\s*,\s*(hl|ix|iy)$()","{0}",string.Empty,string.Empty,0,3,3,3,Assembler.Options.RegexOption),
                // nz
                new FormatBuilder(@"^((c|m|p|z)|(nc|nz|pe|po))$()","{0}",string.Empty,string.Empty,0,4,4,4,Assembler.Options.RegexOption),
                // ixh,a
                new FormatBuilder(@"^i(x|y)(h|l)(\s*,\s*([a-e]|i(x|y)(h|l)))?$()","{0}",string.Empty,string.Empty,0,7,7,7, Assembler.Options.RegexOption),
                // a,ixh
                new FormatBuilder(@"^([a-e])\s*,\s*(i(x|y)(h|l))$()", "{0},{1}", string.Empty, string.Empty, 1,2,5,5, Assembler.Options.RegexOption),
                // (ix+$00)[,a]
                new FormatBuilder(@"^\(\s*i(x|y)\s*((\+|-).+)\)(\s*,\s*([a-ehl]))?$()","(i{0}+{2}){1}","${0:x2}",string.Empty,1,4,2,6, Assembler.Options.RegexOption),
                // a,(iy+$20)
                new FormatBuilder(@"^([a-ehl])\s*,\s*\(\s*i(x|y)\s*((\+|-).+)\)$()","{0},(i{1}+{2})","${0:x2}",string.Empty,1,2,3,5,Assembler.Options.RegexOption),
                // 0,(ix+$30)[,a]
                new FormatBuilder(@"^([^\s]+)\s*,\s*\(\s*i(x|y)\s*((\+|-).+)\)(\s*,\s*[a-ehl])?$()","{3},(i{0}+{2}){1}","${0:x2}","{0}",2,5,3,1,Assembler.Options.RegexOption, Assembler.Evaluator),
                // ($0000),a
                new FormatBuilder(@"^(\(.+\))\s*,\s*([a-ehl])$()", "{2},{0}", "${0:x4}", string.Empty,2,3,1,3,Assembler.Options.RegexOption, true),
                // nz,$0000
                new FormatBuilder(@"^((c|m|p|z)|(nc|nz|pe|po))\s*,\s*(.+)$()","{0},{2}", "${0:x4}",string.Empty,1,5,4,5,Assembler.Options.RegexOption),
                // a,($0000)
                new FormatBuilder(@"^([a-ehl])\s*,\s*(.+)$()", "{0},{2}", "${0:x4}", string.Empty,1,3,2,3,Assembler.Options.RegexOption, true),
                // a,$00 / a,($00)
                new FormatBuilder(@"^([a-ehl]|i(x|y)(h|l))\s*,\s*(.+)$()","{0},{2}","${0:x2}",string.Empty,1,5,4,5,Assembler.Options.RegexOption, true),
                // hl,($0000) / hl,$0000
                new FormatBuilder(@"^(bc|de|hl|ix|iy|sp)\s*,\s*(.+)$()", "{0},{2}", "${0:x4}", string.Empty, 1, 3, 2, 3, Assembler.Options.RegexOption, true),
                // ($0000),hl
                new FormatBuilder(@"^(.+)\s*,\s*(bc|de|hl|ix|iy|sp)$()", "{2},{0}", "${0:x4}", string.Empty, 2, 3, 1, 3, Assembler.Options.RegexOption, true),
                // (hl),$00
                new FormatBuilder(@"^\((bc|de|hl|ix|iy|sp)\)\s*,\s*(.+)$()", "({0}),{2}", "${0:x2}", string.Empty, 1, 3, 2, 3, Assembler.Options.RegexOption),
                // (ix+$00),$00
                new FormatBuilder(@"^\(\s*i(x|y)\s*((\+|-).+)\)\s*,\s*(.+)$()", "(i{0}+{2}),{3}", "${0:x2}", "${1:x2}", 1, 5, 2, 4, Assembler.Options.RegexOption),
                // 0,a / 0,(hl)
                new FormatBuilder(@"^([^\s]+)\s*,\s*(([a-ehl])|\(hl\))$()", "{3},{0}", string.Empty, "{0}", 2, 4, 4, 1, Assembler.Options.RegexOption, Assembler.Evaluator),
                // (c),0
                new FormatBuilder(@"^\(\s*(c)\s*\)\s*,\s*([^\s]+)$()", "({0}),{3}", string.Empty, "{0}", 1, 3, 3, 2, Assembler.Options.RegexOption, Assembler.Evaluator),
                // expression
                new FormatBuilder(@"^.+$()", "{2}", "${0:x4}", string.Empty, 1,1,0,1, Assembler.Options.RegexOption, true)
            };

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

            Controller.AddSymbol("a");
            Controller.AddSymbol("af");
            Controller.AddSymbol("af'");
            Controller.AddSymbol("b");
            Controller.AddSymbol("bc");
            Controller.AddSymbol("c");
            Controller.AddSymbol("d");
            Controller.AddSymbol("de");
            Controller.AddSymbol("e");
            Controller.AddSymbol("h");
            Controller.AddSymbol("hl");
            Controller.AddSymbol("i");
            Controller.AddSymbol("ix");
            Controller.AddSymbol("ixh");
            Controller.AddSymbol("ixl");
            Controller.AddSymbol("iy");
            Controller.AddSymbol("iyl");
            Controller.AddSymbol("l");
            Controller.AddSymbol("r");
            Controller.AddSymbol("sp");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses a <see cref="T:DotNetAsm.SourceLine"/>'s instruction and operand to return a
        /// DotNetAsm.OperandFormat and correspnding <see cref="T:DotNetAsm.Opcode"/>.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/></param>
        /// <returns>A <see cref="T:System.Tuple&lt;DotNetAsm.OperandFormat,DotNetAsm.Opcode&gt;"/>.</returns>
        Tuple<OperandFormat, Opcode> GetFormatAndOpcode(SourceLine line)
        {
            OperandFormat fmt = null;
            Opcode opc = null;

            string instruction = line.Instruction.ToLower();

            if (string.IsNullOrEmpty(line.Operand))
            {
                fmt = new OperandFormat
                {
                    FormatString = instruction
                };
                opc = _opcodes.FirstOrDefault(o => o.DisasmFormat.Equals(instruction));//Opcode.LookupOpcode(instruction, _opcodes);
            }
            else
            {
                string operand = line.Operand;
                if (Reserved.IsOneOf("ImpliedA", instruction))
                {
                    operand = Regex.Replace(operand, @"\s*,\s*a$", string.Empty);
                }
                if (instruction.Equals("rst") || Reserved.IsOneOf("Interrupt", instruction))
                {
                    fmt = new OperandFormat();
                    if (instruction.Equals("rst"))
                    {

                        fmt.FormatString = string.Format("rst ${0:x2}",
                            Assembler.Evaluator.Eval(operand));
                    }
                    else
                    {
                        fmt = new OperandFormat
                        {
                            FormatString = "im " + Assembler.Evaluator.Eval(operand).ToString()
                        };
                    }
                    opc = _opcodes.FirstOrDefault(o => o.DisasmFormat.Equals(fmt.FormatString, Assembler.Options.StringComparison));
                }
                else
                {
                    foreach (FormatBuilder builder in _builders)
                    {
                        fmt = builder.GetFormat(operand);
                        if (fmt == null)
                            continue;

                        string instrFmt = string.Format("{0} {1}", instruction, fmt.FormatString);
                        opc = _opcodes.FirstOrDefault(o => o.DisasmFormat.Equals(instrFmt, Assembler.Options.StringComparison));
                        if (opc == null)
                        {
                            instrFmt = instrFmt.Replace("${0:x4}", "${0:x2}");
                            opc = _opcodes.FirstOrDefault(o => o.DisasmFormat.Equals(instrFmt, Assembler.Options.StringComparison));
                        }
                        break;
                    }
                }
            }
            return new Tuple<OperandFormat, Opcode>(fmt, opc);
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
            OperandFormat fmt = null;
            Opcode opc = null;
            var result = GetFormatAndOpcode(line);
            fmt = result.Item1;
            opc = result.Item2;

            if (fmt == null)
            {
                Assembler.Log.LogEntry(line, ErrorStrings.BadExpression, line.Operand);
                return;
            }
            if (opc == null)
            {
                Assembler.Log.LogEntry(line, ErrorStrings.UnknownInstruction, line.Instruction);
                return;
            }
            fmt.FormatString = opc.DisasmFormat;
            long eval = long.MinValue, eval2 = long.MinValue;
            long evalAbs = long.MinValue;

            if (string.IsNullOrEmpty(fmt.Expression1) == false)
            {
                if (Regex.IsMatch(fmt.FormatString, @"\(i(x|y)\+\${0:x2}\)"))
                {
                    fmt.Expression1 = Regex.Replace(fmt.Expression1, @"(\+|-)\s+"
                        , m => m.Groups[1].Value).Trim();
                      
                    eval = Assembler.Evaluator.Eval(fmt.Expression1, sbyte.MinValue, sbyte.MaxValue);
                    if (eval < 0)
                    {
                        fmt.FormatString = fmt.FormatString.Replace("+", "-");
                        evalAbs = Math.Abs(eval);
                        eval &= 0xFF;
                    }
                    else
                    {
                        evalAbs = eval;
                    }
                }
                else if (fmt.FormatString.Contains("${0:x4}"))
                {
                    evalAbs = Assembler.Evaluator.Eval(fmt.Expression1, short.MinValue, ushort.MaxValue);
                    evalAbs &= 0xFFFF;
                    if (Reserved.IsOneOf("Relatives", line.Instruction))
                    {
                        int pcOffs = Assembler.Output.LogicalPC + opc.Size;
                        try
                        {
                            eval = Convert.ToSByte(Assembler.Output.GetRelativeOffset((int)evalAbs, pcOffs));
                        }
                        catch
                        {
                            throw new OverflowException(eval.ToString());
                        }
                    }
                    else
                    {
                        eval = evalAbs;
                    }
                }
                else
                {
                    eval = Assembler.Evaluator.Eval(fmt.Expression1, sbyte.MinValue, byte.MaxValue);
                    eval &= 0xFF;
                    evalAbs = eval;
                }
            }

            if (string.IsNullOrEmpty(fmt.Expression2) == false)
            {
                eval2 = (byte)Assembler.Evaluator.Eval(fmt.Expression2, sbyte.MinValue, byte.MaxValue);
                eval2 &= 0xFF;
            }

            if (eval2 != long.MinValue)
                line.Disassembly = string.Format(fmt.FormatString, evalAbs, eval2);
            else
                line.Disassembly = string.Format(fmt.FormatString, evalAbs);

            int exprsize = eval == long.MinValue ? 0 : eval.Size();

            exprsize += eval2 == long.MinValue ? 0 : eval2.Size();

            int opcodesize = 1;
            int opcode = opc.Index;
            var opclsb = opcode & 0xFF;

            if (opclsb == 0xCB || opclsb == 0xED || opclsb == 0xDD || opclsb == 0xFD)
            {
                opcodesize++;

                if ((opclsb == 0xDD || opclsb == 0xFD) && ((opcode >> 8) & 0xFF) == 0xCB)
                    opcodesize++;
            }
            if (opcodesize + exprsize > opc.Size)
            {
                Assembler.Log.LogEntry(line, ErrorStrings.UnknownInstruction, line.Instruction);
                return;
            }

            var opcbase = opcode & 0xFFFF;

            if (opcbase == 0xCBDD || opcbase == 0xCBFD)
                opcode = opcbase | ((int)eval << 16) | ((opcode & 0xFF0000) << 8);
            else if (opcbase == 0x36DD || opcbase == 0x36FD)
                opcode = opcbase | ((int)eval << 16) | ((int)eval2 << 24);
            else if (eval != long.MinValue)
                opcode |= ((int)eval << (opcodesize * 8));
            
            line.Assembly = Assembler.Output.Add(opcode, opc.Size);
        }

        public int GetInstructionSize(SourceLine line)
        {
            var opc = GetFormatAndOpcode(line);

            if (opc.Item2 != null)
                return opc.Item2.Size;
            return 0;
        }

        public bool AssemblesInstruction(string instruction) => Reserved.IsReserved(instruction);

        public override bool IsReserved(string token) => Reserved.IsReserved(token);

        #endregion
    }
}