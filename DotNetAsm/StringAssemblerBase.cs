﻿//-----------------------------------------------------------------------------
// Copyright (c) 2017-2019 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE for full license information.
// 
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetAsm
{
    /// <summary>
    /// A base class to assemble string pseudo operations.
    /// </summary>
    public abstract class StringAssemblerBase : AssemblerBase
    {
        #region Members

        private readonly Regex _regEncName;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a <see cref="T:DotNetAsm.StringAssemblerBase"/> class.
        /// </summary>
        protected StringAssemblerBase()
        {
            Reserved.DefineType("Directives",
                    ".cstring", ".lsstring", ".nstring", ".pstring", ".string"
                );

            Reserved.DefineType("Encoding", ".encoding", ".map", ".unmap");

            _regEncName = new Regex("^" + Patterns.SymbolBasic + "$",
                Assembler.Options.RegexOption | RegexOptions.Compiled);

        }

        #endregion

        #region Methods

        private void UpdateEncoding(SourceLine line)
        {
            line.DoNotAssemble = true;
            var instruction = line.Instruction.ToLower();
            var encoding = Assembler.Options.CaseSensitive ? line.Operand : line.Operand.ToLower();
            if (instruction.Equals(".encoding"))
            {
                if (!_regEncName.IsMatch(line.Operand))
                {
                    Assembler.Log.LogEntry(line, ErrorStrings.EncodingNameNotValid, line.Operand);
                    return;
                }
                Assembler.Encoding.SelectEncoding(encoding);
            }
            else
            {
                var parms = line.Operand.CommaSeparate().ToList();
                if (parms.Count == 0)
                    throw new ArgumentException(line.Operand);
                try
                {
                    var firstparm = parms.First();
                    var lastparm = parms.Last();

                    if (instruction.Equals(".map"))
                    {
                        if (parms.Count < 2 || parms.Count > 3)
                            throw new ArgumentException(line.Operand);

                        var translation = 0;

                        if (lastparm.EnclosedInQuotes())
                        {
                            if (lastparm.First().Equals('"'))
                            {
                                var transString = EvalEncodingParam(lastparm);
                                var translationBytes = System.Text.Encoding.UTF8.GetBytes(transString);
                                if (translationBytes.Length < 4)
                                    Array.Resize(ref translationBytes, 4);
                                translation = BitConverter.ToInt32(translationBytes, 0);
                            }
                            else
                            {
                                translation = char.ConvertToUtf32(EvalEncodingParam(lastparm), 0);
                            }
                        }
                        else
                        {
                            translation = (int)Assembler.Evaluator.Eval(lastparm, int.MinValue, int.MaxValue);
                        }

                        if (parms.Count == 2)
                        {
                            var mapchar = EvalEncodingParam(firstparm);
                            Assembler.Encoding.Map(mapchar, translation);
                        }
                        else
                        {
                            var firstRange = EvalEncodingParam(firstparm);
                            var lastRange = EvalEncodingParam(parms[1]);
                            Assembler.Encoding.Map(string.Concat(firstRange, lastRange), translation);
                        }
                    }
                    else
                    {
                        if (parms.Count > 2)
                            throw new ArgumentException(line.Operand);

                        if (parms.Count == 1)
                        {
                            var unmap = EvalEncodingParam(firstparm);
                            Assembler.Encoding.Unmap(unmap);
                        }
                        else
                        {
                            var firstunmap = EvalEncodingParam(firstparm);
                            var lastunmap = EvalEncodingParam(parms[1]);
                            Assembler.Encoding.Unmap(string.Concat(firstunmap, lastunmap));
                        }
                    }
                }
                catch (ArgumentException)
                {
                    Assembler.Log.LogEntry(line, ErrorStrings.BadExpression, line.Operand);
                }
            }
        }

        // Evaluate parameter the string as either a char literal or expression
        private string EvalEncodingParam(string p)
        {
            var quoted = p.GetNextQuotedString();
            if (string.IsNullOrEmpty(quoted))
            {
                var result = (int)Assembler.Evaluator.Eval(p, 0, 0x10FFFF);
                try
                {
                    return char.ConvertFromUtf32(result);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ArgumentException(p);
                }
            }
            return quoted;
        }

        /// <summary>
        /// Get the size of a string expression.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> associated to the expression</param>
        /// <returns>The size in bytes of the string expression</returns>
        protected int GetExpressionSize(SourceLine line)
        {
            if (Reserved.IsOneOf("Encoding", line.Instruction))
                return 0;

            List<string> csvs = line.Operand.CommaSeparate();
            var size = 0;
            foreach (var s in csvs)
            {
                if (s.EnclosedInQuotes(out var quoted))
                {
                    size += Assembler.Encoding.GetByteCount(quoted);//Regex.Unescape(s.TrimOnce(s.First())));
                }
                else
                {
                    if (s == "?")
                    {
                        size++;
                    }
                    else
                    {
                        var atoi = GetFormattedString(s, Assembler.Options.StringComparison, Assembler.Evaluator);
                        if (string.IsNullOrEmpty(atoi))
                        {
                            var v = Assembler.Evaluator.Eval(s);
                            size += v.Size();
                        }
                        else
                        {
                            size += atoi.Length;
                        }
                    }
                }
            }
            if (line.Instruction.Equals(".cstring", Assembler.Options.StringComparison) ||
                line.Instruction.Equals(".pstring", Assembler.Options.StringComparison))
            {
                size++;
            }

            return size;
        }

        public override bool IsReserved(string token)
        {
            return Reserved.IsOneOf("Directives", token) ||
                   Reserved.IsOneOf("Encoding", token);
        }

        /// <summary>
        /// Assemble strings to the output.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> to assemble.</param>
        protected void AssembleStrings(SourceLine line)
        {
            if (Reserved.IsOneOf("Encoding", line.Instruction))
            {
                UpdateEncoding(line);
                return;
            }
            var format = line.Instruction.ToLower();

            if (format.Equals(".pstring"))
            {
                try
                {
                    // we need to get the instruction size for the whole length, including all args
                    line.Assembly = Assembler.Output.Add(Convert.ToByte(GetExpressionSize(line) - 1), 1);
                }
                catch (OverflowException)
                {
                    Assembler.Log.LogEntry(line, ErrorStrings.PStringSizeTooLarge);
                    return;
                }
            }
            else if (format.Equals(".lsstring"))
            {
                Assembler.Output.Transforms.Push(b => Convert.ToByte(b << 1));
            }
            List<string> args = line.Operand.CommaSeparate();
            if (line.Assembly.Count > 0)
                line.Assembly.Clear();
            foreach (var arg in args)
            {
                List<byte> encoded;
                var atoi = GetFormattedString(arg, Assembler.Options.StringComparison, Assembler.Evaluator);
                if (string.IsNullOrEmpty(atoi))
                {
                    if (!arg.EnclosedInQuotes(out var quoted))
                    {
                        if (arg == "?")
                        {
                            Assembler.Output.AddUninitialized(1);
                            continue;
                        }
                        var val = Assembler.Evaluator.Eval(arg);
                        encoded = Assembler.Output.Add(val, val.Size());
                    }
                    else
                    {
                        encoded = Assembler.Output.Add(quoted, Assembler.Encoding);
                    }
                }
                else
                {
                    encoded = Assembler.Output.Add(atoi, Assembler.Encoding);
                }
                if (format.Equals(".nstring"))
                {
                    var neg = encoded.FirstOrDefault(b => b > 0x7f);
                    if (neg > 0x7f)
                    {
                        Assembler.Log.LogEntry(line, ErrorStrings.IllegalQuantity, neg);
                        return;
                    }
                }
                line.Assembly.AddRange(encoded);
            }
            var lastbyte = Assembler.Output.GetCompilation().Last();

            if (format.Equals(".lsstring"))
            {
                line.Assembly[line.Assembly.Count - 1] = (byte)(lastbyte | 1);
                Assembler.Output.ChangeLast(lastbyte | 1, 1);
                Assembler.Output.Transforms.Pop(); // clean up again :)
            }
            else if (format.Equals(".nstring"))
            {
                line.Assembly[line.Assembly.Count - 1] = Convert.ToByte((lastbyte + 128));
                Assembler.Output.ChangeLast(Convert.ToByte(lastbyte + 128), 1);
            }

            if (format.Equals(".cstring"))
            {
                line.Assembly.Add(0);
                Assembler.Output.Add(0, 1);
            }
        }
        #region Static Methods

        /// <summary>
        /// Gets the formatted string.
        /// </summary>
        /// <returns>The formatted string.</returns>
        /// <param name="operand">The line's operand.</param>
        /// <param name="comparison">The <see cref="T:System.StringComparison"/> to evaluate the string operand</param>
        /// <param name="evaluator">The <see cref="T:DotNetAsm.IEvaluator"/> to evaluate non-string objects.</param>
        public static string GetFormattedString(string operand, StringComparison comparison, IEvaluator evaluator)
        {
            if (!operand.StartsWith("format(", comparison))
                return string.Empty;
            if (!operand.EndsWith(")", comparison))
                throw new Exception(ErrorStrings.None);
            var parms = operand.Substring(operand.IndexOf('('));

            List<string> csvs = parms.TrimStartOnce('(').TrimEndOnce(')').CommaSeparate();
            var fmt = csvs.First();
            if (fmt.Length < 5 || !fmt.EnclosedInQuotes(out var fmtQuoted))
                throw new Exception(ErrorStrings.None);
            var parmlist = new List<object>();

            for (var i = 1; i < csvs.Count; i++)
            {
                if (string.IsNullOrEmpty(csvs[i]))
                    throw new Exception(ErrorStrings.None);
                if (csvs[i].EnclosedInQuotes(out var quoted))
                    parmlist.Add(quoted);
                else
                    parmlist.Add(evaluator.Eval(csvs[i]));
            }
            return string.Format(fmtQuoted, parmlist.ToArray());
        }

        #endregion

        #endregion
    }
}