//-----------------------------------------------------------------------------
// Copyright (c) 2017 informedcitizenry <informedcitizenry@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using DotNetAsm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace z80DotNet
{
    class Program
    {
        static void SetBannerTexts(IAssemblyController controller)
        {
            StringBuilder sb = new StringBuilder(), vsb = new StringBuilder();

            sb.Append("z80DotNet, A Simple .Net Z80 Cross Assembler\n(C) Copyright 2017 informedcitizenry.");
            sb.Append(Environment.NewLine);
            sb.Append("z80DotNet comes with ABSOLUTELY NO WARRANTY; see LICENSE!");
            sb.Append(Environment.NewLine);

            vsb.Append("z80DotNet, A Simple .Net Z80 Cross Assembler\n(C) Copyright 2017 informedcitizenry.");
            vsb.AppendFormat("Version {0}.{1} Build {2}",
                                    Assembly.GetEntryAssembly().GetName().Version.Major,
                                    Assembly.GetEntryAssembly().GetName().Version.Minor,
                                    Assembly.GetEntryAssembly().GetName().Version.Build);
            vsb.Append(Environment.NewLine);
            vsb.Append("z80DotNet comes with ABSOLUTELY NO WARRANTY; see LICENSE!");
            vsb.Append(Environment.NewLine);

            controller.BannerText = sb.ToString();
            controller.VerboseBannerText = vsb.ToString();
        }

        static void TargetHeader(IAssemblyController controller, BinaryWriter writer)
        {
            string arch = controller.Options.Architecture.ToLower();
            ushort progstart = Convert.ToUInt16(controller.Output.ProgramStart);
            ushort progend = Convert.ToUInt16(controller.Output.ProgramCounter);
            ushort size = Convert.ToUInt16(controller.Output.GetCompilation().Count);
            string name = controller.Options.OutputFile;

            if (arch.Equals("zx"))
            {
                if (name.Length > 10)
                    name = name.Substring(0, 10);
                else
                    name = name.PadLeft(10);

                List<byte> buffer = new List<byte>
                {
                    // header
                    0x00,
                    // file type - code
                    0x03
                };
                // file name
                buffer.AddRange(Encoding.ASCII.GetBytes(name));
                // file size
                buffer.AddRange(BitConverter.GetBytes(size));
                // program start
                buffer.AddRange(BitConverter.GetBytes(progstart));
                // unused
                buffer.AddRange(BitConverter.GetBytes(0x8000));

                // calculate checksum
                byte checksum = 0x00;
                buffer.ForEach(b => { checksum ^= b; });

                // add checksum
                buffer.Add(checksum);

                // write the buffer
                writer.Write(buffer.ToArray());
            }
            else if (arch.Equals("amsdos") || arch.Equals("amstap"))
            {
                List<byte> buffer = new List<byte>();
                if (arch.Equals("amsdos"))
                {
                    if (name.Length > 8)
                        name = name.Substring(0, 8);
                    else
                        name = name.PadRight(8);

                    name = string.Format("{0}$$$", name);

                    // user number 0
                    buffer.Add(0);

                }
                else
                {
                    if (name.Length > 16)
                        name = name.Substring(0, 16);
                    else
                        name = name.PadRight(16, '\0');
                }

                // name
                buffer.AddRange(Encoding.ASCII.GetBytes(name));

                if (arch.Equals("amsdos"))
                {
                    // block
                    buffer.Add(0);

                    // last block
                    buffer.Add(0);
                }
                else
                {
                    buffer.Add(1);
                    buffer.Add(2);
                }

                // binary type
                buffer.Add(2);

                // size
                buffer.AddRange(BitConverter.GetBytes(size));

                // start address
                buffer.AddRange(BitConverter.GetBytes(progstart));

                // first block
                buffer.Add(0xff);

                // logical size
                buffer.AddRange(BitConverter.GetBytes(size));

                // logical start
                buffer.AddRange(BitConverter.GetBytes(progstart));

                // unallocated
                buffer.AddRange(new byte[36]);

                if (arch.Equals("amsdos"))
                {
                    // file size (24-bit number)
                    buffer.AddRange(BitConverter.GetBytes(size));
                    buffer.Add(0);

                    byte checksum = 0;
                    buffer.ForEach(b =>
                    {
                        checksum = (byte)(checksum + b);
                    });
                    buffer.Add(checksum);

                    // bytes 69 - 127 undefined
                    buffer.AddRange(new byte[60]);
                }
                writer.Write(buffer.ToArray());
            }
            else if (arch.Equals("msx"))
            {
                // ID byte
                writer.Write(0xfe);

                // start address
                writer.Write(BitConverter.GetBytes(progstart));

                // end address
                writer.Write(BitConverter.GetBytes(progend));

                // start address
                writer.Write(BitConverter.GetBytes(progstart));
            }
            else if (string.IsNullOrEmpty(arch) || arch.Equals("flat"))
            {
                // do nothing
            }
            else
            {
                string error = string.Format("Unknown architecture specified '{0}'", arch);
                throw new System.CommandLine.ArgumentSyntaxException(error);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                IAssemblyController controller = new AssemblyController(args);
                controller.AddAssembler(new z80Asm(controller));
                SetBannerTexts(controller);
                controller.HeaderOutputAction = TargetHeader;
                controller.Assemble();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
