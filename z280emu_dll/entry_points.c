#include "../z280emu_repo_submodule/z280/z80common.h"
#include "../z280emu_repo_submodule/z280/z280.h"

#define XTALCLK 29491200

void debugger_instruction_hook(device_t* device, offs_t curpc) {}

int irq0ackcallback(device_t* device, int irqnum) {
    return 0;
}

UINT8 init_bti(device_t* device) {
    // DIC: 0
    // BS: 0 CF, 1=UART // the board has a jumper for UART bootstrap. TODO
    // LM: 0 =no wait
    // CS: 0 =1/2 clock
    return 0;
}

char* name = (char*)"Z280";

__declspec(dllexport) struct z280_device* __cdecl create_z280(struct address_space* ram, struct address_space* iospace);
    
struct z280_device* create_z280(
    struct address_space* ram,
    struct address_space* iospace) {

    return cpu_create_z280(
        name, //tag,
        Z280_TYPE_Z280, //type,
        XTALCLK / 2, //clock,
        ram,
        iospace,
        irq0ackcallback, //irqcallback,
        NULL, //daisy_init,
        init_bti, //bti_init_cb,
        1, //bus16,
        0, //ctin0,
        XTALCLK / 16, //ctin1,
        0, //ctin2,
        NULL, //z280uart_rx_cb,
        NULL  //z280uart_tx_cb
    );
}

__declspec(dllexport) void __cdecl reset_z280(device_t* device) { cpu_reset_z280(device); }

__declspec(dllexport) void __cdecl execute_z280(device_t* device) { cpu_execute_z280(device, 1); }
