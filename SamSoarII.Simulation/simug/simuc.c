#include <stdint.h>
#include "simulib.h"
#include "simuf.h"
#include "simuc.h"

static int32_t _global[2];
void _SBR_fsefws();
void _SBR_tgde();
void _SBR_asd();
void RunLadder()
{
_itr_invoke();
uint32_t _stack_1;
_stack_1 = (DWord[0]==DWord[1]);
if (!(TVEnable[0]||TEnable[0])) 
{
_ton(&TVWord[0], &TBit[0], _stack_1, 0, &_global[0]);
}
_stack_1 = (_global[1]==0&&MBit[8010]==1);
_global[1] = MBit[8010];
if (!(DEnable[0])) 
{
if (_stack_1) {
_mbus(232, NULL, 0, &DWord[0]);
}
}
_stack_1 = !XBit[0];
_stack_1 |= XBit[0];
if (!(YEnable[0])) 
{
YBit[0] = _stack_1;
}
}

void _SBR_fsefws(){
uint16_t _stack_1;
}

void _SBR_tgde(){
uint16_t _stack_1;
}

void _SBR_asd(){
uint16_t _stack_1;
}
