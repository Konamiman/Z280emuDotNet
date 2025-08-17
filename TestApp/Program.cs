using Konamiman.Nestor80.Assembler;
using Konamiman.Z280dotNet.Facade;

namespace TestApp
{
    internal class Program
    {
        static Z280CPU z280;

        static byte[] ram = new byte[16 * 1024 * 1024];
        static byte[] io = new byte[16 * 1024 * 1024];

        unsafe static void Main(string[] args)
        {
            var ramSpace = new AddressSpace {
                read_byte = (address) => ram[address],
                read_word = (address) => (ushort)(ram[address] + ram[address + 1] >> 8),
                write_byte = (address, data) => { ram[address] = data; },
                write_word = (address, data) => { ram[address] = (byte)(data & 0xFF); ram[address + 1] = (byte)(data >> 8); },
                read_raw_byte = (address) => ram[address],
                read_raw_word = (address) => (ushort)(ram[address] + (ram[address + 1] << 8))
            };

            var ioSpace = new AddressSpace() {
                read_byte = (address) => io[address],
                read_word = (address) => (ushort)(io[address] + io[address + 1] >> 8),
                write_byte = (address, data) => { io[address] = data; },
                write_word = (address, data) => { io[address] = (byte)(data & 0xFF); io[address + 1] = (byte)(data >> 8); },
                read_raw_byte = (address) => io[address],
                read_raw_word = (address) => (ushort)(io[address] + (io[address + 1] << 8))
            };

            z280 = Z280CPU.Create(ramSpace, ioSpace);
            z280.Reset();

            var code = @"
    ld hl,1234h
    addw hl,10h
    ld (0x1000),hl

    ld a,10h
    ld b,50h
    mult a,b

    ;exx
    ld hl,05678h
    ex af,af

    ld de,1
    jar AKI
    ld de,2
AKI:    

    ret
";

            AssembleAndLoad(code, 0);

            z280.SSP = 0xFFF0;
 
            while(z280.SSP <= 0xFFF0) {
                z280.ExecuteInstruction();
            }

            var x = ram[0x1000] + (ram[0x1001] << 8); // Should be 0x1244
            var hl = z280.State.HL.UInt16low;    //Should be 0x0500

            z280.Dispose();
        }

        static unsafe void AssembleAndLoad(string sourceCode, int address = 0)
        {
            sourceCode = $" org {address}\r\n .cpu z280\r\n{sourceCode}";

            var assemblyResult = AssemblySourceProcessor.Assemble(sourceCode, new AssemblyConfiguration());
            if(assemblyResult.HasErrors) {
                var errorString = string.Join("\r\n", assemblyResult.Errors.Select(e => $"{e.LineNumber}: {e.Message}").ToArray());
                throw new Exception($"Error assembling:\r\n\r\n" + errorString);
            }

            var ms = new MemoryStream(ram, address, ram.Length-address);
            OutputGenerator.GenerateAbsolute(assemblyResult, ms);
            ms.Dispose();
        }
    }
}
