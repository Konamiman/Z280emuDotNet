using NUnit.Framework;

namespace Konamiman.Z280dotNet.Tests
{
    public class BasicTests : TestBase
    {
        [Test]
        public void RegistersSetInCodeCanBeReadInStatus()
        {
            var lastAddress = AssembleAndLoad(@"
                ld hl,7788h
                ldctl usp,hl

                ld a,55h
                ld r,a
                ld a,66h
                ld i,a

                push 1234h
                pop af
                ld bc,5678h
                ld de,9ABCh
                ld hl,0DEF0h
                ld ix,1122h
                ld iy,3344h

                ex af,af
                exx
                push 99AAh
                pop af
                ld bc,0BBCCh
                ld de,0DDEEh
                ld hl,0FF34h
                exx
                ex af,af

                ret");
            Run();

            Assert.That(z280.AF, Is.EqualTo(0x1234));
            Assert.That(z280.BC, Is.EqualTo(0x5678));
            Assert.That(z280.DE, Is.EqualTo(0x9ABC));
            Assert.That(z280.HL, Is.EqualTo(0xDEF0));
            Assert.That(z280.IX, Is.EqualTo(0x1122));
            Assert.That(z280.IY, Is.EqualTo(0x3344));

            Assert.That(z280.AltAF, Is.EqualTo(0x99AA));
            Assert.That(z280.AltBC, Is.EqualTo(0xBBCC));
            Assert.That(z280.AltDE, Is.EqualTo(0xDDEE));
            Assert.That(z280.AltHL, Is.EqualTo(0xFF34));

            Assert.That(z280.R, Is.EqualTo(0x55));
            Assert.That(z280.I, Is.EqualTo(0x66));

            Assert.That(z280.PC, Is.EqualTo(lastAddress));
            Assert.That(z280.SSP, Is.EqualTo(DEFAULT_INITIAL_SSP + 2));
            Assert.That(z280.USP, Is.EqualTo(0x7788));

            Assert.That(z280.A, Is.EqualTo(0x12));
            Assert.That(z280.F, Is.EqualTo(0x34));
            Assert.That(z280.B, Is.EqualTo(0x56));
            Assert.That(z280.C, Is.EqualTo(0x78));
            Assert.That(z280.D, Is.EqualTo(0x9A));
            Assert.That(z280.E, Is.EqualTo(0xBC));
            Assert.That(z280.H, Is.EqualTo(0xDE));
            Assert.That(z280.L, Is.EqualTo(0xF0));
            Assert.That(z280.IXH, Is.EqualTo(0x11));
            Assert.That(z280.IXL, Is.EqualTo(0x22));
            Assert.That(z280.IYH, Is.EqualTo(0x33));
            Assert.That(z280.IYL, Is.EqualTo(0x44));
            Assert.That(z280.AltA, Is.EqualTo(0x99));
            Assert.That(z280.AltF, Is.EqualTo(0xAA));
            Assert.That(z280.AltB, Is.EqualTo(0xBB));
            Assert.That(z280.AltC, Is.EqualTo(0xCC));
            Assert.That(z280.AltD, Is.EqualTo(0xDD));
            Assert.That(z280.AltE, Is.EqualTo(0xEE));
            Assert.That(z280.AltH, Is.EqualTo(0xFF));
            Assert.That(z280.AltL, Is.EqualTo(0x34));
        }

        [Test]
        public void InterruptModeSetInCodeCanBeReadInStatus()
        {
            z280.Reset();
            Assert.That(z280.IntMode, Is.EqualTo(0));

            AssembleAndRun("im 1 | ret");
            Assert.That(z280.IntMode, Is.EqualTo(1));

            AssembleAndRun("im 2 | ret");
            Assert.That(z280.IntMode, Is.EqualTo(2));

            AssembleAndRun("im 3 | ret");
            Assert.That(z280.IntMode, Is.EqualTo(3));

            AssembleAndRun("im 0 | ret");
            Assert.That(z280.IntMode, Is.EqualTo(0));
        }

        [Test]
        public void AltRegsStateSetInCodeCanBeCheckedInStatus()
        {
            z280.Reset();
            Assert.That(z280.AltRegsInUse, Is.False);
            Assert.That(z280.AltAFInUse, Is.False);

            AssembleAndRun("ex af,af | ret", reset: false);
            Assert.That(z280.AltRegsInUse, Is.False);
            Assert.That(z280.AltAFInUse, Is.True);

            AssembleAndRun("exx | ret", reset: false);
            Assert.That(z280.AltRegsInUse, Is.True);
            Assert.That(z280.AltAFInUse, Is.True);

            AssembleAndRun("ex af,af | exx | ret", reset: false);
            Assert.That(z280.AltRegsInUse, Is.False);
            Assert.That(z280.AltAFInUse, Is.False);
        }

        [Test]
        public void TestJarAndJaf()
        {
            AssembleAndRun(@"
                ld b,1
                jaf JUMP_1
                ld b,2
            JUMP_1:

                ex af,af

                ld c,1
                jaf JUMP_2
                ld c,2
            JUMP_2:

                ld ix,1
                jar JUMP_3
                ld ix,2
            JUMP_3:

                exx

                ld iy,1
                jar JUMP_4
                ld iy,2
            JUMP_4:

                exx
                ret");

            Assert.That(z280.B, Is.EqualTo(2));
            Assert.That(z280.C, Is.EqualTo(1));
            Assert.That(z280.IX, Is.EqualTo(2));
            Assert.That(z280.IY, Is.EqualTo(1));
        }
    }
}

