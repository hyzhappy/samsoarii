/*
	The header file for simulate program of user PLC Device
   */

#include <stdint.h>

// Register Memorys
extern int32_t XBit[128];
extern int32_t YBit[128];
extern int32_t MBit[256<<5];
extern int32_t CBit[256];
extern int32_t TBit[256];
extern int32_t SBit[32<<5];
extern int32_t DWord[8192];
extern int32_t CVWord[200];
extern int64_t CVDoubleWord[56];
extern int32_t TVWord[256];
extern int32_t AIWord[32];
extern int32_t AOWord[32];
extern int32_t VWord[8];
extern int32_t ZWord[8];
// Register writeable
// Set 1 if you want to modify the value of register
// othervise, Set 0 to lock the register and make it constant
extern int32_t XEnable[128];
extern int32_t YEnable[128];
extern int32_t MEnable[256<<5];
extern int32_t CEnable[256];
extern int32_t TEnable[256];
extern int32_t SEnable[32<<5];
extern int32_t DEnable[8192];
extern int32_t CVEnable[256];
extern int32_t TVEnable[256];
extern int32_t AIEnable[32];
extern int32_t AOEnable[32];
extern int32_t VEnable[8];
extern int32_t ZEnable[8];
// pulse signal frequency
extern uint64_t YFeq[4];


