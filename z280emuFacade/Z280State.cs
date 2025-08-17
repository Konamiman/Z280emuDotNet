using System.Runtime.InteropServices;

namespace Konamiman.Z280emuDotNet
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Z280State
    {
        private const int POINTER_SIZE = 8;
        private const int Z280_CRSIZE = 0x18;
        private const int Z280_INT_MAX = 12;

        public Register PREPC;
        public Register PC, SSP, USP, AF, BC, DE, HL, IX, IY;
        public Register AltAF, AltBC, AltDE, AltHL;
        public byte AF2inuse, BC2inuse;
        public byte R, IFF2;
        public byte HALT, IM;
        public byte I;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Z280_CRSIZE)]
        public byte[] cr;

        public byte rrr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] mmur;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public ushort[] pdr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ctcr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] ctcsr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public ushort[] ctctr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public ushort[] cttcr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] sar;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] dar;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] dmatdr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] dmacnt;

        public byte dmamcr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dma_pending;

        public int dma_active;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] rdy_state;

        public byte nmi_state;
        public byte nmi_pending;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] irq_state;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Z280_INT_MAX + 1)]
        public byte[] int_pending;

        public byte after_EI;
        public uint ea;
        public int eapdr;
        public ushort timer_cnt;
        public nint daisy;
        public nint irq_callback;
        public nint device;
        public nint ram;
        public nint iospace;
        public int icount;
        public int extra_cycles;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8 * POINTER_SIZE)]
        public nint[] cc;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] abort_handler;

        public byte abort_type;
    }
}
