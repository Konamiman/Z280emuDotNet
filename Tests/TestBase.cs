using Konamiman.Nestor80.Assembler;
using Konamiman.Z280emuDotNet;
using NUnit.Framework;

namespace Konamiman.Z280dotNet.Tests;

public abstract class TestBase
{
    protected const int DEFAULT_INITIAL_SSP = 0xFFFD;

    protected static Z280CPU z280;

    private static PlainMemoryAddressSpaceWrapper ramSpaceWrapper = new();
    protected static byte[] ram = ramSpaceWrapper.Contents;

    private static PlainMemoryAddressSpaceWrapper ioSpaceWrapper = new();
    protected static byte[] io = ioSpaceWrapper.Contents;

    [OneTimeSetUp]
    public static void SetUpFixture()
    {
        z280 = Z280CPU.Create(ramSpaceWrapper.AddressSpace, ioSpaceWrapper.AddressSpace);
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
