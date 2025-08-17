using System.Runtime.InteropServices;

namespace Konamiman.Z280emuDotNet;

[StructLayout(LayoutKind.Sequential)]
public struct Z80DaisyInterface
{
    // Delegate declarations
    public delegate int Z80DaisyIrqStateCallback(nint device);
    public delegate int Z80DaisyIrqAckCallback(nint device);
    public delegate void Z80DaisyIrqRetiCallback(nint device);

    // Pointer to device (z280_device)
    public nint m_device; // chained device (eg.85230)

    // Pointer to next device in the chain
    public nint m_daisy_next; // next device in the chain

    // Function pointer callbacks with MarshalAs
    [MarshalAs(UnmanagedType.FunctionPtr)]
    public Z80DaisyIrqStateCallback z80daisy_irq_state_cb;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public Z80DaisyIrqAckCallback z80daisy_irq_ack_cb;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public Z80DaisyIrqRetiCallback z80daisy_irq_reti_cb;

    // Last opcode byte
    public byte m_last_opcode;
}