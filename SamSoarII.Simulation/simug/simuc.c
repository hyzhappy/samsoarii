#include <stdint.h>
#include "simulib.h"
#include "simuf.h"
#include "simuc.h"

static int32_t _global[4];
void RunLadder()
{
_itr_invoke();
uint32_t _stack_1;
_stack_1 = (_global[0]==0&&MBit[0]==1);
_global[0] = MBit[0];
if (_stack_1) {
MAX(&DWord[1],&DWord[0],&DWord[100]);
}
_stack_1 = (_global[1]==0&&MBit[1]==1);
_global[1] = MBit[1];
if (_stack_1) {
MAX(&DWord[1],&DWord[0],&DWord[101]);
}
_stack_1 = (_global[2]==0&&MBit[2]==1);
_global[2] = MBit[2];
if (_stack_1) {
AVE(&DWord[1],&DWord[0],&DWord[102]);
}
_stack_1 = (_global[3]==0&&MBit[4]==1);
_global[3] = MBit[4];
if (_stack_1) {
ASSERT_TEST();}
}
