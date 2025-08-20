using System.Runtime.InteropServices;

namespace Konamiman.Z280emuDotNet;

/// <summary>
/// Z280 CPU main struct. Client code should create an instance of this struct using the Create method
/// and interact with its public members. The only other classes/structs that should be used outside of this assembly
/// are AddressSpace and its wrapper classes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct Z280CPU : IDisposable
{
    public Z280CPU()
    {
    }

    #region Fields matching the C structure, must be in the same order as in the C code

    internal nint m_tag;
    internal uint m_type;
    internal uint m_clock;
    internal nint m_token; // Pointer to the Z280State structure
    internal nint z280uart_tag;
    internal nint z280uart;
    internal nint bti_init_cb;
    internal int m_bus16; /* OPT pin */
    internal uint m_ctin0, m_ctin1, m_ctin2;
    internal ushort ctin1_brg_const, ctin1_uart_timer;

    #endregion

    internal nint ram;
    internal nint iospace;
    internal nint self;

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

    /// <summary>
    /// Create a new instance of the Z280 CPU struct.
    /// </summary>
    /// <param name="ram">Address space to use as RAM.</param>
    /// <param name="io">Address space to use as I/O.</param>
    /// <returns></returns>
    public static Z280CPU Create(AddressSpace ram, AddressSpace io) => Z280emuInterop.CreateZ280(ram, io);

    private bool disposed = false;

    /// <summary>
    /// Free the unmanaged memory used by this Z280 CPU struct instance.
    /// </summary>
    public void Dispose()
    {
        if(!disposed) {
            Z280emuInterop.FreeZ280(this);
            disposed = true;
        }
    }

    /// <summary>
    /// Execute the instruction pointed by the PC register.
    /// </summary>
    public void ExecuteInstruction() => Z280emuInterop.execute_z280(ref this);

    /// <summary>
    /// Reset the CPU state to its initial values.
    /// </summary>
    public void Reset() => Z280emuInterop.reset_z280(ref this);

    public bool AltAFInUse => State.AF2inuse != 0;

    public bool AltRegsInUse => State.BC2inuse != 0;

    public byte IntMode => State.IM;

    public bool Halted => State.HALT != 0;

    public bool InUserMode => (MSR & 0x4000) != 0;
}