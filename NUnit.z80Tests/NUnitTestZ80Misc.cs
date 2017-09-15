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
    [TestFixture]
    public class NUnitTestZ80Misc
    {
        [Test]
        public void TestFormatBuilder()
        {
            FormatBuilder builder =
                new FormatBuilder(@"^\(\s*(hl)\s*\)\s*,\s*(.+)$()()",
                                          "({0}),{2}{1}",
                                          "${0:x2}",
                                          string.Empty,
                                          1,
                                          3,
                                          2,
                                          4, false);

            OperandFormat fmt = builder.GetFormat("( hl ) , $00");

            Assert.IsNotNull(fmt);
            Assert.AreEqual("(hl),${0:x2}", fmt.FormatString);
            Assert.AreEqual("$00", fmt.Expression1);
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression2));

            builder = new FormatBuilder(@"^\(\s*i(x|y)\s*\+(.+)\)(\s*,\s*([a-ehl]))?$",
                                                "(i{0}+{2}){1}",
                                                "${0:x2}",
                                                string.Empty,
                                                1,
                                                3,
                                                2,
                                                5, false);
            fmt = builder.GetFormat("(ix+$30),a");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("(ix+${0:x2}),a", fmt.FormatString);
            Assert.AreEqual("$30", fmt.Expression1);

            fmt = builder.GetFormat("(ix+$50)");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("(ix+${0:x2})", fmt.FormatString);
            Assert.AreEqual("$50", fmt.Expression1);

            builder = new FormatBuilder(@"^(([a-ehl])\s*,\s*)?\(\s*(bc|de|hl|ix|iy)\s*\)$()", "{0}", string.Empty, string.Empty, 0, 2, 4, 4, false);

            fmt = builder.GetFormat("(hl)");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("(hl)", fmt.FormatString);
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression1));
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression2));

            fmt = builder.GetFormat("a , ( de )");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("a,(de)", fmt.FormatString);
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression1));
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression2));

            builder = new FormatBuilder(@"^(bc|de|hl|ix|iy|sp)\s*,\s*(.+)$()", "{0},{2}", "${0:x4}", string.Empty, 1, 3, 2, 3, false, true);
            
            fmt = builder.GetFormat("hl,$0000");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("hl,${0:x4}", fmt.FormatString);
            Assert.AreEqual("$0000", fmt.Expression1);
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression2));

            fmt = builder.GetFormat("hl , ( $0000 )");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("hl,(${0:x4})", fmt.FormatString);
            Assert.AreEqual("( $0000 )", fmt.Expression1);
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression2));

            builder = new FormatBuilder(@"^(.+)\s*,\s*y$()", "{2},y", "${0:x4}", string.Empty, 2, 2, 1, 2, false, true);

            fmt = builder.GetFormat("$0000 , y");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("${0:x4},y", fmt.FormatString);
            Assert.AreEqual("$0000", fmt.Expression1.Trim());
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression2));

            fmt = builder.GetFormat("(  ZP_VAR  ),y");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("(${0:x4}),y", fmt.FormatString);
            Assert.AreEqual("(  ZP_VAR  )", fmt.Expression1);
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression2));
        }

        [Test]
        public void TestFormatBuilderAdvanced()
        {
            TestController controller = new TestController(null);

            FormatBuilder builder = new FormatBuilder(@"^\(\s*(c)\s*\)\s*,\s*(.+)$()", "({0}),{3}", string.Empty, "{0}", 1, 3, 3, 2, controller.Options.CaseSensitive, controller.Evaluator);

            OperandFormat fmt = builder.GetFormat("(c),0");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("(c),0", fmt.FormatString);
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression1));
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression2));

            fmt = builder.GetFormat("( c  ), ( 60 - 60)");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("(c),0", fmt.FormatString);
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression1));
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression2));

            builder = new FormatBuilder(@"^(.+)\s*,\s*\(\s*i(x|y)\s*((\+|-).+)\)(\s*,\s*[a-ehl])?$()", "{3},(i{0}+{2}){1}", "${0:x2}", "{0}", 2, 5, 3, 1, controller.Options.CaseSensitive, controller.Evaluator);

            fmt = builder.GetFormat("0,(ix+$30),a");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("0,(ix+${0:x2}),a", fmt.FormatString);
            Assert.AreEqual("+$30", fmt.Expression1);
            Assert.IsTrue(string.IsNullOrEmpty(fmt.Expression2));

            builder = new FormatBuilder(@"^\(\s*i(x|y)\s*\+(.+)\)\s*,\s*(.+)$()", "(i{0}+{2}),{3}", "${0:x2}", "${1:x2}", 1, 4, 2, 3, controller.Options.CaseSensitive);

            fmt = builder.GetFormat("(ix+$00),$ff");
            Assert.IsNotNull(fmt);
            Assert.AreEqual("(ix+${0:x2}),${1:x2}", fmt.FormatString);
            Assert.AreEqual("$00", fmt.Expression1);
            Assert.AreEqual("$ff", fmt.Expression2);
        }
    }
}
 