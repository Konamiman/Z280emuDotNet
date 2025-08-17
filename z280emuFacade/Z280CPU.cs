using System.Runtime.InteropServices;

namespace Konamiman.Z280dotNet.Facade;

/// <summary>
/// Z280 CPU main struct.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Z280CPU : IDisposable
{
    public Z280CPU()
    {
    }

    #region Fields matching the C structure, must be in the same order as in the C code

    internal IntPtr m_tag;
    internal uint m_type;
    internal uint m_clock;
    internal IntPtr m_token; // Pointer to the Z280State structure
    internal IntPtr z280uart_tag;
    internal IntPtr z280uart;
    internal IntPtr bti_init_cb;
    internal int m_bus16; /* OPT pin */
    internal uint m_ctin0, m_ctin1, m_ctin2;
    internal ushort ctin1_brg_const, ctin1_uart_timer;

    #endregion

    internal IntPtr ram;
    internal IntPtr iospace;
    internal IntPtr self;

    /// <summary>
    /// Reference to the state structure. In general it's not recommended to access this directly,
    /// instead use the properties below (and create new properties if needed) to access the CPU registers and state.
    /// 
    /// Note that if state is modified manually via the fields of this object, the changes must be written back
    /// (so get, then modify, then set).
    /// </summary>
    public Z280State State
    {
        get => Marshal.PtrToStructure<Z280State>(m_token); 
        set => Marshal.StructureToPtr(value, m_token, false);
    }

    public static Z280CPU Create(AddressSpace ram, AddressSpace io) => Z280Interop.CreateZ280(ram, io);

    private bool disposed = false;
    public void Dispose()
    {
        if(!disposed) {
            Z280Interop.FreeZ280(this);
            disposed = true;
        }
    }

    public void ExecuteInstruction() => Z280Interop.execute_z280(ref this);

    public void Reset() => Z280Interop.reset_z280(ref this);

    public ushort AF
    {
        get => State.AF.UInt16low;
        set
        {
            var z280State = State;
            z280State.AF.UInt16low = value;
            State = z280State;
        }
    }

    public byte A
    {
        get => State.AF.Byte1;
        set
        {
            var z280State = State;
            z280State.AF.Byte1 = value;
            State = z280State;
        }
    }

    public byte F
    {
        get => State.AF.Byte0;
        set
        {
            var z280State = State;
            z280State.AF.Byte0 = value;
            State = z280State;
        }
    }

    public ushort HL
    {
        get => State.HL.UInt16low;
        set
        {
            var z280State = State;
            z280State.HL.UInt16low = value;
            State = z280State;
        }
    }

    public byte H
    {
        get => State.HL.Byte1;
        set
        {
            var z280State = State;
            z280State.HL.Byte1 = value;
            State = z280State;
        }
    }

    public byte L
    {
        get => State.HL.Byte0;
        set
        {
            var z280State = State;
            z280State.HL.Byte0 = value;
            State = z280State;
        }
    }

    public ushort DE
    {
        get => State.DE.UInt16low;
        set
        {
            var z280State = State;
            z280State.DE.UInt16low = value;
            State = z280State;
        }
    }

    public byte D
    {
        get => State.DE.Byte1;
        set
        {
            var z280State = State;
            z280State.DE.Byte1 = value;
            State = z280State;
        }
    }

    public byte E     
    {
        get => State.DE.Byte0;
        set
        {
            var z280State = State;
            z280State.DE.Byte0 = value;
            State = z280State;
        }
    }

    public ushort BC
    {
        get => State.BC.UInt16low;
        set
        {
            var z280State = State;
            z280State.BC.UInt16low = value;
            State = z280State;
        }
    }

    public byte B
    {
        get => State.BC.Byte1;
        set
        {
            var z280State = State;
            z280State.BC.Byte1 = value;
            State = z280State;
        }
    }

    public byte C
    {
        get => State.BC.Byte0;
        set
        {
            var z280State = State;
            z280State.BC.Byte0 = value;
            State = z280State;
        }
    }

    public ushort IX
    {
        get => State.IX.UInt16low;
        set
        {
            var z280State = State;
            z280State.IX.UInt16low = value;
            State = z280State;
        }
    }

    public byte IXH
    {
        get => State.IX.Byte1;
        set
        {
            var z280State = State;
            z280State.IX.Byte1 = value;
            State = z280State;
        }
    }

    public byte IXL
    {
        get => State.IX.Byte0;
        set
        {
            var z280State = State;
            z280State.IX.Byte0 = value;
            State = z280State;
        }
    }

    public ushort IY
    {
        get => State.IY.UInt16low;
        set
        {
            var z280State = State;
            z280State.IY.UInt16low = value;
            State = z280State;
        }
    }

    public byte IYH
    {
        get => State.IY.Byte1;
        set
        {
            var z280State = State;
            z280State.IY.Byte1 = value;
            State = z280State;
        }
    }

    public byte IYL
    {
        get => State.IY.Byte0;
        set
        {
            var z280State = State;
            z280State.IY.Byte0 = value;
            State = z280State;
        }
    }

    public ushort PC
    {
        get => State.PREPC.UInt16low;
        set
        {
            var z280State = State;
            z280State.PREPC.UInt16low = value;
            State = z280State;
        }
    }


    public ushort SSP
    {
        get => State.SSP.UInt16low;
        set
        {
            var z280State = State;
            z280State.SSP.UInt16low = value;
            State = z280State;
        }
    }

    public ushort USP
    {
        get => State.USP.UInt16low;
        set
        {
            var z280State = State;
            z280State.USP.UInt16low = value;
            State = z280State;
        }
    }

    public byte R
    {
        get => State.R;
        set
        {
            var z280State = State;
            z280State.R = value;
            State = z280State;
        }
    }

    public bool IFF2 => (State.IFF2 & 1) != 0;

    public ushort AltAF
    {
        get => State.AltAF.UInt16low;
        set
        {
            var z280State = State;
            z280State.AltAF.UInt16low = value;
            State = z280State;
        }
    }

    public byte AltA
    {
        get => State.AltAF.Byte1;
        set
        {
            var z280State = State;
            z280State.AltAF.Byte1 = value;
            State = z280State;
        }
    }

    public byte AltF
    {
        get => State.AltAF.Byte0;
        set
        {
            var z280State = State;
            z280State.AltAF.Byte0 = value;
            State = z280State;
        }
    }

    public ushort AltHL
    {
        get => State.AltHL.UInt16low;
        set
        {
            var z280State = State;
            z280State.AltHL.UInt16low = value;
            State = z280State;
        }
    }

    public byte AltH
    {
        get => State.AltHL.Byte1;
        set
        {
            var z280State = State;
            z280State.AltHL.Byte1 = value;
            State = z280State;
        }
    }

    public byte AltL
    {
        get => State.AltHL.Byte0;
        set
        {
            var z280State = State;
            z280State.AltHL.Byte0 = value;
            State = z280State;
        }
    }

    public ushort AltDE
    {
        get => State.AltDE.UInt16low;
        set
        {
            var z280State = State;
            z280State.AltDE.UInt16low = value;
            State = z280State;
        }
    }

    public byte AltD
    {
        get => State.AltDE.Byte1;
        set
        {
            var z280State = State;
            z280State.AltDE.Byte1 = value;
            State = z280State;
        }
    }

    public byte AltE
    {
        get => State.AltDE.Byte0;
        set
        {
            var z280State = State;
            z280State.AltDE.Byte0 = value;
            State = z280State;
        }
    }

    public ushort AltBC
    {
        get => State.AltBC.UInt16low;
        set
        {
            var z280State = State;
            z280State.AltBC.UInt16low = value;
            State = z280State;
        }
    }

    public byte AltB
    {
        get => State.AltBC.Byte1;
        set
        {
            var z280State = State;
            z280State.AltBC.Byte1 = value;
            State = z280State;
        }
    }

    public byte AltC
    {
        get => State.AltBC.Byte0;
        set
        {
            var z280State = State;
            z280State.AltBC.Byte0 = value;
            State = z280State;
        }
    }

    public byte I
    {
        get => State.I;
        set
        {
            var z280State = State;
            z280State.I = value;
            State = z280State;
        }
    }

    public bool AltAFInUse => State.AF2inuse != 0;

    public bool AltRegsInUse => State.BC2inuse != 0;

    public byte IntMode => State.IM;

    public bool Halted => State.HALT != 0;
}