#ifndef _PLC_H_
#define _PLC_H_


#include <stdint.h>
typedef uint32_t plc_bool;
typedef int32_t plc_dw;
typedef int16_t plc_w;


extern plc_dw 	XDoubleWord[4];
extern plc_dw 	YDoubleWord[4];
extern plc_dw 	MDoubleWord[256];
extern plc_dw 	CDoubleWord[8];
extern plc_dw 	TDoubleWord[8];


extern plc_bool* XBit;
extern plc_bool* YBit;
extern plc_bool* MBit;
extern plc_bool* CBit;
extern plc_bool* TBit;
extern plc_bool* SBit;

extern plc_w 	AIWord[32];
extern plc_w 	AOWord[32];
extern plc_w 	DWord[8192];
extern plc_w 	VWord[8];
extern plc_w 	ZWord[8];
extern plc_w 	TVWord[256];
extern plc_w 	CVWord[200];
extern plc_dw 	CVDoubleWord[56];
plc_dw*	CV32DoubleWord = &CVDoubleWord[0];

extern void CI_TON(uint8_t en, uint16_t Tnum, uint32_t SetValue);
extern void CI_DPLSF(uint8_t en, uint32_t freq, uint16_t Yn, uint16_t use_id);
extern void CI_DPWM(uint8_t en, uint32_t freq, uint32_t dutycycle, uint16_t Yn, uint16_t use_id);
extern void CI_DPLSY(uint8_t en, uint32_t freq, uint32_t pulsenum, uint16_t Yn, uint16_t use_id);
extern void CI_HCNT(uint8_t en, uint16_t hcntIndex, uint32_t setValue);


#endif


