using DotNetAsm;
using z80DotNet;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.z80Tests
{
    public class TestController : IAssemblyController
    {
        public TestController(string[] args)
        {
            Output = new Compilation(true);

            Options = new AsmCommandLineOptions();

            Log = new ErrorLog();

            Encoding = new AsmEncoding(false);

            Evaluator = new Evaluator(@"\$([a-fA-F0-9]+)");

            Evaluator.DefineSymbolLookup("'.'", s => Convert.ToInt32(s.Trim('\'').First()).ToString());

            // to help throw errors 
            Evaluator.DefineSymbolLookup(@"[a-ehilr]", s => string.Empty);

            Labels = new LabelCollection(Options.StringComparar);

            Variables = new VariableCollection(Options.StringComparar, Evaluator);

            if (args != null)
                Options.ProcessArgs(args);
        }

        public void AddAssembler(ILineAssembler lineAssembler)
        {
            throw new NotImplementedException();
        }

        public void AddSymbol(string symbol)
        {

        }

        public void Assemble()
        {
            throw new NotImplementedException();
        }

        public AsmCommandLineOptions Options { get; private set; }

        public Action<IAssemblyController, System.IO.BinaryWriter> HeaderOutputAction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Action<IAssemblyController, System.IO.BinaryWriter> FooterOutputAction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ILineDisassembler Disassembler
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsInstruction(string token)
        {
            throw new NotImplementedException();
        }

        public Compilation Output { get; private set; }

        public ErrorLog Log { get; private set; }

        public SymbolCollectionBase Labels { get; private set; }

        public VariableCollection Variables { get; private set; }

        public IEvaluator Evaluator { get; private set; }

        public AsmEncoding Encoding { get; private set; }

        public string BannerText
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string VerboseBannerText
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    [TestFixture]
    public class NUnitTestZ80
    {
        private TestController _controller;
        private z80Asm _asm;

        public NUnitTestZ80()
        {
            _controller = new TestController(null);
            _asm = new z80Asm(_controller);
        }

        private void TestInstruction(SourceLine line, int pc, byte[] expected, string disasm, bool positive = true)
        {
            _asm.AssembleLine(line);
            if (positive)
            {
                Assert.IsFalse(_controller.Log.HasErrors);
                Assert.AreEqual(pc, _controller.Output.LogicalPC);
                Assert.IsTrue(_controller.Output.GetCompilation().SequenceEqual(expected));
                Assert.AreEqual(disasm, line.Disassembly);
                Assert.AreEqual(expected.Count(), _asm.GetInstructionSize(line));
            }
            else
            {
                Assert.IsTrue(_controller.Log.HasErrors);
            }
            ResetController();
        }

        private void TestForFailure(SourceLine line)
        {
            TestInstruction(line, 0, null, string.Empty, false);
        }

        private void TestForFailure<Texc>(SourceLine line) where Texc : System.Exception
        {
            try { Assert.Throws<Texc>(() => TestForFailure(line)); }
            catch { }
            finally { ResetController(); }
        }

        private void ResetController()
        {
            _controller.Output.Reset();
            _controller.Log.ClearErrors();
        }

        [Test]
        public void TestGetInstructionSize()
        {
            SourceLine line = new SourceLine();

            line.Instruction = "ret";
            line.Operand = "c";
            int size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Instruction = "retn";
            line.Operand = string.Empty;
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Instruction = "push";
            line.Operand = "af";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Operand = "ix";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Instruction = "add";
            line.Operand = "hl,de";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Operand = "a,$30";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Instruction = "adc";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Operand = "hl,de";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Instruction = "and";
            line.Operand = "a,a";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Instruction = "cp";
            line.Operand = "$30";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Instruction = "ex";
            line.Operand = "af ,af'";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Operand = "de,hl";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Operand = "(sp),hl";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Operand = "(sp),iy";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Instruction = "jp";
            line.Operand = "$3330";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(3, size);

            line.Operand = "(iy)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Operand = "(hl)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Instruction = "jr";
            line.Operand = "c,$3000";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Instruction = "ld";
            line.Operand = "b,c";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Operand = "a,(hl)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Operand = "a,$(30+2)-3*(4-3)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Operand = "a,($30)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(3, size);

            line.Operand = "($5000),a";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(3, size);

            line.Operand = "hl,($5000)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(3, size);

            line.Operand = "bc,($5000)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(4, size);

            line.Operand = "ixh,ixh";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Operand = "ixh,$30";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(3, size);

            line.Operand = "ix,ix";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(0, size); // there is no ld ix,ix!!

            line.Instruction = "add";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size); // but there is an add ix,ix!

            line.Instruction = "ld";
            line.Operand = "(hl),$30";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Operand = "ix,$3000";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(4, size);

            line.Operand = "iy,($3000)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(4, size);

            line.Operand = "($3000),sp";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(4, size);

            line.Operand = "($3000),iy";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(4, size);

            line.Operand = "(ix+$30),$ff";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(4, size);

            line.Operand = "(ix+$30),a";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(3, size);

            line.Operand = "a,(ix - $30)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(3, size);

            line.Operand = "i,a";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Operand = "a,r";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Instruction = "srl";
            line.Operand = "b";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Operand = "(ix+$30),a";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(4, size);

            line.Instruction = "bit";
            line.Operand = "0,a";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Operand = "0,(hl)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Operand = "0,(ix+$30),a";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(4, size);

            line.Instruction = "im";
            line.Operand = "0";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Instruction = "out";
            line.Operand = "($00),a";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Instruction = "inc";
            line.Operand = "bc";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(1, size);

            line.Operand = "ixh";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(2, size);

            line.Operand = "(iy+$30)";
            size = _asm.GetInstructionSize(line);
            Assert.AreEqual(3, size);
        }

        [Test]
        public void TestZ80SourceLine()
        {
            SourceLine line = new SourceLine();
            line.SourceString = "ld a,$1000";
            line.Parse(r => _asm.AssemblesInstruction(r));

            Assert.AreEqual(line.Instruction, "ld");
            Assert.AreEqual(line.Operand, "a,$1000");
        }

        [Test]
        public void TestZ80UPPERCASE()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "HALT";
            line.Operand = string.Empty;
            TestInstruction(line, 0x0001, new byte[] { 0x76 }, "halt");

            line.Instruction = "LD";
            line.Operand = "A,(IX+$00)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x7e, 0x00 }, "ld a,(ix+$00)");
        }

        [Test]
        public void TestZ80Ld()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "ld";
            line.Operand = "bc,$1234";
            TestInstruction(line, 0x0003, new byte[] { 0x01, 0x34, 0x12 }, "ld bc,$1234");

            line.Operand = "(bc),a";
            TestInstruction(line, 0x0001, new byte[] { 0x02 }, "ld (bc),a");

            line.Operand = "b,$12";
            TestInstruction(line, 0x0002, new byte[] { 0x06, 0x12 }, "ld b,$12");

            line.Operand = "a,(bc)";
            TestInstruction(line, 0x0001, new byte[] { 0x0a }, "ld a,(bc)");

            line.Operand = "c,$12";
            TestInstruction(line, 0x0002, new byte[] { 0x0e, 0x12 }, "ld c,$12");

            line.Operand = "de,$1234";
            TestInstruction(line, 0x0003, new byte[] { 0x11, 0x34, 0x12 }, "ld de,$1234");

            line.Operand = "(de),a";
            TestInstruction(line, 0x0001, new byte[] { 0x12 }, "ld (de),a");

            line.Operand = "d,$12";
            TestInstruction(line, 0x0002, new byte[] { 0x16, 0x12 }, "ld d,$12");

            line.Operand = "a,(de)";
            TestInstruction(line, 0x0001, new byte[] { 0x1a }, "ld a,(de)");

            line.Operand = "e,$12";
            TestInstruction(line, 0x0002, new byte[] { 0x1e, 0x12 }, "ld e,$12");

            line.Operand = "hl,$1234";
            TestInstruction(line, 0x0003, new byte[] { 0x21, 0x34, 0x12 }, "ld hl,$1234");

            line.Operand = "($1234),hl";
            TestInstruction(line, 0x0003, new byte[] { 0x22, 0x34, 0x12 }, "ld ($1234),hl");

            line.Operand = "h,$12";
            TestInstruction(line, 0x0002, new byte[] { 0x26, 0x12 }, "ld h,$12");

            line.Operand = "l,$12";
            TestInstruction(line, 0x0002, new byte[] { 0x2e, 0x12 }, "ld l,$12");

            line.Operand = "sp,$1234";
            TestInstruction(line, 0x0003, new byte[] { 0x31, 0x34, 0x12 }, "ld sp,$1234");

            line.Operand = "($1234),a";
            TestInstruction(line, 0x0003, new byte[] { 0x32, 0x34, 0x12 }, "ld ($1234),a");

            line.Operand = "(hl),$12";
            TestInstruction(line, 0x0002, new byte[] { 0x36, 0x12 }, "ld (hl),$12");

            line.Operand = "a,($1234)";
            TestInstruction(line, 0x0003, new byte[] { 0x3a, 0x34, 0x12 }, "ld a,($1234)");

            line.Operand = "a,$12";
            TestInstruction(line, 0x0002, new byte[] { 0x3e, 0x12 }, "ld a,$12");

            line.Operand = "b,b";
            TestInstruction(line, 0x0001, new byte[] { 0x40 }, "ld b,b");

            line.Operand = "b,c";
            TestInstruction(line, 0x0001, new byte[] { 0x41 }, "ld b,c");

            line.Operand = "b,d";
            TestInstruction(line, 0x0001, new byte[] { 0x42 }, "ld b,d");

            line.Operand = "b,e";
            TestInstruction(line, 0x0001, new byte[] { 0x43 }, "ld b,e");

            line.Operand = "b,h";
            TestInstruction(line, 0x0001, new byte[] { 0x44 }, "ld b,h");

            line.Operand = "b,l";
            TestInstruction(line, 0x0001, new byte[] { 0x45 }, "ld b,l");

            line.Operand = "b,(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x46 }, "ld b,(hl)");

            line.Operand = "b,a";
            TestInstruction(line, 0x0001, new byte[] { 0x47 }, "ld b,a");

            line.Operand = "c,b";
            TestInstruction(line, 0x0001, new byte[] { 0x48 }, "ld c,b");

            line.Operand = "c,c";
            TestInstruction(line, 0x0001, new byte[] { 0x49 }, "ld c,c");

            line.Operand = "c,d";
            TestInstruction(line, 0x0001, new byte[] { 0x4a }, "ld c,d");

            line.Operand = "c,e";
            TestInstruction(line, 0x0001, new byte[] { 0x4b }, "ld c,e");

            line.Operand = "c,h";
            TestInstruction(line, 0x0001, new byte[] { 0x4c }, "ld c,h");

            line.Operand = "c,l";
            TestInstruction(line, 0x0001, new byte[] { 0x4d }, "ld c,l");

            line.Operand = "c,(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x4e }, "ld c,(hl)");

            line.Operand = "c,a";
            TestInstruction(line, 0x0001, new byte[] { 0x4f }, "ld c,a");

            line.Operand = "d,b";
            TestInstruction(line, 0x0001, new byte[] { 0x50 }, "ld d,b");

            line.Operand = "d,c";
            TestInstruction(line, 0x0001, new byte[] { 0x51 }, "ld d,c");

            line.Operand = "d,d";
            TestInstruction(line, 0x0001, new byte[] { 0x52 }, "ld d,d");

            line.Operand = "d,e";
            TestInstruction(line, 0x0001, new byte[] { 0x53 }, "ld d,e");

            line.Operand = "d,h";
            TestInstruction(line, 0x0001, new byte[] { 0x54 }, "ld d,h");

            line.Operand = "d,l";
            TestInstruction(line, 0x0001, new byte[] { 0x55 }, "ld d,l");

            line.Operand = "d,(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x56 }, "ld d,(hl)");

            line.Operand = "d,a";
            TestInstruction(line, 0x0001, new byte[] { 0x57 }, "ld d,a");

            line.Operand = "e,b";
            TestInstruction(line, 0x0001, new byte[] { 0x58 }, "ld e,b");

            line.Operand = "e,c";
            TestInstruction(line, 0x0001, new byte[] { 0x59 }, "ld e,c");

            line.Operand = "e,d";
            TestInstruction(line, 0x0001, new byte[] { 0x5a }, "ld e,d");

            line.Operand = "e,e";
            TestInstruction(line, 0x0001, new byte[] { 0x5b }, "ld e,e");

            line.Operand = "e,h";
            TestInstruction(line, 0x0001, new byte[] { 0x5c }, "ld e,h");

            line.Operand = "e,l";
            TestInstruction(line, 0x0001, new byte[] { 0x5d }, "ld e,l");

            line.Operand = "e,(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x5e }, "ld e,(hl)");

            line.Operand = "e,a";
            TestInstruction(line, 0x0001, new byte[] { 0x5f }, "ld e,a");

            line.Operand = "h,b";
            TestInstruction(line, 0x0001, new byte[] { 0x60 }, "ld h,b");

            line.Operand = "h,c";
            TestInstruction(line, 0x0001, new byte[] { 0x61 }, "ld h,c");

            line.Operand = "h,d";
            TestInstruction(line, 0x0001, new byte[] { 0x62 }, "ld h,d");

            line.Operand = "h,e";
            TestInstruction(line, 0x0001, new byte[] { 0x63 }, "ld h,e");

            line.Operand = "h,h";
            TestInstruction(line, 0x0001, new byte[] { 0x64 }, "ld h,h");

            line.Operand = "h,l";
            TestInstruction(line, 0x0001, new byte[] { 0x65 }, "ld h,l");

            line.Operand = "h,(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x66 }, "ld h,(hl)");

            line.Operand = "h,a";
            TestInstruction(line, 0x0001, new byte[] { 0x67 }, "ld h,a");

            line.Operand = "l,b";
            TestInstruction(line, 0x0001, new byte[] { 0x68 }, "ld l,b");

            line.Operand = "l,c";
            TestInstruction(line, 0x0001, new byte[] { 0x69 }, "ld l,c");

            line.Operand = "l,d";
            TestInstruction(line, 0x0001, new byte[] { 0x6a }, "ld l,d");

            line.Operand = "l,e";
            TestInstruction(line, 0x0001, new byte[] { 0x6b }, "ld l,e");

            line.Operand = "l,h";
            TestInstruction(line, 0x0001, new byte[] { 0x6c }, "ld l,h");

            line.Operand = "l,l";
            TestInstruction(line, 0x0001, new byte[] { 0x6d }, "ld l,l");

            line.Operand = "l,(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x6e }, "ld l,(hl)");

            line.Operand = "l,a";
            TestInstruction(line, 0x0001, new byte[] { 0x6f }, "ld l,a");

            line.Operand = "(hl),b";
            TestInstruction(line, 0x0001, new byte[] { 0x70 }, "ld (hl),b");

            line.Operand = "(hl),c";
            TestInstruction(line, 0x0001, new byte[] { 0x71 }, "ld (hl),c");

            line.Operand = "(hl),d";
            TestInstruction(line, 0x0001, new byte[] { 0x72 }, "ld (hl),d");

            line.Operand = "(hl),e";
            TestInstruction(line, 0x0001, new byte[] { 0x73 }, "ld (hl),e");

            line.Operand = "(hl),h";
            TestInstruction(line, 0x0001, new byte[] { 0x74 }, "ld (hl),h");

            line.Operand = "(hl),l";
            TestInstruction(line, 0x0001, new byte[] { 0x75 }, "ld (hl),l");

            line.Operand = "(hl),a";
            TestInstruction(line, 0x0001, new byte[] { 0x77 }, "ld (hl),a");

            line.Operand = "a,b";
            TestInstruction(line, 0x0001, new byte[] { 0x78 }, "ld a,b");

            line.Operand = "a,c";
            TestInstruction(line, 0x0001, new byte[] { 0x79 }, "ld a,c");

            line.Operand = "a,d";
            TestInstruction(line, 0x0001, new byte[] { 0x7a }, "ld a,d");

            line.Operand = "a,e";
            TestInstruction(line, 0x0001, new byte[] { 0x7b }, "ld a,e");

            line.Operand = "a,h";
            TestInstruction(line, 0x0001, new byte[] { 0x7c }, "ld a,h");

            line.Operand = "a,l";
            TestInstruction(line, 0x0001, new byte[] { 0x7d }, "ld a,l");

            line.Operand = "a,(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x7e }, "ld a,(hl)");

            line.Operand = "a,a";
            TestInstruction(line, 0x0001, new byte[] { 0x7f }, "ld a,a");

            line.Operand = "a,($1234)";
            TestInstruction(line, 0x0003, new byte[] { 0x3a, 0x34, 0x12 }, "ld a,($1234)");

            line.Operand = "($1234),a";
            TestInstruction(line, 0x0003, new byte[] { 0x32, 0x34, 0x12 }, "ld ($1234),a");

            line.Operand = "hl,$1234";
            TestInstruction(line, 0x0003, new byte[] { 0x21, 0x34, 0x12 }, "ld hl,$1234");

            line.Operand = "hl,($1234)";
            TestInstruction(line, 0x0003, new byte[] { 0x2a, 0x34, 0x12 }, "ld hl,($1234)");

            line.Operand = "($1234),hl";
            TestInstruction(line, 0x0003, new byte[] { 0x22, 0x34, 0x12 }, "ld ($1234),hl");

            line.Operand = "($1234),bc";
            TestInstruction(line, 0x0004, new byte[] { 0xed, 0x43, 0x34, 0x12 }, "ld ($1234),bc");

            line.Operand = "i,a";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x47 }, "ld i,a");

            line.Operand = "bc,($1234)";
            TestInstruction(line, 0x0004, new byte[] { 0xed, 0x4b, 0x34, 0x12 }, "ld bc,($1234)");

            line.Operand = "r,a";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x4f }, "ld r,a");

            line.Operand = "($1234),de";
            TestInstruction(line, 0x0004, new byte[] { 0xed, 0x53, 0x34, 0x12 }, "ld ($1234),de");

            line.Operand = "a,i";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x57 }, "ld a,i");

            line.Operand = "de,($1234)";
            TestInstruction(line, 0x0004, new byte[] { 0xed, 0x5b, 0x34, 0x12 }, "ld de,($1234)");

            line.Operand = "a,r";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x5f }, "ld a,r");

            line.Operand = "($1234),sp";
            TestInstruction(line, 0x0004, new byte[] { 0xed, 0x73, 0x34, 0x12 }, "ld ($1234),sp");

            line.Operand = "sp,($1234)";
            TestInstruction(line, 0x0004, new byte[] { 0xed, 0x7b, 0x34, 0x12 }, "ld sp,($1234)");

            line.Operand = "a,-1";
            TestInstruction(line, 0x0002, new byte[] { 0x3e, 0xff }, "ld a,$ff");

            line.Operand = "hl,-1";
            TestInstruction(line, 0x0003, new byte[] { 0x21, 0xff, 0xff }, "ld hl,$ffff");
        }

        [Test]
        public void TestZ80LdIxy()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "ld";
            line.Operand = "ix,$1234";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0x21, 0x34, 0x12 }, "ld ix,$1234");

            line.Operand = "($1234),ix";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0x22, 0x34, 0x12 }, "ld ($1234),ix");

            line.Operand = "ixh,$12";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x26, 0x12 }, "ld ixh,$12");

            line.Operand = "ix,($1234)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0x2a, 0x34, 0x12 }, "ld ix,($1234)");

            line.Operand = "ixl,$12";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x2e, 0x12 }, "ld ixl,$12");

            line.Operand = "(ix - $12),$34";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0x36, 0xee, 0x34 }, "ld (ix-$12),$34");

            line.Operand = "b,ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x44 }, "ld b,ixh");

            line.Operand = "b,ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x45 }, "ld b,ixl");

            line.Operand = "b,(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x46, 0x12 }, "ld b,(ix+$12)");

            line.Operand = "c,ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x4c }, "ld c,ixh");

            line.Operand = "c,ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x4d }, "ld c,ixl");

            line.Operand = "c,(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x4e, 0x12 }, "ld c,(ix+$12)");

            line.Operand = "d,ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x54 }, "ld d,ixh");

            line.Operand = "d,ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x55 }, "ld d,ixl");

            line.Operand = "d,(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x56, 0x12 }, "ld d,(ix+$12)");

            line.Operand = "e,ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x5c }, "ld e,ixh");

            line.Operand = "e,ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x5d }, "ld e,ixl");

            line.Operand = "e,(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x5e, 0x12 }, "ld e,(ix+$12)");

            line.Operand = "ixh,b";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x60 }, "ld ixh,b");

            line.Operand = "ixh,c";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x61 }, "ld ixh,c");

            line.Operand = "ixh,d";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x62 }, "ld ixh,d");

            line.Operand = "ixh,e";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x63 }, "ld ixh,e");

            line.Operand = "ixh,h";
            TestForFailure<ExpressionException>(line);
            
            line.Operand = "ixh,l";
            TestForFailure<ExpressionException>(line);
            
            line.Operand = "ixh,ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x64 }, "ld ixh,ixh");

            line.Operand = "ixh,ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x65 }, "ld ixh,ixl");

            line.Operand = "h,(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x66, 0x12 }, "ld h,(ix+$12)");

            line.Operand = "ixh,a";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x67 }, "ld ixh,a");

            line.Operand = "ixl,b";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x68 }, "ld ixl,b");

            line.Operand = "ixl,c";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x69 }, "ld ixl,c");

            line.Operand = "ixl,d";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x6a }, "ld ixl,d");

            line.Operand = "ixl,e";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x6b }, "ld ixl,e");

            line.Operand = "ixl,h";
            TestForFailure<ExpressionException>(line);
            
            line.Operand = "ixl,l";
            TestForFailure<ExpressionException>(line);
            
            line.Operand = "ixl,ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x6c }, "ld ixl,ixh");

            line.Operand = "ixl,ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x6d }, "ld ixl,ixl");

            line.Operand = "l,(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x6e, 0x12 }, "ld l,(ix+$12)");

            line.Operand = "ixl,a";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x6f }, "ld ixl,a");

            line.Operand = "(ix+$12),b";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x70, 0x12 }, "ld (ix+$12),b");

            line.Operand = "(ix+$12),c";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x71, 0x12 }, "ld (ix+$12),c");

            line.Operand = "(ix+$12),d";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x72, 0x12 }, "ld (ix+$12),d");

            line.Operand = "(ix+$12),e";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x73, 0x12 }, "ld (ix+$12),e");

            line.Operand = "(ix+$12),h";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x74, 0x12 }, "ld (ix+$12),h");

            line.Operand = "(ix+$12),l";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x75, 0x12 }, "ld (ix+$12),l");

            line.Operand = "(ix+$12),a";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x77, 0x12 }, "ld (ix+$12),a");

            line.Operand = "a,ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x7c }, "ld a,ixh");

            line.Operand = "a,ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x7d }, "ld a,ixl");

            line.Operand = "a,(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x7e, 0x12 }, "ld a,(ix+$12)");

            line.Operand = "sp,ix";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xf9 }, "ld sp,ix");

            line.Operand = "iy,$1234";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0x21, 0x34, 0x12 }, "ld iy,$1234");

            line.Operand = "($1234),iy";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0x22, 0x34, 0x12 }, "ld ($1234),iy");

            line.Operand = "iyh,$12";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x26, 0x12 }, "ld iyh,$12");

            line.Operand = "iy,($1234)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0x2a, 0x34, 0x12 }, "ld iy,($1234)");

            line.Operand = "iyl,$12";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x2e, 0x12 }, "ld iyl,$12");

            line.Operand = "(iy+$12),$34";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0x36, 0x12, 0x34 }, "ld (iy+$12),$34");

            line.Operand = "b,iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x44 }, "ld b,iyh");

            line.Operand = "b,iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x45 }, "ld b,iyl");

            line.Operand = "b,(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x46, 0x12 }, "ld b,(iy+$12)");

            line.Operand = "c,iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x4c }, "ld c,iyh");

            line.Operand = "c,iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x4d }, "ld c,iyl");

            line.Operand = "c,(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x4e, 0x12 }, "ld c,(iy+$12)");

            line.Operand = "d,iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x54 }, "ld d,iyh");

            line.Operand = "d,iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x55 }, "ld d,iyl");

            line.Operand = "d,(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x56, 0x12 }, "ld d,(iy+$12)");

            line.Operand = "e,iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x5c }, "ld e,iyh");

            line.Operand = "e,iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x5d }, "ld e,iyl");

            line.Operand = "e,(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x5e, 0x12 }, "ld e,(iy+$12)");

            line.Operand = "iyh,b";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x60 }, "ld iyh,b");

            line.Operand = "iyh,c";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x61 }, "ld iyh,c");

            line.Operand = "iyh,d";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x62 }, "ld iyh,d");

            line.Operand = "iyh,e";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x63 }, "ld iyh,e");

            line.Operand = "iyh,h";
            TestForFailure<ExpressionException>(line);
            //TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x64 }, "ld iyh,h");

            line.Operand = "iyh,l";
            TestForFailure<ExpressionException>(line);
            
            line.Operand = "iyh,iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x64 }, "ld iyh,iyh");

            line.Operand = "iyh,iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x65 }, "ld iyh,iyl");

            line.Operand = "h,(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x66, 0x12 }, "ld h,(iy+$12)");

            line.Operand = "iyh,a";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x67 }, "ld iyh,a");

            line.Operand = "iyl,b";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x68 }, "ld iyl,b");

            line.Operand = "iyl,c";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x69 }, "ld iyl,c");

            line.Operand = "iyl,d";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x6a }, "ld iyl,d");

            line.Operand = "iyl,e";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x6b }, "ld iyl,e");

            line.Operand = "iyl,h";
            TestForFailure<ExpressionException>(line);
            //TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x6c }, "ld iyl,h");

            line.Operand = "iyl,l";
            TestForFailure<ExpressionException>(line);
            //TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x6d }, "ld iyl,l");

            line.Operand = "iyl,iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x6c }, "ld iyl,iyh");

            line.Operand = "iyl,iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x6d }, "ld iyl,iyl");

            line.Operand = "l,(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x6e, 0x12 }, "ld l,(iy+$12)");

            line.Operand = "iyl,a";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x6f }, "ld iyl,a");

            line.Operand = "(iy+$12),b";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x70, 0x12 }, "ld (iy+$12),b");

            line.Operand = "(iy+$12),c";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x71, 0x12 }, "ld (iy+$12),c");

            line.Operand = "(iy+$12),d";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x72, 0x12 }, "ld (iy+$12),d");

            line.Operand = "(iy+$12),e";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x73, 0x12 }, "ld (iy+$12),e");

            line.Operand = "(iy+$12),h";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x74, 0x12 }, "ld (iy+$12),h");

            line.Operand = "(iy+$12),l";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x75, 0x12 }, "ld (iy+$12),l");

            line.Operand = "(iy+$12),a";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x77, 0x12 }, "ld (iy+$12),a");

            line.Operand = "a,iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x7c }, "ld a,iyh");

            line.Operand = "a,iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x7d }, "ld a,iyl");

            line.Operand = "a,(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x7e, 0x12 }, "ld a,(iy+$12)");

            line.Operand = "sp,iy";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xf9 }, "ld sp,iy");
        }

        [Test]
        public void TestZ80Add()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "add";
            line.Operand = "hl,bc";
            TestInstruction(line, 0x0001, new byte[] { 0x09 }, "add hl,bc");

            line.Operand = "hl,de";
            TestInstruction(line, 0x0001, new byte[] { 0x19 }, "add hl,de");

            line.Operand = "hl,hl";
            TestInstruction(line, 0x0001, new byte[] { 0x29 }, "add hl,hl");

            line.Operand = "hl,sp";
            TestInstruction(line, 0x0001, new byte[] { 0x39 }, "add hl,sp");

            line.Operand = "a,b";
            TestInstruction(line, 0x0001, new byte[] { 0x80 }, "add a,b");

            line.Operand = "a,c";
            TestInstruction(line, 0x0001, new byte[] { 0x81 }, "add a,c");

            line.Operand = "a,d";
            TestInstruction(line, 0x0001, new byte[] { 0x82 }, "add a,d");

            line.Operand = "a,e";
            TestInstruction(line, 0x0001, new byte[] { 0x83 }, "add a,e");

            line.Operand = "a,h";
            TestInstruction(line, 0x0001, new byte[] { 0x84 }, "add a,h");

            line.Operand = "a,l";
            TestInstruction(line, 0x0001, new byte[] { 0x85 }, "add a,l");

            line.Operand = "a,(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x86 }, "add a,(hl)");

            line.Operand = "a,a";
            TestInstruction(line, 0x0001, new byte[] { 0x87 }, "add a,a");

            line.Operand = "a,$12";
            TestInstruction(line, 0x0002, new byte[] { 0xc6, 0x12 }, "add a,$12");

            line.Operand = "ix,bc";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x09 }, "add ix,bc");

            line.Operand = "ix,de";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x19 }, "add ix,de");

            line.Operand = "ix,ix";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x29 }, "add ix,ix");

            line.Operand = "ix,sp";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x39 }, "add ix,sp");

            line.Operand = "a,ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x84 }, "add a,ixh");

            line.Operand = "a,ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x85 }, "add a,ixl");

            line.Operand = "a,(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x86, 0x12 }, "add a,(ix+$12)");

            line.Operand = "iy,bc";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x09 }, "add iy,bc");

            line.Operand = "iy,de";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x19 }, "add iy,de");

            line.Operand = "iy,iy";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x29 }, "add iy,iy");

            line.Operand = "iy,sp";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x39 }, "add iy,sp");

            line.Operand = "a,iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x84 }, "add a,iyh");

            line.Operand = "a,iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x85 }, "add a,iyl");

            line.Operand = "a,(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x86, 0x12 }, "add a,(iy+$12)");

            line.Instruction = "adc";
            line.Operand = "a,b";
            TestInstruction(line, 0x0001, new byte[] { 0x88 }, "adc a,b");

            line.Operand = "a,c";
            TestInstruction(line, 0x0001, new byte[] { 0x89 }, "adc a,c");

            line.Operand = "a,d";
            TestInstruction(line, 0x0001, new byte[] { 0x8a }, "adc a,d");

            line.Operand = "a,e";
            TestInstruction(line, 0x0001, new byte[] { 0x8b }, "adc a,e");

            line.Operand = "a,h";
            TestInstruction(line, 0x0001, new byte[] { 0x8c }, "adc a,h");

            line.Operand = "a,l";
            TestInstruction(line, 0x0001, new byte[] { 0x8d }, "adc a,l");

            line.Operand = "a,(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x8e }, "adc a,(hl)");

            line.Operand = "a,a";
            TestInstruction(line, 0x0001, new byte[] { 0x8f }, "adc a,a");

            line.Operand = "a,$12";
            TestInstruction(line, 0x0002, new byte[] { 0xce, 0x12 }, "adc a,$12");

            line.Operand = "hl,bc";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x4a }, "adc hl,bc");

            line.Operand = "hl,de";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x5a }, "adc hl,de");

            line.Operand = "hl,hl";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x6a }, "adc hl,hl");

            line.Operand = "hl,sp";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x7a }, "adc hl,sp");

            line.Operand = "a,ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x8c }, "adc a,ixh");

            line.Operand = "a,ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x8d }, "adc a,ixl");

            line.Operand = "a,(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x8e, 0x12 }, "adc a,(ix+$12)");

            line.Operand = "a,iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x8c }, "adc a,iyh");

            line.Operand = "a,iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x8d }, "adc a,iyl");

            line.Operand = "a,(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x8e, 0x12 }, "adc a,(iy+$12)");
        }

        [Test]
        public void TestZ80Sub()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "sub";
            line.Operand = "b";
            TestInstruction(line, 0x0001, new byte[] { 0x90 }, "sub b");

            line.Operand = "c";
            TestInstruction(line, 0x0001, new byte[] { 0x91 }, "sub c");

            line.Operand = "d";
            TestInstruction(line, 0x0001, new byte[] { 0x92 }, "sub d");

            line.Operand = "e";
            TestInstruction(line, 0x0001, new byte[] { 0x93 }, "sub e");

            line.Operand = "h";
            TestInstruction(line, 0x0001, new byte[] { 0x94 }, "sub h");

            line.Operand = "l";
            TestInstruction(line, 0x0001, new byte[] { 0x95 }, "sub l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x96 }, "sub (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0001, new byte[] { 0x97 }, "sub a");

            line.Operand = "a,a";
            TestInstruction(line, 0x0001, new byte[] { 0x97 }, "sub a");

            line.Operand = "$12";
            TestInstruction(line, 0x0002, new byte[] { 0xd6, 0x12 }, "sub $12");

            line.Operand = "ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x94 }, "sub ixh");

            line.Operand = "ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x95 }, "sub ixl");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x96, 0x12 }, "sub (ix+$12)");

            line.Operand = "iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x94 }, "sub iyh");

            line.Operand = "iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x95 }, "sub iyl");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x96, 0x12 }, "sub (iy+$12)");

            line.Instruction = "sbc";
            line.Operand = "a,b";
            TestInstruction(line, 0x0001, new byte[] { 0x98 }, "sbc a,b");

            line.Operand = "a,c";
            TestInstruction(line, 0x0001, new byte[] { 0x99 }, "sbc a,c");

            line.Operand = "a,d";
            TestInstruction(line, 0x0001, new byte[] { 0x9a }, "sbc a,d");

            line.Operand = "a,e";
            TestInstruction(line, 0x0001, new byte[] { 0x9b }, "sbc a,e");

            line.Operand = "a,h";
            TestInstruction(line, 0x0001, new byte[] { 0x9c }, "sbc a,h");

            line.Operand = "a,l";
            TestInstruction(line, 0x0001, new byte[] { 0x9d }, "sbc a,l");

            line.Operand = "a,(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x9e }, "sbc a,(hl)");

            line.Operand = "a,a";
            TestInstruction(line, 0x0001, new byte[] { 0x9f }, "sbc a,a");

            line.Operand = "a,$12";
            TestInstruction(line, 0x0002, new byte[] { 0xde, 0x12 }, "sbc a,$12");

            line.Operand = "hl,bc";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x42 }, "sbc hl,bc");

            line.Operand = "hl,de";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x52 }, "sbc hl,de");

            line.Operand = "hl,hl";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x62 }, "sbc hl,hl");

            line.Operand = "hl,sp";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x72 }, "sbc hl,sp");

            line.Operand = "a,ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x9c }, "sbc a,ixh");

            line.Operand = "a,ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x9d }, "sbc a,ixl");

            line.Operand = "a,(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x9e, 0x12 }, "sbc a,(ix+$12)");

            line.Operand = "a,iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x9c }, "sbc a,iyh");

            line.Operand = "a,iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x9d }, "sbc a,iyl");

            line.Operand = "a,(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x9e, 0x12 }, "sbc a,(iy+$12)");
        }

        [Test]
        public void TestZ80AndOrXor()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "and";
            line.Operand = "b";
            TestInstruction(line, 0x0001, new byte[] { 0xa0 }, "and b");

            line.Operand = "c";
            TestInstruction(line, 0x0001, new byte[] { 0xa1 }, "and c");

            line.Operand = "d";
            TestInstruction(line, 0x0001, new byte[] { 0xa2 }, "and d");

            line.Operand = "e";
            TestInstruction(line, 0x0001, new byte[] { 0xa3 }, "and e");

            line.Operand = "h";
            TestInstruction(line, 0x0001, new byte[] { 0xa4 }, "and h");

            line.Operand = "l";
            TestInstruction(line, 0x0001, new byte[] { 0xa5 }, "and l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0xa6 }, "and (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0001, new byte[] { 0xa7 }, "and a");

            line.Operand = "a,a";
            TestInstruction(line, 0x0001, new byte[] { 0xa7 }, "and a");

            line.Operand = "$12";
            TestInstruction(line, 0x0002, new byte[] { 0xe6, 0x12 }, "and $12");

            line.Operand = "ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xa4 }, "and ixh");

            line.Operand = "ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xa5 }, "and ixl");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0xa6, 0x12 }, "and (ix+$12)");

            line.Operand = "iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xa4 }, "and iyh");

            line.Operand = "iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xa5 }, "and iyl");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0xa6, 0x12 }, "and (iy+$12)");

            line.Instruction = "xor";
            line.Operand = "b";
            TestInstruction(line, 0x0001, new byte[] { 0xa8 }, "xor b");

            line.Operand = "c";
            TestInstruction(line, 0x0001, new byte[] { 0xa9 }, "xor c");

            line.Operand = "d";
            TestInstruction(line, 0x0001, new byte[] { 0xaa }, "xor d");

            line.Operand = "e";
            TestInstruction(line, 0x0001, new byte[] { 0xab }, "xor e");

            line.Operand = "h";
            TestInstruction(line, 0x0001, new byte[] { 0xac }, "xor h");

            line.Operand = "l";
            TestInstruction(line, 0x0001, new byte[] { 0xad }, "xor l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0xae }, "xor (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0001, new byte[] { 0xaf }, "xor a");

            line.Operand = "a,a";
            TestInstruction(line, 0x0001, new byte[] { 0xaf }, "xor a");

            line.Operand = "$12";
            TestInstruction(line, 0x0002, new byte[] { 0xee, 0x12 }, "xor $12");

            line.Operand = "ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xac }, "xor ixh");

            line.Operand = "ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xad }, "xor ixl");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0xae, 0x12 }, "xor (ix+$12)");

            line.Operand = "iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xac }, "xor iyh");

            line.Operand = "iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xad }, "xor iyl");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0xae, 0x12 }, "xor (iy+$12)");

            line.Instruction = "or";
            line.Operand = "b";
            TestInstruction(line, 0x0001, new byte[] { 0xb0 }, "or b");

            line.Operand = "c";
            TestInstruction(line, 0x0001, new byte[] { 0xb1 }, "or c");

            line.Operand = "d";
            TestInstruction(line, 0x0001, new byte[] { 0xb2 }, "or d");

            line.Operand = "e";
            TestInstruction(line, 0x0001, new byte[] { 0xb3 }, "or e");

            line.Operand = "h";
            TestInstruction(line, 0x0001, new byte[] { 0xb4 }, "or h");

            line.Operand = "l";
            TestInstruction(line, 0x0001, new byte[] { 0xb5 }, "or l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0xb6 }, "or (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0001, new byte[] { 0xb7 }, "or a");

            line.Operand = "a,a";
            TestInstruction(line, 0x0001, new byte[] { 0xb7 }, "or a");

            line.Operand = "$12";
            TestInstruction(line, 0x0002, new byte[] { 0xf6, 0x12 }, "or $12");

            line.Operand = "ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xb4 }, "or ixh");

            line.Operand = "ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xb5 }, "or ixl");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0xb6, 0x12 }, "or (ix+$12)");

            line.Operand = "iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xb4 }, "or iyh");

            line.Operand = "iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xb5 }, "or iyl");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0xb6, 0x12 }, "or (iy+$12)");

            line.Instruction = "cp";
            line.Operand = "b";
            TestInstruction(line, 0x0001, new byte[] { 0xb8 }, "cp b");

            line.Operand = "c";
            TestInstruction(line, 0x0001, new byte[] { 0xb9 }, "cp c");

            line.Operand = "d";
            TestInstruction(line, 0x0001, new byte[] { 0xba }, "cp d");

            line.Operand = "e";
            TestInstruction(line, 0x0001, new byte[] { 0xbb }, "cp e");

            line.Operand = "h";
            TestInstruction(line, 0x0001, new byte[] { 0xbc }, "cp h");

            line.Operand = "l";
            TestInstruction(line, 0x0001, new byte[] { 0xbd }, "cp l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0xbe }, "cp (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0001, new byte[] { 0xbf }, "cp a");

            line.Operand = "a,a";
            TestInstruction(line, 0x0001, new byte[] { 0xbf }, "cp a");

            line.Operand = "$12";
            TestInstruction(line, 0x0002, new byte[] { 0xfe, 0x12 }, "cp $12");

            line.Operand = "ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xbc }, "cp ixh");

            line.Operand = "ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xbd }, "cp ixl");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0xbe, 0x12 }, "cp (ix+$12)");

            line.Operand = "iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xbc }, "cp iyh");

            line.Operand = "iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xbd }, "cp iyl");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0xbe, 0x12 }, "cp (iy+$12)");
        }

        [Test]
        public void TestZ80Branch()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "jr";
            line.Operand = "$0000";
            _controller.Output.SetPC(0x0006);
            TestInstruction(line, 0x0008, new byte[] { 0x18, (byte)(256 - (0x0006 + 2)) }, "jr $0000");

            line.Operand = "$0006";
            TestInstruction(line, 0x0002, new byte[] { 0x18, (byte)(0x0006 - (0x0000 + 2)) }, "jr $0006");

            line.Operand = "nz,$0006";
            TestInstruction(line, 0x0002, new byte[] { 0x20, 0x04 }, "jr nz,$0006");

            _controller.Output.SetPC(0x0006);
            line.Operand = "z,$0000";
            TestInstruction(line, 0x0008, new byte[] { 0x28, 0xf8 }, "jr z,$0000");

            line.Operand = "nc,$0006";
            TestInstruction(line, 0x0002, new byte[] { 0x30, 0x04 }, "jr nc,$0006");

            _controller.Output.SetPC(0x0006);
            line.Operand = "c,$0000";
            TestInstruction(line, 0x0008, new byte[] { 0x38, 0xf8 }, "jr c,$0000");

            line.Instruction = "djnz";
            line.Operand = "$0006";
            TestInstruction(line, 0x0002, new byte[] { 0x10, 0x04 }, "djnz $0006");

            line.Instruction = "ret";
            line.Operand = "nz";
            TestInstruction(line, 0x0001, new byte[] { 0xc0 }, "ret nz");

            line.Operand = "z";
            TestInstruction(line, 0x0001, new byte[] { 0xc8 }, "ret z");

            line.Operand = string.Empty;
            TestInstruction(line, 0x0001, new byte[] { 0xc9 }, "ret");

            line.Operand = "nc";
            TestInstruction(line, 0x0001, new byte[] { 0xd0 }, "ret nc");

            line.Operand = "c";
            TestInstruction(line, 0x0001, new byte[] { 0xd8 }, "ret c");

            line.Operand = "po";
            TestInstruction(line, 0x0001, new byte[] { 0xe0 }, "ret po");

            line.Operand = "pe";
            TestInstruction(line, 0x0001, new byte[] { 0xe8 }, "ret pe");

            line.Operand = "p";
            TestInstruction(line, 0x0001, new byte[] { 0xf0 }, "ret p");

            line.Operand = "m";
            TestInstruction(line, 0x0001, new byte[] { 0xf8 }, "ret m");

            line.Instruction = "jp";
            line.Operand = "nz,$0012";
            TestInstruction(line, 0x0003, new byte[] { 0xc2, 0x12, 0x00 }, "jp nz,$0012");

            line.Operand = "$12";
            TestInstruction(line, 0x0003, new byte[] { 0xc3, 0x12, 0x00 }, "jp $0012");

            line.Operand = "z,$0012";
            TestInstruction(line, 0x0003, new byte[] { 0xca, 0x12, 0x00 }, "jp z,$0012");

            line.Operand = "nc,$0012";
            TestInstruction(line, 0x0003, new byte[] { 0xd2, 0x12, 0x00 }, "jp nc,$0012");

            line.Operand = "c,$0012";
            TestInstruction(line, 0x0003, new byte[] { 0xda, 0x12, 0x00 }, "jp c,$0012");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0xe9 }, "jp (hl)");

            line.Operand = "(ix)";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xe9 }, "jp (ix)");

            line.Operand = "(iy)";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xe9 }, "jp (iy)");

            line.Instruction = "call";
            line.Operand = "nz,$12";
            TestInstruction(line, 0x0003, new byte[] { 0xc4, 0x12, 0x00 }, "call nz,$0012");

            line.Operand = "z,$0012";
            TestInstruction(line, 0x0003, new byte[] { 0xcc, 0x12, 0x00 }, "call z,$0012");

            line.Operand = "$012";
            TestInstruction(line, 0x0003, new byte[] { 0xcd, 0x12, 0x00 }, "call $0012");

            line.Operand = "nc,$0012";
            TestInstruction(line, 0x0003, new byte[] { 0xd4, 0x12, 0x00 }, "call nc,$0012");

            line.Operand = "c,$0012";
            TestInstruction(line, 0x0003, new byte[] { 0xdc, 0x12, 0x00 }, "call c,$0012");

            line.Operand = "po,$0012";
            TestInstruction(line, 0x0003, new byte[] { 0xe4, 0x12, 0x00 }, "call po,$0012");

            line.Operand = "pe,$0012";
            TestInstruction(line, 0x0003, new byte[] { 0xec, 0x12, 0x00 }, "call pe,$0012");

            line.Operand = "p,18";
            TestInstruction(line, 0x0003, new byte[] { 0xf4, 0x12, 0x00 }, "call p,$0012");

            line.Operand = "m , $0012";
            TestInstruction(line, 0x0003, new byte[] { 0xfc, 0x12, 0x00 }, "call m,$0012");

            line.Instruction = "ret";
            line.Operand = "nz";
            TestInstruction(line, 0x0001, new byte[] { 0xc0 }, "ret nz");

            line.Operand = "z";
            TestInstruction(line, 0x0001, new byte[] { 0xc8 }, "ret z");

            line.Operand = string.Empty;
            TestInstruction(line, 0x0001, new byte[] { 0xc9 }, "ret");

            line.Operand = "nc";
            TestInstruction(line, 0x0001, new byte[] { 0xd0 }, "ret nc");

            line.Operand = "c";
            TestInstruction(line, 0x0001, new byte[] { 0xd8 }, "ret c");

            line.Operand = "po";
            TestInstruction(line, 0x0001, new byte[] { 0xe0 }, "ret po");

            line.Operand = "pe";
            TestInstruction(line, 0x0001, new byte[] { 0xe8 }, "ret pe");

            line.Operand = "p";
            TestInstruction(line, 0x0001, new byte[] { 0xf0 }, "ret p");

            line.Operand = "m";
            TestInstruction(line, 0x0001, new byte[] { 0xf8 }, "ret m");
        }

        [Test]
        public void TestZ80Bits()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "bit";
            line.Operand = "0,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x40 }, "bit 0,b");

            line.Operand = "0,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x41 }, "bit 0,c");

            line.Operand = "0,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x42 }, "bit 0,d");

            line.Operand = "0,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x43 }, "bit 0,e");

            line.Operand = "0,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x44 }, "bit 0,h");

            line.Operand = "0,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x45 }, "bit 0,l");

            line.Operand = "0,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x46 }, "bit 0,(hl)");

            line.Operand = "0,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x47 }, "bit 0,a");

            line.Operand = "1,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x48 }, "bit 1,b");

            line.Operand = "1,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x49 }, "bit 1,c");

            line.Operand = "1,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x4a }, "bit 1,d");

            line.Operand = "1,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x4b }, "bit 1,e");

            line.Operand = "1,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x4c }, "bit 1,h");

            line.Operand = "1,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x4d }, "bit 1,l");

            line.Operand = "1,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x4e }, "bit 1,(hl)");

            line.Operand = "1,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x4f }, "bit 1,a");

            line.Operand = "2,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x50 }, "bit 2,b");

            line.Operand = "2,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x51 }, "bit 2,c");

            line.Operand = "2,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x52 }, "bit 2,d");

            line.Operand = "2,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x53 }, "bit 2,e");

            line.Operand = "2,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x54 }, "bit 2,h");

            line.Operand = "2,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x55 }, "bit 2,l");

            line.Operand = "2,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x56 }, "bit 2,(hl)");

            line.Operand = "2,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x57 }, "bit 2,a");

            line.Operand = "3,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x58 }, "bit 3,b");

            line.Operand = "3,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x59 }, "bit 3,c");

            line.Operand = "3,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x5a }, "bit 3,d");

            line.Operand = "3,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x5b }, "bit 3,e");

            line.Operand = "3,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x5c }, "bit 3,h");

            line.Operand = "3,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x5d }, "bit 3,l");

            line.Operand = "3,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x5e }, "bit 3,(hl)");

            line.Operand = "3,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x5f }, "bit 3,a");

            line.Operand = "4,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x60 }, "bit 4,b");

            line.Operand = "4,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x61 }, "bit 4,c");

            line.Operand = "4,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x62 }, "bit 4,d");

            line.Operand = "4,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x63 }, "bit 4,e");

            line.Operand = "4,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x64 }, "bit 4,h");

            line.Operand = "4,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x65 }, "bit 4,l");

            line.Operand = "4,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x66 }, "bit 4,(hl)");

            line.Operand = "4,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x67 }, "bit 4,a");

            line.Operand = "5,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x68 }, "bit 5,b");

            line.Operand = "5,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x69 }, "bit 5,c");

            line.Operand = "5,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x6a }, "bit 5,d");

            line.Operand = "10-5,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x6b }, "bit 5,e");

            line.Operand = "5,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x6c }, "bit 5,h");

            line.Operand = "5,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x6d }, "bit 5,l");

            line.Operand = "5,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x6e }, "bit 5,(hl)");

            line.Operand = "5,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x6f }, "bit 5,a");

            line.Operand = "6,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x70 }, "bit 6,b");

            line.Operand = "6,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x71 }, "bit 6,c");

            line.Operand = "6,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x72 }, "bit 6,d");

            line.Operand = "6,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x73 }, "bit 6,e");

            line.Operand = "6,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x74 }, "bit 6,h");

            line.Operand = "6,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x75 }, "bit 6,l");

            line.Operand = "6,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x76 }, "bit 6,(hl)");

            line.Operand = "6,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x77 }, "bit 6,a");

            line.Operand = "7,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x78 }, "bit 7,b");

            line.Operand = "7,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x79 }, "bit 7,c");

            line.Operand = "7,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x7a }, "bit 7,d");

            line.Operand = "7,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x7b }, "bit 7,e");

            line.Operand = "7,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x7c }, "bit 7,h");

            line.Operand = "7,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x7d }, "bit 7,l");

            line.Operand = "7,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x7e }, "bit 7,(hl)");

            line.Operand = "7,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x7f }, "bit 7,a");

            line.Instruction = "res";
            line.Operand = "0,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x80 }, "res 0,b");

            line.Operand = "0,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x81 }, "res 0,c");

            line.Operand = "0,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x82 }, "res 0,d");

            line.Operand = "0,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x83 }, "res 0,e");

            line.Operand = "0,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x84 }, "res 0,h");

            line.Operand = "0,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x85 }, "res 0,l");

            line.Operand = "0,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x86 }, "res 0,(hl)");

            line.Operand = "0,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x87 }, "res 0,a");

            line.Operand = "1,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x88 }, "res 1,b");

            line.Operand = "1,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x89 }, "res 1,c");

            line.Operand = "1,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x8a }, "res 1,d");

            line.Operand = "1,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x8b }, "res 1,e");

            line.Operand = "1,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x8c }, "res 1,h");

            line.Operand = "1,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x8d }, "res 1,l");

            line.Operand = "1,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x8e }, "res 1,(hl)");

            line.Operand = "1,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x8f }, "res 1,a");

            line.Operand = "2,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x90 }, "res 2,b");

            line.Operand = "2,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x91 }, "res 2,c");

            line.Operand = "2,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x92 }, "res 2,d");

            line.Operand = "2,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x93 }, "res 2,e");

            line.Operand = "2,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x94 }, "res 2,h");

            line.Operand = "2,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x95 }, "res 2,l");

            line.Operand = "2,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x96 }, "res 2,(hl)");

            line.Operand = "2,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x97 }, "res 2,a");

            line.Operand = "3,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x98 }, "res 3,b");

            line.Operand = "3,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x99 }, "res 3,c");

            line.Operand = "3,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x9a }, "res 3,d");

            line.Operand = "3,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x9b }, "res 3,e");

            line.Operand = "3,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x9c }, "res 3,h");

            line.Operand = "3,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x9d }, "res 3,l");

            line.Operand = "3,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x9e }, "res 3,(hl)");

            line.Operand = "3,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x9f }, "res 3,a");

            line.Operand = "4,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xa0 }, "res 4,b");

            line.Operand = "4,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xa1 }, "res 4,c");

            line.Operand = "4,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xa2 }, "res 4,d");

            line.Operand = "4,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xa3 }, "res 4,e");

            line.Operand = "4,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xa4 }, "res 4,h");

            line.Operand = "4,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xa5 }, "res 4,l");

            line.Operand = "4,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xa6 }, "res 4,(hl)");

            line.Operand = "4,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xa7 }, "res 4,a");

            line.Operand = "5,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xa8 }, "res 5,b");

            line.Operand = "5,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xa9 }, "res 5,c");

            line.Operand = "5,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xaa }, "res 5,d");

            line.Operand = "5,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xab }, "res 5,e");

            line.Operand = "5,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xac }, "res 5,h");

            line.Operand = "5,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xad }, "res 5,l");

            line.Operand = "5,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xae }, "res 5,(hl)");

            line.Operand = "5,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xaf }, "res 5,a");

            line.Operand = "6,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xb0 }, "res 6,b");

            line.Operand = "6,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xb1 }, "res 6,c");

            line.Operand = "6,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xb2 }, "res 6,d");

            line.Operand = "6,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xb3 }, "res 6,e");

            line.Operand = "6,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xb4 }, "res 6,h");

            line.Operand = "6,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xb5 }, "res 6,l");

            line.Operand = "6,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xb6 }, "res 6,(hl)");

            line.Operand = "6,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xb7 }, "res 6,a");

            line.Operand = "7,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xb8 }, "res 7,b");

            line.Operand = "7,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xb9 }, "res 7,c");

            line.Operand = "7,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xba }, "res 7,d");

            line.Operand = "7,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xbb }, "res 7,e");

            line.Operand = "7,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xbc }, "res 7,h");

            line.Operand = "7,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xbd }, "res 7,l");

            line.Operand = "7,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xbe }, "res 7,(hl)");

            line.Operand = "7,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xbf }, "res 7,a");

            line.Instruction = "set";
            line.Operand = "0,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xc0 }, "set 0,b");

            line.Operand = "0,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xc1 }, "set 0,c");

            line.Operand = "0,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xc2 }, "set 0,d");

            line.Operand = "0,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xc3 }, "set 0,e");

            line.Operand = "0,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xc4 }, "set 0,h");

            line.Operand = "0,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xc5 }, "set 0,l");

            line.Operand = "0,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xc6 }, "set 0,(hl)");

            line.Operand = "0,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xc7 }, "set 0,a");

            line.Operand = "1,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xc8 }, "set 1,b");

            line.Operand = "1,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xc9 }, "set 1,c");

            line.Operand = "1,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xca }, "set 1,d");

            line.Operand = "1,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xcb }, "set 1,e");

            line.Operand = "1,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xcc }, "set 1,h");

            line.Operand = "1,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xcd }, "set 1,l");

            line.Operand = "1,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xce }, "set 1,(hl)");

            line.Operand = "1,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xcf }, "set 1,a");

            line.Operand = "2,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xd0 }, "set 2,b");

            line.Operand = "2,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xd1 }, "set 2,c");

            line.Operand = "2,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xd2 }, "set 2,d");

            line.Operand = "2,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xd3 }, "set 2,e");

            line.Operand = "2,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xd4 }, "set 2,h");

            line.Operand = "2,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xd5 }, "set 2,l");

            line.Operand = "2,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xd6 }, "set 2,(hl)");

            line.Operand = "2,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xd7 }, "set 2,a");

            line.Operand = "3,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xd8 }, "set 3,b");

            line.Operand = "3,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xd9 }, "set 3,c");

            line.Operand = "3,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xda }, "set 3,d");

            line.Operand = "3,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xdb }, "set 3,e");

            line.Operand = "3,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xdc }, "set 3,h");

            line.Operand = "3,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xdd }, "set 3,l");

            line.Operand = "3,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xde }, "set 3,(hl)");

            line.Operand = "3,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xdf }, "set 3,a");

            line.Operand = "4,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xe0 }, "set 4,b");

            line.Operand = "4,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xe1 }, "set 4,c");

            line.Operand = "4,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xe2 }, "set 4,d");

            line.Operand = "4,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xe3 }, "set 4,e");

            line.Operand = "4,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xe4 }, "set 4,h");

            line.Operand = "4,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xe5 }, "set 4,l");

            line.Operand = "4,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xe6 }, "set 4,(hl)");

            line.Operand = "4,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xe7 }, "set 4,a");

            line.Operand = "5,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xe8 }, "set 5,b");

            line.Operand = "5,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xe9 }, "set 5,c");

            line.Operand = "5,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xea }, "set 5,d");

            line.Operand = "5,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xeb }, "set 5,e");

            line.Operand = "5,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xec }, "set 5,h");

            line.Operand = "5,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xed }, "set 5,l");

            line.Operand = "5,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xee }, "set 5,(hl)");

            line.Operand = "5,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xef }, "set 5,a");

            line.Operand = "6,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xf0 }, "set 6,b");

            line.Operand = "6,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xf1 }, "set 6,c");

            line.Operand = "6,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xf2 }, "set 6,d");

            line.Operand = "6,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xf3 }, "set 6,e");

            line.Operand = "6,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xf4 }, "set 6,h");

            line.Operand = "6,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xf5 }, "set 6,l");

            line.Operand = "6,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xf6 }, "set 6,(hl)");

            line.Operand = "6,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xf7 }, "set 6,a");

            line.Operand = "7,b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xf8 }, "set 7,b");

            line.Operand = "7,c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xf9 }, "set 7,c");

            line.Operand = "7,d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xfa }, "set 7,d");

            line.Operand = "7,e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xfb }, "set 7,e");

            line.Operand = "7,h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xfc }, "set 7,h");

            line.Operand = "7,l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xfd }, "set 7,l");

            line.Operand = "7,(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xfe }, "set 7,(hl)");

            line.Operand = "7,a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0xff }, "set 7,a");
        }

        [Test]
        public void TestZ80PopPush()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "pop";
            line.Operand = "bc";
            TestInstruction(line, 0x0001, new byte[] { 0xc1 }, "pop bc");

            line.Operand = "de";
            TestInstruction(line, 0x0001, new byte[] { 0xd1 }, "pop de");

            line.Operand = "hl";
            TestInstruction(line, 0x0001, new byte[] { 0xe1 }, "pop hl");

            line.Operand = "af";
            TestInstruction(line, 0x0001, new byte[] { 0xf1 }, "pop af");

            line.Operand = "ix";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xe1 }, "pop ix");

            line.Operand = "iy";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xe1 }, "pop iy");

            line.Instruction = "push";
            line.Operand = "bc";
            TestInstruction(line, 0x0001, new byte[] { 0xc5 }, "push bc");

            line.Operand = "de";
            TestInstruction(line, 0x0001, new byte[] { 0xd5 }, "push de");

            line.Operand = "hl";
            TestInstruction(line, 0x0001, new byte[] { 0xe5 }, "push hl");

            line.Operand = "af";
            TestInstruction(line, 0x0001, new byte[] { 0xf5 }, "push af");

            line.Operand = "ix";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xe5 }, "push ix");

            line.Operand = "iy";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xe5 }, "push iy");
        }

        [Test]
        public void TestZ80Exchange()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "ex";
            line.Operand = "af,af'";
            TestInstruction(line, 0x0001, new byte[] { 0x08 }, "ex af,af'");

            line.Operand = "(sp),hl";
            TestInstruction(line, 0x0001, new byte[] { 0xe3 }, "ex (sp),hl");

            line.Operand = "de,hl";
            TestInstruction(line, 0x0001, new byte[] { 0xeb }, "ex de,hl");

            line.Operand = "(sp),ix";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0xe3 }, "ex (sp),ix");

            line.Operand = "(sp),iy";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0xe3 }, "ex (sp),iy");

            line.Instruction = "exx";
            line.Operand = string.Empty;
            TestInstruction(line, 0x0001, new byte[] { 0xd9 }, "exx");
        }

        [Test]
        public void TestZ80IncDec()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "inc";
            line.Operand = "bc";
            TestInstruction(line, 0x0001, new byte[] { 0x03 }, "inc bc");

            line.Operand = "b";
            TestInstruction(line, 0x0001, new byte[] { 0x04 }, "inc b");

            line.Operand = "c";
            TestInstruction(line, 0x0001, new byte[] { 0x0c }, "inc c");

            line.Operand = "de";
            TestInstruction(line, 0x0001, new byte[] { 0x13 }, "inc de");

            line.Operand = "d";
            TestInstruction(line, 0x0001, new byte[] { 0x14 }, "inc d");

            line.Operand = "e";
            TestInstruction(line, 0x0001, new byte[] { 0x1c }, "inc e");

            line.Operand = "hl";
            TestInstruction(line, 0x0001, new byte[] { 0x23 }, "inc hl");

            line.Operand = "h";
            TestInstruction(line, 0x0001, new byte[] { 0x24 }, "inc h");

            line.Operand = "l";
            TestInstruction(line, 0x0001, new byte[] { 0x2c }, "inc l");

            line.Operand = "sp";
            TestInstruction(line, 0x0001, new byte[] { 0x33 }, "inc sp");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x34 }, "inc (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0001, new byte[] { 0x3c }, "inc a");

            line.Operand = "ix";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x23 }, "inc ix");

            line.Operand = "ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x24 }, "inc ixh");

            line.Operand = "ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x2c }, "inc ixl");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x34, 0x12 }, "inc (ix+$12)");

            line.Operand = "iy";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x23 }, "inc iy");

            line.Operand = "iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x24 }, "inc iyh");

            line.Operand = "iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x2c }, "inc iyl");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x34, 0x12 }, "inc (iy+$12)");

            line.Instruction = "dec";
            line.Operand = "bc";
            TestInstruction(line, 0x0001, new byte[] { 0x0b }, "dec bc");

            line.Operand = "b";
            TestInstruction(line, 0x0001, new byte[] { 0x05 }, "dec b");

            line.Operand = "c";
            TestInstruction(line, 0x0001, new byte[] { 0x0d }, "dec c");

            line.Operand = "de";
            TestInstruction(line, 0x0001, new byte[] { 0x1b }, "dec de");

            line.Operand = "d";
            TestInstruction(line, 0x0001, new byte[] { 0x15 }, "dec d");

            line.Operand = "e";
            TestInstruction(line, 0x0001, new byte[] { 0x1d }, "dec e");

            line.Operand = "hl";
            TestInstruction(line, 0x0001, new byte[] { 0x2b }, "dec hl");

            line.Operand = "h";
            TestInstruction(line, 0x0001, new byte[] { 0x25 }, "dec h");

            line.Operand = "l";
            TestInstruction(line, 0x0001, new byte[] { 0x2d }, "dec l");

            line.Operand = "sp";
            TestInstruction(line, 0x0001, new byte[] { 0x3b }, "dec sp");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0001, new byte[] { 0x35 }, "dec (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0001, new byte[] { 0x3d }, "dec a");

            line.Operand = "ix";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x2b }, "dec ix");

            line.Operand = "ixh";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x25 }, "dec ixh");

            line.Operand = "ixl";
            TestInstruction(line, 0x0002, new byte[] { 0xdd, 0x2d }, "dec ixl");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x35, 0x12 }, "dec (ix+$12)");

            line.Operand = "iy";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x2b }, "dec iy");

            line.Operand = "iyh";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x25 }, "dec iyh");

            line.Operand = "iyl";
            TestInstruction(line, 0x0002, new byte[] { 0xfd, 0x2d }, "dec iyl");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0003, new byte[] { 0xfd, 0x35, 0x12 }, "dec (iy+$12)");
        }
        [Test]
        public void TestZ80Interrupts()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "di";
            line.Operand = string.Empty;
            TestInstruction(line, 0x0001, new byte[] { 0xf3 }, "di");

            line.Instruction = "ei";
            TestInstruction(line, 0x0001, new byte[] { 0xfb }, "ei");

            line.Instruction = "rst";
            line.Operand = "$00";
            TestInstruction(line, 0x0001, new byte[] { 0xc7 }, "rst $00");

            line.Operand = "$08";
            TestInstruction(line, 0x0001, new byte[] { 0xcf }, "rst $08");

            line.Operand = "$10";
            TestInstruction(line, 0x0001, new byte[] { 0xd7 }, "rst $10");

            line.Operand = "$18";
            TestInstruction(line, 0x0001, new byte[] { 0xdf }, "rst $18");

            line.Operand = "$20";
            TestInstruction(line, 0x0001, new byte[] { 0xe7 }, "rst $20");

            line.Operand = "$28";
            TestInstruction(line, 0x0001, new byte[] { 0xef }, "rst $28");

            line.Operand = "$30";
            TestInstruction(line, 0x0001, new byte[] { 0xf7 }, "rst $30");

            line.Operand = "56";
            TestInstruction(line, 0x0001, new byte[] { 0xff }, "rst $38");

            line.Instruction = "im";
            line.Operand = "0";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x46 }, "im 0");

            line.Operand = "1";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x56 }, "im 1");

            line.Operand = "2";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x5e }, "im 2");

            line.Instruction = "retn";
            line.Operand = string.Empty;
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x45 }, "retn");

            line.Instruction = "reti";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x4d }, "reti");
        }

        [Test]
        public void TestZ80InOut()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "in";
            line.Operand = "a,($12)";
            TestInstruction(line, 0x0002, new byte[] { 0xdb, 0x12 }, "in a,($12)");

            line.Operand = "b,(c)";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x40 }, "in b,(c)");

            line.Operand = "c,(c)";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x48 }, "in c,(c)");

            line.Operand = "d,(c)";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x50 }, "in d,(c)");

            line.Operand = "e,(c)";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x58 }, "in e,(c)");

            line.Operand = "h,(c)";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x60 }, "in h,(c)");

            line.Operand = "l,(c)";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x68 }, "in l,(c)");

            line.Operand = "(c)";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x70 }, "in (c)");

            line.Operand = "a,(c)";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x78 }, "in a,(c)");

            line.Instruction = "out";
            line.Operand = "($12),a";
            TestInstruction(line, 0x0002, new byte[] { 0xd3, 0x12 }, "out ($12),a");

            line.Operand = "(c),b";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x41 }, "out (c),b");

            line.Operand = "(c),c";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x49 }, "out (c),c");

            line.Operand = "(c),d";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x51 }, "out (c),d");

            line.Operand = "(c),e";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x59 }, "out (c),e");

            line.Operand = "(c),h";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x61 }, "out (c),h");

            line.Operand = "(c),l";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x69 }, "out (c),l");

            line.Operand = "(c),0";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x71 }, "out (c),0");

            line.Operand = "(c),a";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x79 }, "out (c),a");
        }

        [Test]
        public void TestZ80Misc()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "nop";
            line.Operand = string.Empty;
            TestInstruction(line, 0x0001, new byte[] { 0x00 }, "nop");

            line.Instruction = "daa";
            TestInstruction(line, 0x0001, new byte[] { 0x27 }, "daa");

            line.Instruction = "cpl";
            TestInstruction(line, 0x0001, new byte[] { 0x2f }, "cpl");

            line.Instruction = "scf";
            TestInstruction(line, 0x0001, new byte[] { 0x37 }, "scf");

            line.Instruction = "ccf";
            TestInstruction(line, 0x0001, new byte[] { 0x3f }, "ccf");

            line.Instruction = "halt";
            TestInstruction(line, 0x0001, new byte[] { 0x76 }, "halt");

            line.Instruction = "neg";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x44 }, "neg");

            line.Instruction = "rrd";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x67 }, "rrd");

            line.Instruction = "rld";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0x6f }, "rld");

            line.Instruction = "ldi";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xa0 }, "ldi");

            line.Instruction = "cpi";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xa1 }, "cpi");

            line.Instruction = "ini";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xa2 }, "ini");

            line.Instruction = "outi";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xa3 }, "outi");

            line.Instruction = "ldd";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xa8 }, "ldd");

            line.Instruction = "cpd";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xa9 }, "cpd");

            line.Instruction = "ind";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xaa }, "ind");

            line.Instruction = "outd";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xab }, "outd");

            line.Instruction = "ldir";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xb0 }, "ldir");

            line.Instruction = "cpir";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xb1 }, "cpir");

            line.Instruction = "inir";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xb2 }, "inir");

            line.Instruction = "otir";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xb3 }, "otir");

            line.Instruction = "lddr";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xb8 }, "lddr");

            line.Instruction = "cpdr";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xb9 }, "cpdr");

            line.Instruction = "indr";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xba }, "indr");

            line.Instruction = "otdr";
            TestInstruction(line, 0x0002, new byte[] { 0xed, 0xbb }, "otdr");
        }

        [Test]
        public void TestZ80BitsIxy()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "bit";
            line.Operand = "0,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x40 }, "bit 0,(ix+$12),b");

            line.Operand = "0,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x41 }, "bit 0,(ix+$12),c");

            line.Operand = "0,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x42 }, "bit 0,(ix+$12),d");

            line.Operand = "0,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x43 }, "bit 0,(ix+$12),e");

            line.Operand = "0,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x44 }, "bit 0,(ix+$12),h");

            line.Operand = "0,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x45 }, "bit 0,(ix+$12),l");

            line.Operand = "0,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x46 }, "bit 0,(ix+$12)");

            line.Operand = "0,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x47 }, "bit 0,(ix+$12),a");

            line.Operand = "1,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x48 }, "bit 1,(ix+$12),b");

            line.Operand = "1,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x49 }, "bit 1,(ix+$12),c");

            line.Operand = "1,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x4a }, "bit 1,(ix+$12),d");

            line.Operand = "1,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x4b }, "bit 1,(ix+$12),e");

            line.Operand = "1,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x4c }, "bit 1,(ix+$12),h");

            line.Operand = "1,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x4d }, "bit 1,(ix+$12),l");

            line.Operand = "1,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x4e }, "bit 1,(ix+$12)");

            line.Operand = "1,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x4f }, "bit 1,(ix+$12),a");

            line.Operand = "2,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x50 }, "bit 2,(ix+$12),b");

            line.Operand = "2,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x51 }, "bit 2,(ix+$12),c");

            line.Operand = "2,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x52 }, "bit 2,(ix+$12),d");

            line.Operand = "2,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x53 }, "bit 2,(ix+$12),e");

            line.Operand = "2,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x54 }, "bit 2,(ix+$12),h");

            line.Operand = "2,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x55 }, "bit 2,(ix+$12),l");

            line.Operand = "2,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x56 }, "bit 2,(ix+$12)");

            line.Operand = "2,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x57 }, "bit 2,(ix+$12),a");

            line.Operand = "3,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x58 }, "bit 3,(ix+$12),b");

            line.Operand = "3 , ( ix + $12 ) , c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x59 }, "bit 3,(ix+$12),c");

            line.Operand = "3 , ( ix - $12 ) , d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0xee, 0x5a }, "bit 3,(ix-$12),d");

            line.Operand = "3,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x5b }, "bit 3,(ix+$12),e");

            line.Operand = "3,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x5c }, "bit 3,(ix+$12),h");

            line.Operand = "3,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x5d }, "bit 3,(ix+$12),l");

            line.Operand = "3,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x5e }, "bit 3,(ix+$12)");

            line.Operand = "3,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x5f }, "bit 3,(ix+$12),a");

            line.Operand = "4,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x60 }, "bit 4,(ix+$12),b");

            line.Operand = "4,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x61 }, "bit 4,(ix+$12),c");

            line.Operand = "4,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x62 }, "bit 4,(ix+$12),d");

            line.Operand = "4,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x63 }, "bit 4,(ix+$12),e");

            line.Operand = "4,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x64 }, "bit 4,(ix+$12),h");

            line.Operand = "4,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x65 }, "bit 4,(ix+$12),l");

            line.Operand = "4,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x66 }, "bit 4,(ix+$12)");

            line.Operand = "4,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x67 }, "bit 4,(ix+$12),a");

            line.Operand = "5,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x68 }, "bit 5,(ix+$12),b");

            line.Operand = "5,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x69 }, "bit 5,(ix+$12),c");

            line.Operand = "5,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x6a }, "bit 5,(ix+$12),d");

            line.Operand = "5,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x6b }, "bit 5,(ix+$12),e");

            line.Operand = "5,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x6c }, "bit 5,(ix+$12),h");

            line.Operand = "5,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x6d }, "bit 5,(ix+$12),l");

            line.Operand = "5,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x6e }, "bit 5,(ix+$12)");

            line.Operand = "5,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x6f }, "bit 5,(ix+$12),a");

            line.Operand = "6,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x70 }, "bit 6,(ix+$12),b");

            line.Operand = "6,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x71 }, "bit 6,(ix+$12),c");

            line.Operand = "6,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x72 }, "bit 6,(ix+$12),d");

            line.Operand = "6,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x73 }, "bit 6,(ix+$12),e");

            line.Operand = "6,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x74 }, "bit 6,(ix+$12),h");

            line.Operand = "6,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x75 }, "bit 6,(ix+$12),l");

            line.Operand = "6,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x76 }, "bit 6,(ix+$12)");

            line.Operand = "6,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x77 }, "bit 6,(ix+$12),a");

            line.Operand = "7,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x78 }, "bit 7,(ix+$12),b");

            line.Operand = "7,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x79 }, "bit 7,(ix+$12),c");

            line.Operand = "7,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x7a }, "bit 7,(ix+$12),d");

            line.Operand = "7,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x7b }, "bit 7,(ix+$12),e");

            line.Operand = "7,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x7c }, "bit 7,(ix+$12),h");

            line.Operand = "7,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x7d }, "bit 7,(ix+$12),l");

            line.Operand = "7,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x7e }, "bit 7,(ix+$12)");

            line.Operand = "7,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x7f }, "bit 7,(ix+$12),a");

            line.Instruction = "res";
            line.Operand = "0,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x80 }, "res 0,(ix+$12),b");

            line.Operand = "0,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x81 }, "res 0,(ix+$12),c");

            line.Operand = "0,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x82 }, "res 0,(ix+$12),d");

            line.Operand = "0,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x83 }, "res 0,(ix+$12),e");

            line.Operand = "0,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x84 }, "res 0,(ix+$12),h");

            line.Operand = "0,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x85 }, "res 0,(ix+$12),l");

            line.Operand = "0,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x86 }, "res 0,(ix+$12)");

            line.Operand = "0,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x87 }, "res 0,(ix+$12),a");

            line.Operand = "1,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x88 }, "res 1,(ix+$12),b");

            line.Operand = "1,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x89 }, "res 1,(ix+$12),c");

            line.Operand = "1,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x8a }, "res 1,(ix+$12),d");

            line.Operand = "1,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x8b }, "res 1,(ix+$12),e");

            line.Operand = "1,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x8c }, "res 1,(ix+$12),h");

            line.Operand = "1,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x8d }, "res 1,(ix+$12),l");

            line.Operand = "1,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x8e }, "res 1,(ix+$12)");

            line.Operand = "1,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x8f }, "res 1,(ix+$12),a");

            line.Operand = "2,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x90 }, "res 2,(ix+$12),b");

            line.Operand = "2,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x91 }, "res 2,(ix+$12),c");

            line.Operand = "2,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x92 }, "res 2,(ix+$12),d");

            line.Operand = "2,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x93 }, "res 2,(ix+$12),e");

            line.Operand = "2,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x94 }, "res 2,(ix+$12),h");

            line.Operand = "2,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x95 }, "res 2,(ix+$12),l");

            line.Operand = "2,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x96 }, "res 2,(ix+$12)");

            line.Operand = "2,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x97 }, "res 2,(ix+$12),a");

            line.Operand = "3,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x98 }, "res 3,(ix+$12),b");

            line.Operand = "3,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x99 }, "res 3,(ix+$12),c");

            line.Operand = "3,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x9a }, "res 3,(ix+$12),d");

            line.Operand = "3,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x9b }, "res 3,(ix+$12),e");

            line.Operand = "3,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x9c }, "res 3,(ix+$12),h");

            line.Operand = "3,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x9d }, "res 3,(ix+$12),l");

            line.Operand = "3,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x9e }, "res 3,(ix+$12)");

            line.Operand = "3,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x9f }, "res 3,(ix+$12),a");

            line.Operand = "4,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xa0 }, "res 4,(ix+$12),b");

            line.Operand = "4,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xa1 }, "res 4,(ix+$12),c");

            line.Operand = "4,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xa2 }, "res 4,(ix+$12),d");

            line.Operand = "4,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xa3 }, "res 4,(ix+$12),e");

            line.Operand = "4,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xa4 }, "res 4,(ix+$12),h");

            line.Operand = "4,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xa5 }, "res 4,(ix+$12),l");

            line.Operand = "4,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xa6 }, "res 4,(ix+$12)");

            line.Operand = "4,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xa7 }, "res 4,(ix+$12),a");

            line.Operand = "5,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xa8 }, "res 5,(ix+$12),b");

            line.Operand = "5,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xa9 }, "res 5,(ix+$12),c");

            line.Operand = "5,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xaa }, "res 5,(ix+$12),d");

            line.Operand = "5,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xab }, "res 5,(ix+$12),e");

            line.Operand = "5,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xac }, "res 5,(ix+$12),h");

            line.Operand = "5,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xad }, "res 5,(ix+$12),l");

            line.Operand = "5,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xae }, "res 5,(ix+$12)");

            line.Operand = "5,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xaf }, "res 5,(ix+$12),a");

            line.Operand = "6,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xb0 }, "res 6,(ix+$12),b");

            line.Operand = "6,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xb1 }, "res 6,(ix+$12),c");

            line.Operand = "6,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xb2 }, "res 6,(ix+$12),d");

            line.Operand = "6,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xb3 }, "res 6,(ix+$12),e");

            line.Operand = "6,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xb4 }, "res 6,(ix+$12),h");

            line.Operand = "6,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xb5 }, "res 6,(ix+$12),l");

            line.Operand = "6,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xb6 }, "res 6,(ix+$12)");

            line.Operand = "6,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xb7 }, "res 6,(ix+$12),a");

            line.Operand = "7,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xb8 }, "res 7,(ix+$12),b");

            line.Operand = "7,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xb9 }, "res 7,(ix+$12),c");

            line.Operand = "7,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xba }, "res 7,(ix+$12),d");

            line.Operand = "7,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xbb }, "res 7,(ix+$12),e");

            line.Operand = "7,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xbc }, "res 7,(ix+$12),h");

            line.Operand = "7,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xbd }, "res 7,(ix+$12),l");

            line.Operand = "7,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xbe }, "res 7,(ix+$12)");

            line.Operand = "7,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xbf }, "res 7,(ix+$12),a");

            line.Instruction = "set";
            line.Operand = "0,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xc0 }, "set 0,(ix+$12),b");

            line.Operand = "0,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xc1 }, "set 0,(ix+$12),c");

            line.Operand = "0,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xc2 }, "set 0,(ix+$12),d");

            line.Operand = "0,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xc3 }, "set 0,(ix+$12),e");

            line.Operand = "0,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xc4 }, "set 0,(ix+$12),h");

            line.Operand = "0,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xc5 }, "set 0,(ix+$12),l");

            line.Operand = "0,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xc6 }, "set 0,(ix+$12)");

            line.Operand = "0,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xc7 }, "set 0,(ix+$12),a");

            line.Operand = "1,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xc8 }, "set 1,(ix+$12),b");

            line.Operand = "1,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xc9 }, "set 1,(ix+$12),c");

            line.Operand = "1,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xca }, "set 1,(ix+$12),d");

            line.Operand = "1,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xcb }, "set 1,(ix+$12),e");

            line.Operand = "1,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xcc }, "set 1,(ix+$12),h");

            line.Operand = "1,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xcd }, "set 1,(ix+$12),l");

            line.Operand = "1,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xce }, "set 1,(ix+$12)");

            line.Operand = "1,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xcf }, "set 1,(ix+$12),a");

            line.Operand = "2,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xd0 }, "set 2,(ix+$12),b");

            line.Operand = "2,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xd1 }, "set 2,(ix+$12),c");

            line.Operand = "2,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xd2 }, "set 2,(ix+$12),d");

            line.Operand = "2,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xd3 }, "set 2,(ix+$12),e");

            line.Operand = "2,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xd4 }, "set 2,(ix+$12),h");

            line.Operand = "2,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xd5 }, "set 2,(ix+$12),l");

            line.Operand = "2,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xd6 }, "set 2,(ix+$12)");

            line.Operand = "2,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xd7 }, "set 2,(ix+$12),a");

            line.Operand = "3,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xd8 }, "set 3,(ix+$12),b");

            line.Operand = "3,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xd9 }, "set 3,(ix+$12),c");

            line.Operand = "3,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xda }, "set 3,(ix+$12),d");

            line.Operand = "3,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xdb }, "set 3,(ix+$12),e");

            line.Operand = "3,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xdc }, "set 3,(ix+$12),h");

            line.Operand = "3,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xdd }, "set 3,(ix+$12),l");

            line.Operand = "3,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xde }, "set 3,(ix+$12)");

            line.Operand = "3,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xdf }, "set 3,(ix+$12),a");

            line.Operand = "4,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xe0 }, "set 4,(ix+$12),b");

            line.Operand = "4,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xe1 }, "set 4,(ix+$12),c");

            line.Operand = "4,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xe2 }, "set 4,(ix+$12),d");

            line.Operand = "4,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xe3 }, "set 4,(ix+$12),e");

            line.Operand = "4,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xe4 }, "set 4,(ix+$12),h");

            line.Operand = "4,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xe5 }, "set 4,(ix+$12),l");

            line.Operand = "4,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xe6 }, "set 4,(ix+$12)");

            line.Operand = "4,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xe7 }, "set 4,(ix+$12),a");

            line.Operand = "5,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xe8 }, "set 5,(ix+$12),b");

            line.Operand = "5,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xe9 }, "set 5,(ix+$12),c");

            line.Operand = "5,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xea }, "set 5,(ix+$12),d");

            line.Operand = "5,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xeb }, "set 5,(ix+$12),e");

            line.Operand = "5,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xec }, "set 5,(ix+$12),h");

            line.Operand = "5,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xed }, "set 5,(ix+$12),l");

            line.Operand = "5,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xee }, "set 5,(ix+$12)");

            line.Operand = "5,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xef }, "set 5,(ix+$12),a");

            line.Operand = "6,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xf0 }, "set 6,(ix+$12),b");

            line.Operand = "6,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xf1 }, "set 6,(ix+$12),c");

            line.Operand = "6,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xf2 }, "set 6,(ix+$12),d");

            line.Operand = "6,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xf3 }, "set 6,(ix+$12),e");

            line.Operand = "6,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xf4 }, "set 6,(ix+$12),h");

            line.Operand = "6,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xf5 }, "set 6,(ix+$12),l");

            line.Operand = "6,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xf6 }, "set 6,(ix+$12)");

            line.Operand = "6,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xf7 }, "set 6,(ix+$12),a");

            line.Operand = "7,(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xf8 }, "set 7,(ix+$12),b");

            line.Operand = "7,(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xf9 }, "set 7,(ix+$12),c");

            line.Operand = "7,(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xfa }, "set 7,(ix+$12),d");

            line.Operand = "7,(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xfb }, "set 7,(ix+$12),e");

            line.Operand = "7,(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xfc }, "set 7,(ix+$12),h");

            line.Operand = "7,(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xfd }, "set 7,(ix+$12),l");

            line.Operand = "7,(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xfe }, "set 7,(ix+$12)");

            line.Operand = "7,(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0xff }, "set 7,(ix+$12),a");

            line.Instruction = "bit";
            line.Operand = "0,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x40 }, "bit 0,(iy+$12),b");

            line.Operand = "0,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x41 }, "bit 0,(iy+$12),c");

            line.Operand = "0,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x42 }, "bit 0,(iy+$12),d");

            line.Operand = "0,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x43 }, "bit 0,(iy+$12),e");

            line.Operand = "0,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x44 }, "bit 0,(iy+$12),h");

            line.Operand = "0,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x45 }, "bit 0,(iy+$12),l");

            line.Operand = "0,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x46 }, "bit 0,(iy+$12)");

            line.Operand = "0,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x47 }, "bit 0,(iy+$12),a");

            line.Operand = "1,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x48 }, "bit 1,(iy+$12),b");

            line.Operand = "1,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x49 }, "bit 1,(iy+$12),c");

            line.Operand = "1,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x4a }, "bit 1,(iy+$12),d");

            line.Operand = "1,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x4b }, "bit 1,(iy+$12),e");

            line.Operand = "1,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x4c }, "bit 1,(iy+$12),h");

            line.Operand = "1,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x4d }, "bit 1,(iy+$12),l");

            line.Operand = "1,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x4e }, "bit 1,(iy+$12)");

            line.Operand = "1,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x4f }, "bit 1,(iy+$12),a");

            line.Operand = "2,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x50 }, "bit 2,(iy+$12),b");

            line.Operand = "2,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x51 }, "bit 2,(iy+$12),c");

            line.Operand = "2,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x52 }, "bit 2,(iy+$12),d");

            line.Operand = "2,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x53 }, "bit 2,(iy+$12),e");

            line.Operand = "2,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x54 }, "bit 2,(iy+$12),h");

            line.Operand = "2,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x55 }, "bit 2,(iy+$12),l");

            line.Operand = "2,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x56 }, "bit 2,(iy+$12)");

            line.Operand = "2,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x57 }, "bit 2,(iy+$12),a");

            line.Operand = "3,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x58 }, "bit 3,(iy+$12),b");

            line.Operand = "3,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x59 }, "bit 3,(iy+$12),c");

            line.Operand = "3,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x5a }, "bit 3,(iy+$12),d");

            line.Operand = "3,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x5b }, "bit 3,(iy+$12),e");

            line.Operand = "3,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x5c }, "bit 3,(iy+$12),h");

            line.Operand = "3,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x5d }, "bit 3,(iy+$12),l");

            line.Operand = "3,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x5e }, "bit 3,(iy+$12)");

            line.Operand = "3,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x5f }, "bit 3,(iy+$12),a");

            line.Operand = "4,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x60 }, "bit 4,(iy+$12),b");

            line.Operand = "4,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x61 }, "bit 4,(iy+$12),c");

            line.Operand = "4,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x62 }, "bit 4,(iy+$12),d");

            line.Operand = "4,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x63 }, "bit 4,(iy+$12),e");

            line.Operand = "4,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x64 }, "bit 4,(iy+$12),h");

            line.Operand = "4,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x65 }, "bit 4,(iy+$12),l");

            line.Operand = "4,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x66 }, "bit 4,(iy+$12)");

            line.Operand = "4,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x67 }, "bit 4,(iy+$12),a");

            line.Operand = "5,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x68 }, "bit 5,(iy+$12),b");

            line.Operand = "5,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x69 }, "bit 5,(iy+$12),c");

            line.Operand = "5,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x6a }, "bit 5,(iy+$12),d");

            line.Operand = "5,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x6b }, "bit 5,(iy+$12),e");

            line.Operand = "5,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x6c }, "bit 5,(iy+$12),h");

            line.Operand = "5,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x6d }, "bit 5,(iy+$12),l");

            line.Operand = "5,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x6e }, "bit 5,(iy+$12)");

            line.Operand = "5,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x6f }, "bit 5,(iy+$12),a");

            line.Operand = "6,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x70 }, "bit 6,(iy+$12),b");

            line.Operand = "6,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x71 }, "bit 6,(iy+$12),c");

            line.Operand = "6,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x72 }, "bit 6,(iy+$12),d");

            line.Operand = "6,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x73 }, "bit 6,(iy+$12),e");

            line.Operand = "6,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x74 }, "bit 6,(iy+$12),h");

            line.Operand = "6,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x75 }, "bit 6,(iy+$12),l");

            line.Operand = "6,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x76 }, "bit 6,(iy+$12)");

            line.Operand = "6,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x77 }, "bit 6,(iy+$12),a");

            line.Operand = "7,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x78 }, "bit 7,(iy+$12),b");

            line.Operand = "7,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x79 }, "bit 7,(iy+$12),c");

            line.Operand = "7,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x7a }, "bit 7,(iy+$12),d");

            line.Operand = "7,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x7b }, "bit 7,(iy+$12),e");

            line.Operand = "7,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x7c }, "bit 7,(iy+$12),h");

            line.Operand = "7,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x7d }, "bit 7,(iy+$12),l");

            line.Operand = "7,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x7e }, "bit 7,(iy+$12)");

            line.Operand = "7,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x7f }, "bit 7,(iy+$12),a");

            line.Instruction = "res";
            line.Operand = "0,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x80 }, "res 0,(iy+$12),b");

            line.Operand = "0,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x81 }, "res 0,(iy+$12),c");

            line.Operand = "0,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x82 }, "res 0,(iy+$12),d");

            line.Operand = "0,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x83 }, "res 0,(iy+$12),e");

            line.Operand = "0,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x84 }, "res 0,(iy+$12),h");

            line.Operand = "0,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x85 }, "res 0,(iy+$12),l");

            line.Operand = "0,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x86 }, "res 0,(iy+$12)");

            line.Operand = "0,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x87 }, "res 0,(iy+$12),a");

            line.Operand = "1,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x88 }, "res 1,(iy+$12),b");

            line.Operand = "1,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x89 }, "res 1,(iy+$12),c");

            line.Operand = "1,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x8a }, "res 1,(iy+$12),d");

            line.Operand = "1,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x8b }, "res 1,(iy+$12),e");

            line.Operand = "1,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x8c }, "res 1,(iy+$12),h");

            line.Operand = "1,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x8d }, "res 1,(iy+$12),l");

            line.Operand = "1,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x8e }, "res 1,(iy+$12)");

            line.Operand = "1,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x8f }, "res 1,(iy+$12),a");

            line.Operand = "2,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x90 }, "res 2,(iy+$12),b");

            line.Operand = "2,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x91 }, "res 2,(iy+$12),c");

            line.Operand = "2,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x92 }, "res 2,(iy+$12),d");

            line.Operand = "2,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x93 }, "res 2,(iy+$12),e");

            line.Operand = "2,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x94 }, "res 2,(iy+$12),h");

            line.Operand = "2,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x95 }, "res 2,(iy+$12),l");

            line.Operand = "2,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x96 }, "res 2,(iy+$12)");

            line.Operand = "2,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x97 }, "res 2,(iy+$12),a");

            line.Operand = "3,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x98 }, "res 3,(iy+$12),b");

            line.Operand = "3,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x99 }, "res 3,(iy+$12),c");

            line.Operand = "3,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x9a }, "res 3,(iy+$12),d");

            line.Operand = "3,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x9b }, "res 3,(iy+$12),e");

            line.Operand = "3,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x9c }, "res 3,(iy+$12),h");

            line.Operand = "3,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x9d }, "res 3,(iy+$12),l");

            line.Operand = "3,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x9e }, "res 3,(iy+$12)");

            line.Operand = "3,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x9f }, "res 3,(iy+$12),a");

            line.Operand = "4,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xa0 }, "res 4,(iy+$12),b");

            line.Operand = "4,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xa1 }, "res 4,(iy+$12),c");

            line.Operand = "4,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xa2 }, "res 4,(iy+$12),d");

            line.Operand = "4,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xa3 }, "res 4,(iy+$12),e");

            line.Operand = "4,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xa4 }, "res 4,(iy+$12),h");

            line.Operand = "4,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xa5 }, "res 4,(iy+$12),l");

            line.Operand = "4,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xa6 }, "res 4,(iy+$12)");

            line.Operand = "4,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xa7 }, "res 4,(iy+$12),a");

            line.Operand = "5,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xa8 }, "res 5,(iy+$12),b");

            line.Operand = "5,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xa9 }, "res 5,(iy+$12),c");

            line.Operand = "5,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xaa }, "res 5,(iy+$12),d");

            line.Operand = "5,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xab }, "res 5,(iy+$12),e");

            line.Operand = "5,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xac }, "res 5,(iy+$12),h");

            line.Operand = "5,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xad }, "res 5,(iy+$12),l");

            line.Operand = "5,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xae }, "res 5,(iy+$12)");

            line.Operand = "5,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xaf }, "res 5,(iy+$12),a");

            line.Operand = "6,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xb0 }, "res 6,(iy+$12),b");

            line.Operand = "6,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xb1 }, "res 6,(iy+$12),c");

            line.Operand = "6,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xb2 }, "res 6,(iy+$12),d");

            line.Operand = "6,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xb3 }, "res 6,(iy+$12),e");

            line.Operand = "6,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xb4 }, "res 6,(iy+$12),h");

            line.Operand = "6,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xb5 }, "res 6,(iy+$12),l");

            line.Operand = "6,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xb6 }, "res 6,(iy+$12)");

            line.Operand = "6,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xb7 }, "res 6,(iy+$12),a");

            line.Operand = "7,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xb8 }, "res 7,(iy+$12),b");

            line.Operand = "7,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xb9 }, "res 7,(iy+$12),c");

            line.Operand = "7,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xba }, "res 7,(iy+$12),d");

            line.Operand = "7,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xbb }, "res 7,(iy+$12),e");

            line.Operand = "7,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xbc }, "res 7,(iy+$12),h");

            line.Operand = "7,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xbd }, "res 7,(iy+$12),l");

            line.Operand = "7,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xbe }, "res 7,(iy+$12)");

            line.Operand = "7,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xbf }, "res 7,(iy+$12),a");

            line.Instruction = "set";
            line.Operand = "0,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xc0 }, "set 0,(iy+$12),b");

            line.Operand = "0,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xc1 }, "set 0,(iy+$12),c");

            line.Operand = "0,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xc2 }, "set 0,(iy+$12),d");

            line.Operand = "0,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xc3 }, "set 0,(iy+$12),e");

            line.Operand = "0,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xc4 }, "set 0,(iy+$12),h");

            line.Operand = "0,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xc5 }, "set 0,(iy+$12),l");

            line.Operand = "0,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xc6 }, "set 0,(iy+$12)");

            line.Operand = "0,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xc7 }, "set 0,(iy+$12),a");

            line.Operand = "1,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xc8 }, "set 1,(iy+$12),b");

            line.Operand = "1,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xc9 }, "set 1,(iy+$12),c");

            line.Operand = "1,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xca }, "set 1,(iy+$12),d");

            line.Operand = "1,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xcb }, "set 1,(iy+$12),e");

            line.Operand = "1,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xcc }, "set 1,(iy+$12),h");

            line.Operand = "1,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xcd }, "set 1,(iy+$12),l");

            line.Operand = "1,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xce }, "set 1,(iy+$12)");

            line.Operand = "1,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xcf }, "set 1,(iy+$12),a");

            line.Operand = "2,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xd0 }, "set 2,(iy+$12),b");

            line.Operand = "2,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xd1 }, "set 2,(iy+$12),c");

            line.Operand = "2,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xd2 }, "set 2,(iy+$12),d");

            line.Operand = "2,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xd3 }, "set 2,(iy+$12),e");

            line.Operand = "2,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xd4 }, "set 2,(iy+$12),h");

            line.Operand = "2,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xd5 }, "set 2,(iy+$12),l");

            line.Operand = "2,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xd6 }, "set 2,(iy+$12)");

            line.Operand = "2,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xd7 }, "set 2,(iy+$12),a");

            line.Operand = "3,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xd8 }, "set 3,(iy+$12),b");

            line.Operand = "3,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xd9 }, "set 3,(iy+$12),c");

            line.Operand = "3,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xda }, "set 3,(iy+$12),d");

            line.Operand = "3,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xdb }, "set 3,(iy+$12),e");

            line.Operand = "3,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xdc }, "set 3,(iy+$12),h");

            line.Operand = "3,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xdd }, "set 3,(iy+$12),l");

            line.Operand = "3,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xde }, "set 3,(iy+$12)");

            line.Operand = "3,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xdf }, "set 3,(iy+$12),a");

            line.Operand = "4,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xe0 }, "set 4,(iy+$12),b");

            line.Operand = "4,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xe1 }, "set 4,(iy+$12),c");

            line.Operand = "4,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xe2 }, "set 4,(iy+$12),d");

            line.Operand = "4,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xe3 }, "set 4,(iy+$12),e");

            line.Operand = "4,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xe4 }, "set 4,(iy+$12),h");

            line.Operand = "4,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xe5 }, "set 4,(iy+$12),l");

            line.Operand = "4,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xe6 }, "set 4,(iy+$12)");

            line.Operand = "4,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xe7 }, "set 4,(iy+$12),a");

            line.Operand = "5,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xe8 }, "set 5,(iy+$12),b");

            line.Operand = "5,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xe9 }, "set 5,(iy+$12),c");

            line.Operand = "5,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xea }, "set 5,(iy+$12),d");

            line.Operand = "5,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xeb }, "set 5,(iy+$12),e");

            line.Operand = "5,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xec }, "set 5,(iy+$12),h");

            line.Operand = "5,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xed }, "set 5,(iy+$12),l");

            line.Operand = "5,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xee }, "set 5,(iy+$12)");

            line.Operand = "5,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xef }, "set 5,(iy+$12),a");

            line.Operand = "6,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xf0 }, "set 6,(iy+$12),b");

            line.Operand = "6,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xf1 }, "set 6,(iy+$12),c");

            line.Operand = "6,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xf2 }, "set 6,(iy+$12),d");

            line.Operand = "6,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xf3 }, "set 6,(iy+$12),e");

            line.Operand = "6,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xf4 }, "set 6,(iy+$12),h");

            line.Operand = "6,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xf5 }, "set 6,(iy+$12),l");

            line.Operand = "6,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xf6 }, "set 6,(iy+$12)");

            line.Operand = "6,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xf7 }, "set 6,(iy+$12),a");

            line.Operand = "7,(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xf8 }, "set 7,(iy+$12),b");

            line.Operand = "7,(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xf9 }, "set 7,(iy+$12),c");

            line.Operand = "7,(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xfa }, "set 7,(iy+$12),d");

            line.Operand = "7,(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xfb }, "set 7,(iy+$12),e");

            line.Operand = "7,(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xfc }, "set 7,(iy+$12),h");

            line.Operand = "7,(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xfd }, "set 7,(iy+$12),l");

            line.Operand = "7,(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xfe }, "set 7,(iy+$12)");

            line.Operand = "7,(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0xff }, "set 7,(iy+$12),a");
        }

        [Test]
        public void TestZ80Shifts()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "rlca";
            line.Operand = string.Empty;
            TestInstruction(line, 0x0001, new byte[] { 0x07 }, "rlca");

            line.Instruction = "rrca";
            TestInstruction(line, 0x0001, new byte[] { 0x0f }, "rrca");

            line.Instruction = "rla";
            TestInstruction(line, 0x0001, new byte[] { 0x17 }, "rla");

            line.Instruction = "rra";
            TestInstruction(line, 0x0001, new byte[] { 0x1f }, "rra");

            line.Instruction = "rlc";
            line.Operand = "b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x00 }, "rlc b");

            line.Operand = "c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x01 }, "rlc c");

            line.Operand = "d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x02 }, "rlc d");

            line.Operand = "e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x03 }, "rlc e");

            line.Operand = "h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x04 }, "rlc h");

            line.Operand = "l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x05 }, "rlc l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x06 }, "rlc (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x07 }, "rlc a");

            line.Instruction = "rrc";
            line.Operand = "b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x08 }, "rrc b");

            line.Operand = "c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x09 }, "rrc c");

            line.Operand = "d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x0a }, "rrc d");

            line.Operand = "e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x0b }, "rrc e");

            line.Operand = "h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x0c }, "rrc h");

            line.Operand = "l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x0d }, "rrc l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x0e }, "rrc (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x0f }, "rrc a");

            line.Instruction = "rl";
            line.Operand = "b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x10 }, "rl b");

            line.Operand = "c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x11 }, "rl c");

            line.Operand = "d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x12 }, "rl d");

            line.Operand = "e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x13 }, "rl e");

            line.Operand = "h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x14 }, "rl h");

            line.Operand = "l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x15 }, "rl l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x16 }, "rl (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x17 }, "rl a");

            line.Instruction = "rr";
            line.Operand = "b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x18 }, "rr b");

            line.Operand = "c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x19 }, "rr c");

            line.Operand = "d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x1a }, "rr d");

            line.Operand = "e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x1b }, "rr e");

            line.Operand = "h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x1c }, "rr h");

            line.Operand = "l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x1d }, "rr l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x1e }, "rr (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x1f }, "rr a");

            line.Instruction = "sla";
            line.Operand = "b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x20 }, "sla b");

            line.Operand = "c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x21 }, "sla c");

            line.Operand = "d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x22 }, "sla d");

            line.Operand = "e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x23 }, "sla e");

            line.Operand = "h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x24 }, "sla h");

            line.Operand = "l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x25 }, "sla l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x26 }, "sla (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x27 }, "sla a");

            line.Instruction = "sra";
            line.Operand = "b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x28 }, "sra b");

            line.Operand = "c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x29 }, "sra c");

            line.Operand = "d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x2a }, "sra d");

            line.Operand = "e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x2b }, "sra e");

            line.Operand = "h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x2c }, "sra h");

            line.Operand = "l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x2d }, "sra l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x2e }, "sra (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x2f }, "sra a");

            line.Instruction = "sll";
            line.Operand = "b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x30 }, "sll b");

            line.Operand = "c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x31 }, "sll c");

            line.Operand = "d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x32 }, "sll d");

            line.Operand = "e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x33 }, "sll e");

            line.Operand = "h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x34 }, "sll h");

            line.Operand = "l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x35 }, "sll l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x36 }, "sll (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x37 }, "sll a");

            line.Instruction = "srl";
            line.Operand = "b";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x38 }, "srl b");

            line.Operand = "c";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x39 }, "srl c");

            line.Operand = "d";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x3a }, "srl d");

            line.Operand = "e";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x3b }, "srl e");

            line.Operand = "h";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x3c }, "srl h");

            line.Operand = "l";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x3d }, "srl l");

            line.Operand = "(hl)";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x3e }, "srl (hl)");

            line.Operand = "a";
            TestInstruction(line, 0x0002, new byte[] { 0xcb, 0x3f }, "srl a");
        }

        [Test]
        public void TextZ80ShiftsIxy()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "rlc";
            line.Operand = "(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x00 }, "rlc (ix+$12),b");

            line.Operand = "(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x01 }, "rlc (ix+$12),c");

            line.Operand = "(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x02 }, "rlc (ix+$12),d");

            line.Operand = "(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x03 }, "rlc (ix+$12),e");

            line.Operand = "(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x04 }, "rlc (ix+$12),h");

            line.Operand = "(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x05 }, "rlc (ix+$12),l");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x06 }, "rlc (ix+$12)");

            line.Operand = "(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x07 }, "rlc (ix+$12),a");

            line.Instruction = "rrc";
            line.Operand = "(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x08 }, "rrc (ix+$12),b");

            line.Operand = "(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x09 }, "rrc (ix+$12),c");

            line.Operand = "(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x0a }, "rrc (ix+$12),d");

            line.Operand = "(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x0b }, "rrc (ix+$12),e");

            line.Operand = "(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x0c }, "rrc (ix+$12),h");

            line.Operand = "(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x0d }, "rrc (ix+$12),l");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x0e }, "rrc (ix+$12)");

            line.Operand = "(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x0f }, "rrc (ix+$12),a");

            line.Instruction = "rl";
            line.Operand = "(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x10 }, "rl (ix+$12),b");

            line.Operand = "(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x11 }, "rl (ix+$12),c");

            line.Operand = "(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x12 }, "rl (ix+$12),d");

            line.Operand = "(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x13 }, "rl (ix+$12),e");

            line.Operand = "(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x14 }, "rl (ix+$12),h");

            line.Operand = "(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x15 }, "rl (ix+$12),l");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x16 }, "rl (ix+$12)");

            line.Operand = "(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x17 }, "rl (ix+$12),a");

            line.Instruction = "rr";
            line.Operand = "(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x18 }, "rr (ix+$12),b");

            line.Operand = "(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x19 }, "rr (ix+$12),c");

            line.Operand = "(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x1a }, "rr (ix+$12),d");

            line.Operand = "(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x1b }, "rr (ix+$12),e");

            line.Operand = "(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x1c }, "rr (ix+$12),h");

            line.Operand = "(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x1d }, "rr (ix+$12),l");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x1e }, "rr (ix+$12)");

            line.Operand = "(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x1f }, "rr (ix+$12),a");

            line.Instruction = "sla";
            line.Operand = "(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x20 }, "sla (ix+$12),b");

            line.Operand = "(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x21 }, "sla (ix+$12),c");

            line.Operand = "(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x22 }, "sla (ix+$12),d");

            line.Operand = "(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x23 }, "sla (ix+$12),e");

            line.Operand = "(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x24 }, "sla (ix+$12),h");

            line.Operand = "(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x25 }, "sla (ix+$12),l");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x26 }, "sla (ix+$12)");

            line.Operand = "(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x27 }, "sla (ix+$12),a");

            line.Instruction = "sra";
            line.Operand = "(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x28 }, "sra (ix+$12),b");

            line.Operand = "(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x29 }, "sra (ix+$12),c");

            line.Operand = "(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x2a }, "sra (ix+$12),d");

            line.Operand = "(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x2b }, "sra (ix+$12),e");

            line.Operand = "(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x2c }, "sra (ix+$12),h");

            line.Operand = "(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x2d }, "sra (ix+$12),l");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x2e }, "sra (ix+$12)");

            line.Operand = "(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x2f }, "sra (ix+$12),a");

            line.Instruction = "sll";
            line.Operand = "(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x30 }, "sll (ix+$12),b");

            line.Operand = "(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x31 }, "sll (ix+$12),c");

            line.Operand = "(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x32 }, "sll (ix+$12),d");

            line.Operand = "(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x33 }, "sll (ix+$12),e");

            line.Operand = "(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x34 }, "sll (ix+$12),h");

            line.Operand = "(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x35 }, "sll (ix+$12),l");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x36 }, "sll (ix+$12)");

            line.Operand = "(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x37 }, "sll (ix+$12),a");

            line.Instruction = "srl";
            line.Operand = "(ix+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x38 }, "srl (ix+$12),b");

            line.Operand = "(ix+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x39 }, "srl (ix+$12),c");

            line.Operand = "(ix+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x3a }, "srl (ix+$12),d");

            line.Operand = "(ix+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x3b }, "srl (ix+$12),e");

            line.Operand = "(ix+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x3c }, "srl (ix+$12),h");

            line.Operand = "(ix+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x3d }, "srl (ix+$12),l");

            line.Operand = "(ix+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x3e }, "srl (ix+$12)");

            line.Operand = "(ix+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0xcb, 0x12, 0x3f }, "srl (ix+$12),a");

            line.Instruction = "rlc";
            line.Operand = "(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x00 }, "rlc (iy+$12),b");

            line.Operand = "(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x01 }, "rlc (iy+$12),c");

            line.Operand = "(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x02 }, "rlc (iy+$12),d");

            line.Operand = "(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x03 }, "rlc (iy+$12),e");

            line.Operand = "(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x04 }, "rlc (iy+$12),h");

            line.Operand = "(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x05 }, "rlc (iy+$12),l");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x06 }, "rlc (iy+$12)");

            line.Operand = "(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x07 }, "rlc (iy+$12),a");

            line.Instruction = "rrc";
            line.Operand = "(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x08 }, "rrc (iy+$12),b");

            line.Operand = "(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x09 }, "rrc (iy+$12),c");

            line.Operand = "(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x0a }, "rrc (iy+$12),d");

            line.Operand = "(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x0b }, "rrc (iy+$12),e");

            line.Operand = "(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x0c }, "rrc (iy+$12),h");

            line.Operand = "(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x0d }, "rrc (iy+$12),l");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x0e }, "rrc (iy+$12)");

            line.Operand = "(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x0f }, "rrc (iy+$12),a");

            line.Instruction = "rl";
            line.Operand = "(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x10 }, "rl (iy+$12),b");

            line.Operand = "(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x11 }, "rl (iy+$12),c");

            line.Operand = "(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x12 }, "rl (iy+$12),d");

            line.Operand = "(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x13 }, "rl (iy+$12),e");

            line.Operand = "(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x14 }, "rl (iy+$12),h");

            line.Operand = "(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x15 }, "rl (iy+$12),l");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x16 }, "rl (iy+$12)");

            line.Operand = "(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x17 }, "rl (iy+$12),a");

            line.Instruction = "rr";
            line.Operand = "(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x18 }, "rr (iy+$12),b");

            line.Operand = "(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x19 }, "rr (iy+$12),c");

            line.Operand = "(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x1a }, "rr (iy+$12),d");

            line.Operand = "(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x1b }, "rr (iy+$12),e");

            line.Operand = "(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x1c }, "rr (iy+$12),h");

            line.Operand = "(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x1d }, "rr (iy+$12),l");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x1e }, "rr (iy+$12)");

            line.Operand = "(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x1f }, "rr (iy+$12),a");

            line.Instruction = "sla";
            line.Operand = "(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x20 }, "sla (iy+$12),b");

            line.Operand = "(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x21 }, "sla (iy+$12),c");

            line.Operand = "(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x22 }, "sla (iy+$12),d");

            line.Operand = "(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x23 }, "sla (iy+$12),e");

            line.Operand = "(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x24 }, "sla (iy+$12),h");

            line.Operand = "(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x25 }, "sla (iy+$12),l");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x26 }, "sla (iy+$12)");

            line.Operand = "(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x27 }, "sla (iy+$12),a");

            line.Instruction = "sra";
            line.Operand = "(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x28 }, "sra (iy+$12),b");

            line.Operand = "(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x29 }, "sra (iy+$12),c");

            line.Operand = "(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x2a }, "sra (iy+$12),d");

            line.Operand = "(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x2b }, "sra (iy+$12),e");

            line.Operand = "(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x2c }, "sra (iy+$12),h");

            line.Operand = "(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x2d }, "sra (iy+$12),l");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x2e }, "sra (iy+$12)");

            line.Operand = "(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x2f }, "sra (iy+$12),a");

            line.Instruction = "sll";
            line.Operand = "(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x30 }, "sll (iy+$12),b");

            line.Operand = "(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x31 }, "sll (iy+$12),c");

            line.Operand = "(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x32 }, "sll (iy+$12),d");

            line.Operand = "(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x33 }, "sll (iy+$12),e");

            line.Operand = "(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x34 }, "sll (iy+$12),h");

            line.Operand = "(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x35 }, "sll (iy+$12),l");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x36 }, "sll (iy+$12)");

            line.Operand = "(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x37 }, "sll (iy+$12),a");

            line.Instruction = "srl";
            line.Operand = "(iy+$12),b";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x38 }, "srl (iy+$12),b");

            line.Operand = "(iy+$12),c";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x39 }, "srl (iy+$12),c");

            line.Operand = "(iy+$12),d";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x3a }, "srl (iy+$12),d");

            line.Operand = "(iy+$12),e";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x3b }, "srl (iy+$12),e");

            line.Operand = "(iy+$12),h";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x3c }, "srl (iy+$12),h");

            line.Operand = "(iy+$12),l";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x3d }, "srl (iy+$12),l");

            line.Operand = "(iy+$12)";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x3e }, "srl (iy+$12)");

            line.Operand = "(iy+$12),a";
            TestInstruction(line, 0x0004, new byte[] { 0xfd, 0xcb, 0x12, 0x3f }, "srl (iy+$12),a");
        }

        [Test]
        public void TestSyntaxErrors()
        {
            SourceLine line = new SourceLine();
            line.Instruction = "sub";
            line.Operand = "hl,de";
            TestForFailure(line);

            line.Instruction = "ld";
            line.Operand = "(ix+','),b";
            TestInstruction(line, 0x0003, new byte[] { 0xdd, 0x70, 0x2c }, "ld (ix+$2c),b");

            line.Operand = "(ix+($30-2)), $43";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0x36, 0x2e, 0x43 }, "ld (ix+$2e),$43");

            line.Operand = "(ix + $43),($43-1)";
            TestForFailure(line);

            line.Operand = "(ix+129),a";
            TestForFailure<OverflowException>(line);

            line.Operand = "(ix-129),b";
            TestForFailure<OverflowException>(line);

            line.Operand = "(ix+127),-2";
            TestInstruction(line, 0x0004, new byte[] { 0xdd, 0x36, 0x7f, 0xfe }, "ld (ix+$7f),$fe");

            line.Operand = "($10000),a";
            TestForFailure<OverflowException>(line);

            line.Operand = "a,-129";
            TestForFailure<OverflowException>(line);
        }
    }
}