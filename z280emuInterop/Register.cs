using System.Runtime.InteropServices;

namespace Konamiman.Z280emuDotNet
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Register
    {
        // Byte access (LSB first)
        [FieldOffset(0)] public byte Byte0;   // least significant byte
        [FieldOffset(1)] public byte Byte1;   // high byte
        [FieldOffset(2)] public byte Byte2;  // second high byte
        [FieldOffset(3)] public byte Byte3;  // most significant byte

        // Word access (LSB first)
        [FieldOffset(0)] public ushort UInt16low;  // least significant word
        [FieldOffset(2)] public ushort UInt16high;  // high word

        // Signed byte access (LSB first)
        [FieldOffset(0)] public sbyte SByte0;   // signed least significant byte
        [FieldOffset(1)] public sbyte SByte1;   // signed high byte
        [FieldOffset(2)] public sbyte SByte2;  // signed second high byte
        [FieldOffset(3)] public sbyte SByte3;  // signed most significant byte

        // Signed word access (LSB first)
        [FieldOffset(0)] public short Int16low;  // signed least significant word
        [FieldOffset(2)] public short Int16high;  // signed high word

        // Double word access
        [FieldOffset(0)] public uint UInt32;     // unsigned 32-bit
        [FieldOffset(0)] public int Int32;     // signed 32-bit

        public override string ToString() => $"{UInt32:x8}";
    }
}
