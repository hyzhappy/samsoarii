#include <stdint.h>
#include "downlib.h"
#include "downf.h"
#include "downc.h"
static int32_t _global[0];
void RunLadder()
{
_itr_invoke();
uint32_t _stack_1;
_stack_1 = (DWord[0+VWord[0]]<DWord[1+VWord[1]]);
_stack_1 &= (16==0x10);
_stack_1 &= (0x16!=16);
_stack_1 &= (DWord[0]>=CVWord[0]);
_stack_1 &= (CVWord[1]<=TVWord[0]);
_stack_1 &= (AOWord[0]>AIWord[0]);
YBit[0] = _stack_1;
_stack_1 = (16==0x10);
_stack_1 &= (0x10!=10);
_stack_1 &= ((*((int32_t*)(DWord+10)))>=(*((int32_t*)(DWord+12))));
_stack_1 &= ((*((int32_t*)(DWord+14)))<=(*((int32_t*)(DWord+16))));
_stack_1 &= (CV32DoubleWord[0]>CV32DoubleWord[2]);
_stack_1 &= (CV32DoubleWord[4]<CV32DoubleWord[6]);
YBit[10] = _stack_1;
}
