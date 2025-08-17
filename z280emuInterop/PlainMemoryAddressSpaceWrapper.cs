namespace Konamiman.Z280emuDotNet;

public class PlainMemoryAddressSpaceWrapper
{
    public AddressSpace AddressSpace { get; private set; }

    public byte[] Contents { get; private set; } = new byte[16 * 1024 * 1024];

    public PlainMemoryAddressSpaceWrapper()
    {
        AddressSpace = new() {
            read_byte = (address) => Contents[address],
            read_word = (address) => (ushort)(Contents[address] + Contents[address + 1] >> 8),
            write_byte = (address, data) => { Contents[address] = data; },
            write_word = (address, data) => { Contents[address] = (byte)(data & 0xFF); Contents[address + 1] = (byte)(data >> 8); },
            read_raw_byte = (address) => Contents[address],
            read_raw_word = (address) => (ushort)(Contents[address] + (Contents[address + 1] << 8))
        };
    }

}
