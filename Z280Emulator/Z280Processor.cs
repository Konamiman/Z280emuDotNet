using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Konamiman.Z280dotNet.Core;

public class Z280Processor
{
    #region z80common.h

    [StructLayout(LayoutKind.Explicit)]
    public struct Pair
    {
        // Byte access (LSB first)
        [FieldOffset(0)] public byte l;   // least significant byte
        [FieldOffset(1)] public byte h;   // high byte
        [FieldOffset(2)] public byte h2;  // second high byte
        [FieldOffset(3)] public byte h3;  // most significant byte

        // Word access (LSB first)
        [FieldOffset(0)] public ushort lw;  // least significant word
        [FieldOffset(2)] public ushort hw;  // high word

        // Signed byte access (LSB first)
        [FieldOffset(0)] public sbyte sl;   // signed least significant byte
        [FieldOffset(1)] public sbyte sh;   // signed high byte
        [FieldOffset(2)] public sbyte sh2;  // signed second high byte
        [FieldOffset(3)] public sbyte sh3;  // signed most significant byte

        // Signed word access (LSB first)
        [FieldOffset(0)] public short slw;  // signed least significant word
        [FieldOffset(2)] public short shw;  // signed high word

        // Double word access
        [FieldOffset(0)] public uint d;     // unsigned 32-bit
        [FieldOffset(0)] public int sd;     // signed 32-bit
    }

    /// <summary>
    /// Extracts the nth bit from value x
    /// </summary>
    /// <param name="x">The value to extract the bit from</param>
    /// <param name="n">The bit position (0-based)</param>
    /// <returns>The bit value (0 or 1)</returns>
    public static int Bit(int x, int n)
    {
        return (x >> n) & 1;
    }

    // standard state indexes
    private const int STATE_GENPC = -1;               // generic program counter (live)
    private const int STATE_GENPCBASE = -2;           // generic program counter (base of current instruction)
    private const int STATE_GENSP = -3;               // generic stack pointer
    private const int STATE_GENFLAGS = -4;            // generic flags

    public enum LineState
    {
        CLEAR_LINE = 0,             // clear (a fired or held) line
        ASSERT_LINE,                // assert an interrupt immediately
        HOLD_LINE,                  // hold interrupt line until acknowledged
        PULSE_LINE                  // pulse interrupt line instantaneously (only for NMI, RESET)
    }

    // I/O line definitions
    // input lines
    private const int MAX_INPUT_LINES = 32 + 3;
    private const int INPUT_LINE_IRQ0 = 0;
    private const int INPUT_LINE_IRQ1 = 1;
    private const int INPUT_LINE_IRQ2 = 2;
    private const int INPUT_LINE_IRQ3 = 3;
    private const int INPUT_LINE_IRQ4 = 4;
    private const int INPUT_LINE_IRQ5 = 5;
    private const int INPUT_LINE_IRQ6 = 6;
    private const int INPUT_LINE_IRQ7 = 7;
    private const int INPUT_LINE_IRQ8 = 8;
    private const int INPUT_LINE_IRQ9 = 9;
    private const int INPUT_LINE_NMI = MAX_INPUT_LINES - 3;

    // special input lines that are implemented in the core
    private const int INPUT_LINE_RESET = MAX_INPUT_LINES - 2;
    private const int INPUT_LINE_HALT = MAX_INPUT_LINES - 1;



    public enum ParityType
    {
        PARITY_NONE,     // no parity. a parity bit will not be in the transmitted/received data
        PARITY_ODD,      // odd parity
        PARITY_EVEN,     // even parity
        PARITY_MARK,     // one parity
        PARITY_SPACE     // zero parity
    }

    public enum StopBitsType
    {
        STOP_BITS_0,
        STOP_BITS_1 = 1,
        STOP_BITS_1_5 = 2,
        STOP_BITS_2 = 3
    }

    // address spaces
    public enum AddressSpaceNum
    {
        AS_0,                           // first address space
        AS_1,                           // second address space
        AS_2,                           // third address space
        AS_3,                           // fourth address space
        ADDRESS_SPACES,                 // maximum number of address spaces

        // alternate address space names for common use
        AS_PROGRAM = AS_0,              // program address space
        AS_DATA = AS_1,                 // data address space
        AS_IO = AS_2                    // I/O address space
    }

    // Disassembler constants for the return value
    private const uint DASMFLAG_SUPPORTED = 0x80000000;      // are disassembly flags supported?
    private const uint DASMFLAG_STEP_OUT = 0x40000000;       // this instruction should be the end of a step out sequence
    private const uint DASMFLAG_STEP_OVER = 0x20000000;      // this instruction should be stepped over by setting a breakpoint afterwards
    private const uint DASMFLAG_OVERINSTMASK = 0x18000000;   // number of extra instructions to skip when stepping over
    private const uint DASMFLAG_OVERINSTSHIFT = 27;          // bits to shift after masking to get the value
    private const uint DASMFLAG_LENGTHMASK = 0x0000ffff;     // the low 16-bits contain the actual length

    // int (*device_irq_acknowledge_callback)(device_t *device, int irqnum);
    public static Func<IntPtr, int, int> DeviceIrqAcknowledgeCallback;

    // void (*devcb_write_line)(device_t *device, int state);
    public static Action<IntPtr, int> DevcbWriteLine;

    // UINT8 (*init_byte_callback)(device_t *device);
    public static Func<IntPtr, byte> InitByteCallback;

    // struct with function pointers for accessors; use is generally discouraged unless necessary
    public class AddressSpace
    {
        // accessor methods for reading data
        public Func<uint, byte> ReadByte;
        public Func<uint, ushort> ReadWord;
        public Action<uint, byte> WriteByte;
        public Action<uint, ushort> WriteWord;

        // accessor methods for reading raw data (opcodes)
        public Func<uint, byte> ReadRawByte;    // offs_t directxor = 0 parameter omitted
        public Func<uint, ushort> ReadRawWord;  // offs_t directxor = 0 parameter omitted
    }

    #endregion

    private const int Z280_PC = 0x100000;
    private const int Z280_SP = 0x100001;
    private const int Z280_USP = 0x100002;
    private const int Z280_SSP = 0x100003;
    private const int Z280_AF = 0x100004;
    private const int Z280_BC = 0x100005;
    private const int Z280_DE = 0x100006;
    private const int Z280_HL = 0x100007;
    private const int Z280_IX = 0x100008;
    private const int Z280_IY = 0x100009;
    private const int Z280_A = 0x10000A;
    private const int Z280_B = 0x10000B;
    private const int Z280_C = 0x10000C;
    private const int Z280_D = 0x10000D;
    private const int Z280_E = 0x10000E;
    private const int Z280_H = 0x10000F;
    private const int Z280_L = 0x100010;
    private const int Z280_AF2 = 0x100011;
    private const int Z280_BC2 = 0x100012;
    private const int Z280_DE2 = 0x100013;
    private const int Z280_HL2 = 0x100014;
    private const int Z280_R = 0x100015;
    private const int Z280_I = 0x100016;
    private const int Z280_IM = 0x100017;
    private const int Z280_IFF2 = 0x100018;
    private const int Z280_HALT = 0x100019;
    private const int Z280_DC0 = 0x10001A;
    private const int Z280_DC1 = 0x10001B;
    private const int Z280_DC2 = 0x10001C;
    private const int Z280_DC3 = 0x10001D;
    private const int Z280_CR_MSR = 0x10001E;

    private const int Z280_GENPC = STATE_GENPC;
    private const int Z280_GENSP = STATE_GENSP;
    private const int Z280_GENPCBASE = STATE_GENPCBASE;


    private const int Z280_TABLE_op = 0;
    private const int Z280_TABLE_cb = 1;
    private const int Z280_TABLE_ed = 2;
    private const int Z280_TABLE_xy = 3;
    private const int Z280_TABLE_xycb = 4;
    private const int Z280_TABLE_dded = 5;
    private const int Z280_TABLE_fded = 6;
    private const int Z280_TABLE_ex = 7;    // cycles counts for taken jr/jp/call and interrupt latency (rst opcodes)
}
