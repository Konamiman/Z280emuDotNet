using System.Runtime.InteropServices;

namespace Konamiman.Z280emuDotNet;

[StructLayout(LayoutKind.Sequential)]
public struct AddressSpace
{
    // Delegate type declarations (no MarshalAs here)
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte ReadByteDelegate(uint byteaddress);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ushort ReadWordDelegate(uint byteaddress);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WriteByteDelegate(uint byteaddress, byte data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WriteWordDelegate(uint byteaddress, ushort data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte ReadRawByteDelegate(uint byteaddress);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ushort ReadRawWordDelegate(uint byteaddress);

    // The actual function pointer fields with MarshalAs
    [MarshalAs(UnmanagedType.FunctionPtr)]
    public ReadByteDelegate read_byte;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public ReadWordDelegate read_word;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public WriteByteDelegate write_byte;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public WriteWordDelegate write_word;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public ReadRawByteDelegate read_raw_byte;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public ReadRawWordDelegate read_raw_word;
}