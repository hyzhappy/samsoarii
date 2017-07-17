#include <stdint.h>
#include <math.h>
//#include <Windows.h>

#include "simuc.h"
#include "simuf.h"
#include "simulib.h"

/*
typedef short uint16_t;
typedef int uint32_t;
typedef long uint64_t;
*/
// temporary variable
uint16_t _tempw;
uint32_t _tempd;
float _tempf;
// new time
uint32_t ntim;
// inteval
uint32_t itv;
// interrupt timer 1 start
uint16_t itr_timer1_s;
// interrupt timer 2 start
uint16_t itr_timer2_s;
// interrupt timer 1 terminate
uint16_t itr_timer1_t;
// interrupt timer 2 terminate
uint16_t itr_timer2_t;
// interrupt timer 1 enable
uint16_t itr_timer1_e;
// interrupt timer 2 enable
uint16_t itr_timer2_e;

// Convert 16-bits integer to BCD code
uint16_t _WORD_to_BCD(uint16_t itg)
{
	return (itg % 10) | ((itg / 10 % 10) << 4) | ((itg / 100 % 10) << 8) | ((itg / 1000 % 10) << 12);
}

// Convert BCD code to normal 16-bits integer
uint16_t _BCD_to_WORD(uint16_t BCD)
{
	return ((BCD >> 12) & 15) * 1000 + ((BCD >> 8) & 15) * 100 + ((BCD >> 4) & 15) * 10 + (BCD & 15);
}

/*
	Set a series of BIT to 1
	addr : base address
	size : series size
*/
void _bitset(uint32_t* addr, uint16_t size)
{
	while (--size > 0)
		addr[size] = 0x00000001;
}

/*
	Reset a series of BIT to 0
	addr : base address
	size : series size
*/
void _bitrst(uint32_t* addr, uint16_t size)
{
	while (--size > 0)
		addr[size] = 0x00000000;
}

/*
	Duplicate a series of BIT to the targeted memory space
	
	source_addr : the base address of the source datas
	target_addr : the base address of the targeted datas
	size : series size
*/
void _bitcpy(uint32_t* source_addr, uint32_t* target_addr, uint16_t size)
{
	while (--size > 0)
		target_addr[size] = source_addr[size];
}

// Time counter
// tval : counter value
// tbit : the position of the counter bit in the block
// cod  : input condition
// sv   : reserve value
// otim : old time(ms)
void _ton(uint16_t* tval, uint32_t* tbit, uint16_t cod, uint16_t sv, uint32_t* otim)
{
	// if condition is TRUE
	if (cod)
	{
		// get the new time
		ntim = counttimems;
		// get the inteval from old to new
		itv = (ntim - *otim) / 100;
		// increase the counter value and old time
		*tval += itv;
		*otim += itv * 100;
		// arrive at the reserve value?
		if (*tval >= sv)
		{
			// set the counter bit
			*tbit = 0x00000001;
		}
	}
	else
	{
		// reset the counter value
		*tval = 0;
		// reset the counter bit to zero
		*tbit = 0x00000000;
	}
}

// Time counter (opened preservation)
// tval : counter value
// tbit : the position of the counter bit in the block
// cod  : input condition
// sv   : reserve value
// otim : old time(ms)
void _tonr(uint16_t* tval, uint32_t* tbit, uint16_t cod, uint16_t sv, uint32_t* otim)
{
	// if condition is TRUE
	if (cod)
	{
		// get the new time
		ntim = counttimems;
		// get the inteval from old to new
		itv = (ntim - *otim) / 100;
		// increase the counter value and old time
		*tval += itv;
		*otim += itv * 100;
		// arrive at the reserve value?
		if (*tval >= sv)
		{
			// set the counter bit
			*tbit = 0x00000001;
		}
	}
}

// Calculate the factorial of the 16-bit integer
uint32_t _fact(uint16_t itg)
{
	uint32_t ret = 1;
	while (itg > 1)
	{
		ret *= itg;
		itg--;
	}
	return ret;
}

// Compare a couple of 16-bit integers(named as ia and ib)
// return 0 if ia == ib
// return -1 if ia < ib
// return 1 if ia > ib
uint16_t _cmpw(uint16_t ia, uint16_t ib)
{
	if (ia == ib)
	{
		return 0;
	}
	if (ia < ib)
	{
		return -1;
	}
	if (ia > ib)
	{
		return 1;
	}
	return 1;
}

// compare a couple of 32-bit integers(named as ia and ib)
// return 0 if ia == ib
// return -1 if ia < ib
// return 1 if ia > ib
uint16_t _cmpd(uint32_t ia, uint32_t ib)
{
	if (ia == ib)
	{
		return 0;
	}
	if (ia < ib)
	{
		return -1;
	}
	if (ia > ib)
	{
		return 1;
	}
	return 1;
}

// compare a couple of 32-bit float(named as ia and ib)
// return 0 if ia == ib
// return -1 if ia < ib
// return 1 if ia > ib
uint16_t _cmpf(float ia, float ib)
{
	if (ia == ib)
	{
		return 0;
	}
	if (ia < ib)
	{
		return -1;
	}
	if (ia > ib)
	{
		return 1;
	}
	return 1;
}

// check it if the targeted 16-bit integer is inside the range [il, ir] (16-bit)
// return 0 if the range indeedly include it
// return -1 if it is less than left border
// return 1 if it is more than right border
uint16_t _zcpw(uint16_t ia, uint16_t il, uint16_t ir)
{
	if (ia >= il && ia <= ir)
	{
		return 0;
	}
	if (ia < il)
	{
		return -1;
	}
	if (ia > ir)
	{
		return 1;
	}
	return 1;
}

// check it if the targeted 32-bit integer is inside the range [il, ir] (32-bit)
// return 0 if the range indeedly include it
// return -1 if it is less than left border
// return 1 if it is more than right border
uint16_t _zcpd(uint32_t ia, uint32_t il, uint32_t ir)
{
	if (ia >= il && ia <= ir)
	{
		return 0;
	}
	if (ia < il)
	{
		return -1;
	}
	if (ia > ir)
	{
		return 1;
	}
	return 1;
}

// check it if the targeted 32-bit float is inside the range [il, ir] (32-bit float)
// return 0 if the range indeedly include it
// return -1 if it is less than left border
// return 1 if it is more than right border
uint16_t _zcpf(float ia, float il, float ir)
{
	if (ia >= il && ia <= ir)
	{
		return 0;
	}
	if (ia < il)
	{
		return -1;
	}
	if (ia > ir)
	{
		return 1;
	}
	return 1;
}

// swap a couple of 16-bit integers
void _xchw(uint16_t* pa, uint16_t* pb)
{
	_tempw = *pa;
	*pa = *pb;
	*pb = _tempw;
}

// swap a couple of 32-bit integer
void _xchd(uint16_t* pa, uint16_t* pb)
{
	_tempd = *pa;
	*pa = *pb;
	*pb = _tempd;
}

// swap a couple of 32-bit float
void _xchf(float* pa, float* pb)
{
	_tempf = *pa;
	*pa = *pb;
	*pb = _tempf;
}

// active an interrupt timer
// code : interrupt code
// t1 : counting time of timer1
// t2 : counting time of timer2
void _interrupt_timer_create(uint16_t code, uint16_t t1, uint16_t t2)
{
	if (code == 6)
	{
		itr_timer1_s = counttimems;
		itr_timer1_t = itr_timer1_t + t1;
		itr_timer1_e = 1;
	}
	if (code == 7)
	{
		itr_timer2_s = counttimems;
		itr_timer2_t = itr_timer2_s + t2;
		itr_timer2_e = 1;
	}
}

// close an interrupt timer
// code : interrupt code
void _interrupt_timer_cancel(uint16_t code)
{
	if (code == 6)
	{
		itr_timer1_e = 0;
	}
	if (code == 7)
	{
		itr_timer2_e = 0;
	}
}

// check if the timer is over the terminate
// code : interrupt code
uint16_t _interrupt_timer_arrive(uint16_t code)
{
	if (code == 6)
	{
		if (GetTickCount() >= itr_timer1_t)
		{
			itr_timer1_e = 0;
			return 1;
		}
	}
	if (code == 7)
	{
		if (GetTickCount() >= itr_timer2_t)
		{
			itr_timer2_e = 0;
			return 1;
		}
	}
	return 0;
}

void _memsetw(uint16_t* dest, uint16_t* source, uint16_t size)
{
	while (--size >= 0)
	{
		dest[size] = source[size];
	}
}

void _memsetd(uint32_t* dest, uint32_t* source, uint32_t size)
{
	while (--size >= 0)
	{
		dest[size] = source[size];
	}
}

void _smov(uint16_t source, uint16_t sb1, uint16_t sb2, uint16_t* target, uint16_t tb1)
{
	source = _WORD_to_BCD(source);
	*target = _WORD_to_BCD(*target);
	*target &= ~(((1 << ((sb2-sb1+1)*4)) - 1) << ((tb1-1)*4));
	*target |= ((source >> ((sb1-1) * 4))&((1 << ((sb2-sb1+1)*4)) - 1)) << ((tb1-1)*4);
	*target = _BCD_to_WORD(*target);
}

  
