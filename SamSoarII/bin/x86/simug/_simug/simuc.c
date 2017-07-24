#include <stdint.h>
#include "simulib.h"
#include "simuf.h"
#include "simuc.h"

static int32_t _global[0];
static int32_t _signal;
void RunLadder()
{
callinto();
_itr_invoke();
uint32_t _stack_1;
uint32_t _stack_2;
bpcycle(1);
_stack_1 = XBit[0];
_signal = _stack_1;
cpcycle(1, _signal);
bpcycle(2);
_stack_1 &= XBit[1];
_signal = _stack_1;
cpcycle(2, _signal);
bpcycle(3);
_stack_1 &= XBit[2];
_signal = _stack_1;
cpcycle(3, _signal);
bpcycle(4);
if (!(YEnable[0])) 
{
YBit[0] = _stack_1;
}
_signal = _stack_1;
cpcycle(4, _signal);
bpcycle(5);
_stack_2 = !XBit[3];
_signal = _stack_2;
cpcycle(5, _signal);
bpcycle(6);
_stack_2 &= !XBit[4];
_signal = _stack_2;
cpcycle(6, _signal);
bpcycle(7);
_stack_2 &= !XBit[5];
_signal = _stack_2;
cpcycle(7, _signal);
bpcycle(8);
if (!(YEnable[1])) 
{
YBit[1] = _stack_2;
}
_signal = _stack_2;
cpcycle(8, _signal);
callleave();
}
