using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Konamiman.Z280dotNet.Facade;

[StructLayout(LayoutKind.Sequential)]
public struct Z80DaisyInterface
{
    // Delegate declarations
    public delegate int Z80DaisyIrqStateCallback(IntPtr device);
    public delegate int Z80DaisyIrqAckCallback(IntPtr device);
    public delegate void Z80DaisyIrqRetiCallback(IntPtr device);

    // Pointer to device (z280_device)
    public IntPtr m_device; // chained device (eg.85230)

    // Pointer to next device in the chain
    public IntPtr m_daisy_next; // next device in the chain

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