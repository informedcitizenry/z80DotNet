﻿//-----------------------------------------------------------------------------
// Copyright (c) 2017-2019 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE for full license information.
// 
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetAsm
{
    public sealed class MacroHandler : AssemblerBase, IBlockHandler
    {
        #region Members

        Dictionary<string, Macro> _macros;

        List<SourceLine> _expandedSource;

        Stack<List<SourceLine>> _macroDefinitions;

        Func<string, bool> _instructionFcn;

        Stack<SourceLine> _definitions;

        Stack<SourceLine> _declarations;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DotNetAsm.MacroHandler"/> class.
        /// </summary>
        /// <param name="instructionFcn">The lookup function to validate whether the name is an instruction or directive.</param>
        public MacroHandler(Func<string, bool> instructionFcn)
        {
            Reserved.DefineType("Directives", ".macro", ".endmacro", ".dsegment", ".segment", ".endsegment");
            _macros = new Dictionary<string, Macro>(Assembler.Options.StringComparar);
            _expandedSource = new List<SourceLine>();
            _macroDefinitions = new Stack<List<SourceLine>>();
            _instructionFcn = instructionFcn;
            _definitions = new Stack<SourceLine>();
            _declarations = new Stack<SourceLine>();
        }

        public bool Processes(string token)
        {
            return Reserved.IsReserved(token) || _macros.ContainsKey(token);
        }

        public void Process(SourceLine line)
        {
            var instruction = line.Instruction.ToLower();
            if (!Processes(line.Instruction))
            {
                if (_macroDefinitions.Count > 0)
                {
                    // We are in a macro definition
                    if (instruction.Equals(_macros.Last().Key))
                        Assembler.Log.LogEntry(line, ErrorStrings.RecursiveMacro, line.Instruction);

                    _macroDefinitions.Peek().Add(line);
                }
                else
                {
                    // We are consuming source after an undefined .dsegment
                    _expandedSource.Add(line);

                }
                return;
            }

            if (instruction.Equals(".macro") || instruction.Equals(".segment"))
            {
                _macroDefinitions.Push(new List<SourceLine>());
                string name;
                _definitions.Push(line);
                if (instruction.Equals(".segment"))
                {
                    if (!string.IsNullOrEmpty(line.Label))
                    {
                        Assembler.Log.LogEntry(line, ErrorStrings.None);
                        return;
                    }
                    name = line.Operand;
                }
                else
                {
                    name = "." + line.Label;
                }
                if (!Macro.IsValidMacroName(name) || _instructionFcn(name))
                {
                    Assembler.Log.LogEntry(line, ErrorStrings.LabelNotValid, line.Label);
                    return;
                }
                if (_macros.ContainsKey(name))
                {
                    Assembler.Log.LogEntry(line, ErrorStrings.MacroRedefinition, line.Label);
                    return;
                }
                _macros.Add(name, null);
            }
            else if (instruction.Equals(".endmacro") || instruction.Equals(".endsegment"))
            {
                var def = instruction.Replace("end", string.Empty);
                var name = "." + _definitions.Peek().Label;
                if (!_definitions.Peek().Instruction.Equals(def, Assembler.Options.StringComparison))
                {
                    Assembler.Log.LogEntry(line, ErrorStrings.ClosureDoesNotCloseMacro, line.Instruction);
                    return;
                }
                if (def.Equals(".segment"))
                {
                    name = _definitions.Peek().Operand;
                    if (!name.Equals(line.Operand, Assembler.Options.StringComparison))
                    {
                        Assembler.Log.LogEntry(line, ErrorStrings.None);
                        return;
                    }
                    //name = "." + name;
                }
                _macros[name] = Macro.Create(_definitions.Pop(),
                                             line,
                                             _macroDefinitions.Pop(),
                                             Assembler.Options.StringComparison,
                                             ConstStrings.OPEN_SCOPE,
                                             ConstStrings.CLOSE_SCOPE);
                if (def.Equals(".segment"))
                {
                    while (_declarations.Any(l => l.Operand.Equals(name, Assembler.Options.StringComparison)))
                    {
                        var dsegment = _declarations.Pop();
                        var segname = dsegment.Operand;
                        var ix = _expandedSource.IndexOf(dsegment);
                        _expandedSource.RemoveAt(ix);
                        _expandedSource.InsertRange(ix, _macros[segname].Expand(dsegment));
                    }
                }
            }
            else if (instruction.Equals(".dsegment"))
            {
                if (string.IsNullOrEmpty(line.Operand))
                {
                    Assembler.Log.LogEntry(line, ErrorStrings.TooFewArguments, line.Instruction);
                    return;
                }
                if (_macros.ContainsKey(line.Operand))
                {
                    var seg = _macros[line.Operand];
                    _expandedSource = seg.Expand(line).ToList();
                }
                else
                {
                    _expandedSource.Add(line);
                    _declarations.Push(line);
                }
            }
            else
            {
                var macro = _macros[line.Instruction];
                if (macro == null)
                {
                    Assembler.Log.LogEntry(line, ErrorStrings.MissingClosureMacro, line.Instruction);
                    return;
                }
                if (IsProcessing())
                {
                    _macroDefinitions.Peek().Remove(line);
                    _macroDefinitions.Peek().AddRange(macro.Expand(line).ToList());
                }
                else
                {
                    _expandedSource = macro.Expand(line).ToList();
                }
            }
        }

        public void Reset()
        {
            _macroDefinitions.Clear();
            _expandedSource.Clear();
            _definitions.Clear();
        }

        public bool IsProcessing() => _definitions.Count > 0 || _macroDefinitions.Count > 0 || _declarations.Count > 0;

        public IEnumerable<SourceLine> GetProcessedLines() => _expandedSource;
    }
}