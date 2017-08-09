/*
	The header file for simulate program of user PLC Device
   */

#include <stdint.h>

// Register Memorys
extern int8_t XBit[128];
extern int8_t YBit[128];
extern int8_t MBit[256<<5];
extern int8_t CBit[256];
extern int8_t TBit[256];
extern int8_t SBit[32<<5];
extern int16_t DWord[8192];
extern int16_t CVWord[200];
extern int32_t CV32DoubleWord[56];
extern int16_t TVWord[256];
extern int16_t AIWord[32];
extern int16_t AOWord[32];
extern int16_t VWord[8];
extern int16_t ZWord[8];
// Register writeable
// Set 1 if you want to modify the value of register
// othervise, Set 0 to lock the register and make it constant
extern int8_t XEnable[128];
extern int8_t YEnable[128];
extern int8_t MEnable[256<<5];
extern int8_t CEnable[256];
extern int8_t TEnable[256];
extern int8_t SEnable[32<<5];
extern int8_t DEnable[8192];
extern int8_t CVEnable[256];
extern int8_t CV32Enable[56];
extern int8_t TVEnable[256];
extern int8_t AIEnable[32];
extern int8_t AOEnable[32];
extern int8_t VEnable[8];
extern int8_t ZEnable[8];
// pulse signal frequency
extern uint32_t YFeq[4];

