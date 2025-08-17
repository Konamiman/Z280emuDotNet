using System.Runtime.InteropServices;

namespace Konamiman.Z280dotNet.Facade;

internal class Z280Interop
{
    const string DLL_NAME = "z280emudll.dll";

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr create_z280(IntPtr ram, IntPtr iospace);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void reset_z280(ref Z280CPU device);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void execute_z280(ref Z280CPU device);

    public static Z280CPU CreateZ280(AddressSpace ram, AddressSpace iospace)
    {
        var ramPtr = Marshal.AllocHGlobal(Marshal.SizeOf<AddressSpace>());
        var ioPtr = Marshal.AllocHGlobal(Marshal.SizeOf<AddressSpace>());
        Marshal.StructureToPtr(ram, ramPtr, false);
        Marshal.StructureToPtr(iospace, ioPtr, false);

        var cpuPtr = create_z280(ramPtr, ioPtr);
        var cpu = Marshal.PtrToStructure<Z280CPU>(cpuPtr);
        cpu.ram = ramPtr;
        cpu.iospace = ioPtr;
        cpu.self = cpuPtr;

        return cpu;
    }

    public static void FreeZ280(Z280CPU device)
    {
        if(device.ram != IntPtr.Zero) {
            Marshal.FreeHGlobal(device.ram);
            device.ram = IntPtr.Zero;
        }
        if(device.iospace != IntPtr.Zero) {
            Marshal.FreeHGlobal(device.iospace);
            device.iospace = IntPtr.Zero;
        }
        if(device.self != IntPtr.Zero) {
            Marshal.FreeHGlobal(device.self);
            device.self = IntPtr.Zero;
        }
    }


#if false

    // Delegate declarations for the callback types
    public delegate int DeviceIrqAcknowledgeCallback(IntPtr device, int irqnum);
    public delegate byte InitByteCallback(IntPtr device);
    public delegate void TxCallback(IntPtr device, int channel, byte data);
    public delegate int RxCallback(IntPtr device, int channel);

    public delegate int DeviceIrqAcknowledgeCallbackCSharp(Z280Device device, int irqnum);
    public delegate byte InitByteCallbackCSharp(Z280Device device);
    public delegate void TxCallbackCSharp(Z280Device device, int channel, byte data);
    public delegate int RxCallbackCSharp(Z280Device device, int channel);

    // Import the native function
    [DllImport("../../../../x64/Debug/z80emudll.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr cpu_create_z280(
        [MarshalAs(UnmanagedType.LPStr)] string tag,
        uint type,
        uint clock,
        ref AddressSpace ram,
        ref AddressSpace iospace,
        [MarshalAs(UnmanagedType.FunctionPtr)] DeviceIrqAcknowledgeCallback irqcallback,
        ref Z80DaisyInterface daisy_init,
        [MarshalAs(UnmanagedType.FunctionPtr)] InitByteCallback bti_init_cb,
        int bus16,
        uint ctin0,
        uint ctin1,
        uint ctin2,
        [MarshalAs(UnmanagedType.FunctionPtr)] RxCallback z280uart_rx_cb,
        [MarshalAs(UnmanagedType.FunctionPtr)] TxCallback z280uart_tx_cb
    );

    // Example usage method
    public static IntPtr CreateZ280Device(
        string tag,
        uint type,
        uint clock,
        AddressSpace ram,
        AddressSpace iospace,
        DeviceIrqAcknowledgeCallback irqcallback,
        Z80DaisyInterface daisy_init,
        InitByteCallback bti_init_cb,
        int bus16,
        uint ctin0,
        uint ctin1,
        uint ctin2,
        RxCallback z280uart_rx_cb,
        TxCallback z280uart_tx_cb)
    {
        return cpu_create_z280(
            tag, type, clock,
            ref ram, ref iospace,
            irqcallback, ref daisy_init,
            bti_init_cb, bus16,
            ctin0, ctin1, ctin2,
            z280uart_rx_cb, z280uart_tx_cb
        );
    }
}

#endif

#if false
public class Z280Interop
{
    // Keep the original IntPtr-based delegates for the actual interop
    public delegate int DeviceIrqAcknowledgeCallback(IntPtr device, int irqnum);
    public delegate byte InitByteCallback(IntPtr device);
    public delegate void TxCallback(IntPtr device, int channel, byte data);
    public delegate int RxCallback(IntPtr device, int channel);

    // Public C#-friendly delegates
    public delegate int DeviceIrqAcknowledgeCallbackCSharp(Z280Device device, int irqnum);
    public delegate byte InitByteCallbackCSharp(Z280Device device);
    public delegate void TxCallbackCSharp(Z280Device device, int channel, byte data);
    public delegate int RxCallbackCSharp(Z280Device device, int channel);

    public static Z280Device CreateZ280Device(
        string tag, uint type, uint clock,
        AddressSpace ram, AddressSpace iospace,
        DeviceIrqAcknowledgeCallbackCSharp irqcallback,
        Z80DaisyInterface daisy_init,
        InitByteCallbackCSharp bti_init_cb,
        int bus16, uint ctin0, uint ctin1, uint ctin2,
        RxCallbackCSharp z280uart_rx_cb, TxCallbackCSharp z280uart_tx_cb)
    {
        // Convert C# callbacks to native callbacks
        DeviceIrqAcknowledgeCallback nativeIrqCallback = (devicePtr, irqnum) =>
            irqcallback(new Z280Device(devicePtr), irqnum);
        // ... similar conversions for other callbacks

        // Call the native function with converted parameters
        IntPtr nativeHandle = cpu_create_z280(/* parameters */);
        return new Z280Device(nativeHandle);
    }
}

#endif

}