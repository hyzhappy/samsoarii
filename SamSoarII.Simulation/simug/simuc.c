#include <stdint.h>
#include "simulib.h"
#include "simuf.h"
#include "simuc.h"

static uint16_t _global[0];
static uint32_t _old_XBits0;
static uint32_t _old_XBits1;
static uint16_t _interrupt_enable;
static void (*_interrupt[8])();

void _interrupt_proc()
{
if (_interrupt_enable) {
if (_old_XBits0==0 && XBit[0]!=0 && _interrupt[0])
_interrupt[0]();
if (_old_XBits0!=0 && XBit[0]==0 && _interrupt[1])
_interrupt[1]();
if (((_old_XBits0==0 && XBit[0]!=0) || (_old_XBits1!=0 && XBit[0]==0)) && _interrupt[2])
_interrupt[2]();
if (_old_XBits1==0 && XBit[1]!=0 && _interrupt[3])
_interrupt[3]();
if (_old_XBits1!=0 && XBit[1]==0 && _interrupt[4])
_interrupt[4]();
if (((_old_XBits1==0 && XBit[1]!=0) || (_old_XBits1!=0 && XBit[1]==0)) && _interrupt[5])
_interrupt[5]();
if (_interrupt_timer_arrive(6) && !_interrupt[6])
_interrupt[6]();
if (_interrupt_timer_arrive(7) && !_interrupt[7])
_interrupt[7]();
}
_old_XBits0 = XBit[0];
_old_XBits1 = XBit[1];
}

void _SBR_FUNC1();void RunLadder()
{
_interrupt_proc();
uint16_t temp;
uint16_t _stack_1;
_stack_1 = XBit[0];
if (_stack_1) 
_SBR_FUNC1();
}

void _SBR_FUNC1(){
uint16_t _stack_1;
_stack_1 = XBit[1];
if (!DEnable[0])if (_stack_1) {
DWord[0] = 10;
}
}
