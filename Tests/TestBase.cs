using Konamiman.Nestor80.Assembler;
using Konamiman.Z280dotNet.Facade;
using NUnit.Framework;

namespace Konamiman.Z280dotNet.Tests;

public abstract class TestBase
{
    protected const int DEFAULT_INITIAL_SSP = 0xFFFD;

    protected static Z280CPU z280;

    protected static byte[] ram = new byte[16 * 1024 * 1024];
    protected static byte[] io = new byte[16 * 1024 * 1024];

    protected static AddressSpace ramSpace = new() {
        read_byte = (address) => ram[address],
        read_word = (address) => (ushort)(ram[address] + ram[address + 1] >> 8),
        write_byte = (address, data) => { ram[address] = data; },
        write_word = (address, data) => { ram[address] = (byte)(data & 0xFF); ram[address + 1] = (byte)(data >> 8); },
        read_raw_byte = (address) => ram[address],
        read_raw_word = (address) => (ushort)(ram[address] + (ram[address + 1] << 8))
    };

    protected static AddressSpace ioSpace = new() {
        read_byte = (address) => io[address],
        read_word = (address) => (ushort)(io[address] + io[address + 1] >> 8),
        write_byte = (address, data) => { io[address] = data; },
        write_word = (address, data) => { io[address] = (byte)(data & 0xFF); io[address + 1] = (byte)(data >> 8); },
        read_raw_byte = (address) => io[address],
        read_raw_word = (address) => (ushort)(io[address] + (io[address + 1] << 8))
    };

    [OneTimeSetUp]
    public static void SetUpFixture()
    {
        z280 = Z280CPU.Create(ramSpace, ioSpace);
    }

    [OneTimeTearDown]
    public static void TearDownFixture()
    {
        z280.Dispose();
    }

    protected static int AssembleAndLoad(string sourceCode, int address = 0)
    {
        sourceCode = $" org {address}\r\n .cpu z280\r\n{sourceCode.Replace("|","\r\n ")}";

        var assemblyResult = AssemblySourceProcessor.Assemble(sourceCode, new AssemblyConfiguration());
        if(assemblyResult.HasErrors) {
            var errorString = string.Join("\r\n", assemblyResult.Errors.Select(e => $"{e.LineNumber-2}: {e.Message}").ToArray());
            throw new Exception($"Error assembling:\r\n\r\n" + errorString);
        }

        var ms = new MemoryStream(ram, address, ram.Length - address);
        var size = OutputGenerator.GenerateAbsolute(assemblyResult, ms);
        ms.Dispose();
        return address + size - 1;
    }

    protected static void Run(int address = 0, int initialSSP = DEFAULT_INITIAL_SSP, bool reset = true)
    {
        if(reset)
            z280.Reset();

        if(initialSSP != -1)
            z280.SSP = (ushort)initialSSP;

        if(address != 0)
            z280.PC = (ushort)address;

        while(z280.SSP <= initialSSP) {
            z280.ExecuteInstruction();
        }
    }

    protected static void AssembleAndRun(string code, int address = 0, int initialSSP = DEFAULT_INITIAL_SSP, bool reset = true)
    {
        AssembleAndLoad(code, address);
        Run(address, initialSSP, reset);
    }
}
