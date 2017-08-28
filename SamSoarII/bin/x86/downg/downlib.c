#include "downlib.h"
#include <math.h>

// get a result (16-bit integer) by add a couple of 16-bit integers
int16_t _addw(int16_t ia, int16_t ib)
{
	// calculate the result
	int16_t ic = ia + ib;
	// M8169 : is overflowed
	MBit[8169] = (ia < 0 && ib < 0 && ic > 0);
	MBit[8169] |= (ia > 0 && ib > 0 && ic < 0);
	// M8170 : is less than 0
	MBit[8170] = (ic < 0);
	// M8171 : is 0
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (32-bit integer) by add a couple of 32-bit integers
int32_t _addd(int32_t ia, int32_t ib)
{
	int32_t ic = ia + ib;
	MBit[8169] = (ia < 0 && ib < 0 && ic > 0);
	MBit[8169] |= (ia > 0 && ib > 0 && ic < 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (32-bit float) by add a couple of 32-bit floats
float _addf(float ia, float ib)
{
	float ic = (ia + ib);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// get a result (16-bit integer) by sub a couple of 16-bit integers
int16_t _subw(int16_t ia, int16_t ib)
{
	int16_t ic = ia - ib;
	MBit[8169] = (ia > 0 && ib < 0 && ic < 0);
	MBit[8169] |= (ia < 0 && ib > 0 && ic > 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (32-bit integer) by sub a couple of 32-bit integers
int32_t _subd(int32_t ia, int32_t ib)
{
	int64_t ic = ia - ib;
	MBit[8169] = (ia > 0 && ib < 0 && ic < 0);
	MBit[8169] |= (ia < 0 && ib > 0 && ic > 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (32-bit float) by sub a couple of 32-bit floats
float _subf(float ia, float ib)
{
	float ic = ia - ib;
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// get a result (32-bit integer) by mul a couple of 16-bit integers
int32_t _mulwd(int16_t ia, int16_t ib)
{
	int32_t _ia = (int32_t)(ia);
	int32_t _ib = (int32_t)(ib);
	int32_t ic = _ia * _ib;
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (16-bit integer) by mul a couple of 16-bit integers
int16_t _mulww(int16_t ia, int16_t ib)
{
	int16_t ic = ia * ib;
	uint16_t _ia = (uint16_t)(ia < 0 ? -ia : ia);
	uint16_t _ib = (uint16_t)(ib < 0 ? -ib : ib);
	// get the maximum number in 64-bit integer
	uint16_t max = 0x3fff;
	MBit[8169] = (ia ? ((max / _ia) < _ib) : 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (32-bit integer) by mul a couple of 32-bit integers
int32_t _muldd(int32_t ia, int32_t ib)
{
	int32_t ic = ia * ib;
	uint32_t _ia = (uint32_t)(ia < 0 ? -ia : ia);
	uint32_t _ib = (uint32_t)(ib < 0 ? -ib : ib);
	// get the maximum number in 64-bit integer
	uint32_t max = 0x3fffffff;
	MBit[8169] = (ia ? ((max / _ia) < _ib) : 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (32-bit float) by mul a couple of 32-bit floats
float _mulff(float ia, float ib)
{
	float ic = ia * ib;
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// get a result data structure (store in 32-bit integer) by div a couple of 16-bit integers
// low 16-bit store the div result when high one store mod result.
int32_t _divwd(int16_t ia, int16_t ib)
{
	// if the one to divide is 0
	if (ib == 0)
	{
		MBit[8169] = 1;
		MBit[8170] = 0;
		MBit[8171] = 0;
		// M8172 : if divide in 0
		MBit[8172] = 1;
		return 0;
	}
	else
	{
		int32_t ic = 0;
		int16_t div = ia / ib;
		int16_t mod = ia % ib;
		ic = mod;
		ic <<= 16;
		ic |= div;
		MBit[8169] = 0;
		MBit[8170] = (div < 0);
		MBit[8171] = (div == 0);
		MBit[8172] = 0;
		return ic;
	}
}

// get a result (16-bit integer) by div a couple of 16-bit integers
int16_t _divww(int16_t ia, int16_t ib)
{
	if (ib == 0)
	{
		MBit[8169] = 1;
		MBit[8170] = 0;
		MBit[8171] = 0;
		MBit[8172] = 1;
		return 0;
	}
	else
	{
		int16_t ic = ia / ib;
		MBit[8169] = 0;
		MBit[8170] = (ic < 0);
		MBit[8171] = (ic == 0);
		MBit[8172] = 0;
		return ic;
	}
}

// get a result (32-bit integer) by div a couple of 32-bit integers
int32_t _divdd(int32_t ia, int32_t ib)
{
	if (ib == 0)
	{
		MBit[8169] = 1;
		MBit[8170] = 0;
		MBit[8171] = 0;
		MBit[8172] = 1;
		return 0;
	}
	else
	{
		int32_t ic = ia / ib;
		MBit[8169] = 0;
		MBit[8170] = (ic < 0);
		MBit[8171] = (ic == 0);
		MBit[8172] = 0;
		return ic;
	}
}

// get a result (32-bit float) by div a couple of 32-bit float
float _divff(float ia, float ib)
{
	if (ib == 0)
	{
		MBit[8169] = 1;
		MBit[8170] = 0;
		MBit[8171] = 0;
		MBit[8172] = 1;
		return 0;
	}
	else
	{
		float ic = ia / ib;
		MBit[8169] = 0;
		MBit[8170] = (ic < 0);
		MBit[8171] = (ic == 0);
		MBit[8172] = 0;
		return ic;
	}
}

// increase a 16-bit integer by 1
int16_t _incw(int16_t ia)
{
	int16_t ic = ia + 1;
	MBit[8169] = (ia > 0 && ic < 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// increase a 32-bit integer by 1
int32_t _incd(int32_t ia)
{
	int32_t ic = ia + 1;
	MBit[8169] = (ia > 0 && ic < 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// decrease a 16-bit integer by 1
int16_t _decw(int16_t ia)
{
	int16_t ic = ia - 1;
	MBit[8169] = (ia < 0 && ic > 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// decrease a 32-bit integer by 1
int32_t _decd(int32_t ia)
{
	int32_t ic = ia - 1;
	MBit[8169] = (ia < 0 && ic > 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// PI (The rate between radios and perimeter)
float PI = 3.1415926; 

// sin a 32-bit float (radian)
float _sin(float ia)
{
	float ic = sinf(ia * PI / 180);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// cos a 32-bit float
float _cos(float ia)
{
	float ic = cosf(ia * PI / 180);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// tan a 32-bit float
float _tan(float ia)
{
	float ic = tanf(ia * PI / 180);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// ln a 32-bit float
float _ln(float ia)
{
	float ic = logf(ia) / logf(exp(1.0));
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// exp a 32-bit float
float _exp(float ia)
{
	double ic = expf(ia);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return (float)(ic);
}

// Convert 32-Bit integer to 16-Bit integer 
int16_t _DWORD_to_WORD(int32_t ia)
{
	int16_t ic = (int16_t)(ia);
	MBit[8170] = ((ia >> 16) != 0);
	return ic;
}

// Convert 16-Bit integer to 32-Bit integer 
int32_t _WORD_to_DWORD(int16_t ia)
{
	int32_t ic = (int32_t)(ia);
	return ic;
}

// Convert 32-Bit integer to 32-Bit float
float _DWORD_to_FLOAT(int32_t ia)
{
	float ic = (float)(ia);
	return ic;
}

int32_t _round(float d)
{
	int32_t i = (int32_t)(d);
	if (d - i > 0.5) i++;
	return i;
}

// Convert 32-Bit float to 32-Bit integer by ROUND strategy
int32_t _FLOAT_to_ROUND(float ia)
{
	int32_t ic = _round(ia);
	return ic;
}

// Convert 64-Bit float to 64-Bit integer by TRUNC strategy
int32_t _FLOAT_to_TRUNC(float ia)
{
	int32_t ic = (int32_t)(ia);
	return ic;
}

// Convert 16-Bit integer to BCD code
int16_t _WORD_to_BCD(int16_t itg)
{
	// M8168 : BCD converting error
	MBit[8168] = (itg < 0 || itg > 9999);
	int16_t ret = (itg % 10) | ((itg / 10 % 10) << 4) | ((itg / 100 % 10) << 8) | ((itg / 1000 % 10) << 12);
	return ret;
}

// Convert BCD code to 16-Bit integer
int16_t _BCD_to_WORD(int16_t BCD)
{
	MBit[8168] = 0;
	MBit[8168] |= (((BCD >> 12) & 15) > 9);
	MBit[8168] |= (((BCD >> 8) & 15) > 9);
	MBit[8168] |= (((BCD >> 4) & 15) > 9);
	MBit[8168] |= ((BCD & 15) > 9);
	int16_t ret = ((BCD >> 12) & 15) * 1000 + ((BCD >> 8) & 15) * 100 + ((BCD >> 4) & 15) * 10 + (BCD & 15);
	return ret;
}

// shift 16-Bit integer to right
uint16_t _shrw(uint16_t ia, uint16_t ib)
{
	uint16_t ic = (ia >> ib);
	// M8166 : over bit 
	MBit[8166] = ib <= 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}

// shift 32-Bit integer to right
uint32_t _shrd(uint32_t ia, uint32_t ib)
{
	uint32_t ic = (ia >> ib);
	MBit[8166] = ib <= 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}

// shift 16-Bit integer to left
uint16_t _shlw(uint16_t ia, uint16_t ib)
{
	uint16_t ic = (ia << ib);
	MBit[8166] = ib <= 0 ? 0 : (((ia << (ib - 1)) & 0x8000) >> 15);
	MBit[8167] = (ic == 0);
	return ic;
}

// shift 32-Bit integer to left
uint32_t _shld(uint32_t ia, uint32_t ib)
{
	uint32_t ic = (ia << ib);
	MBit[8166] = ib <= 0 ? 0 : (((ia << (ib - 1)) & 0x80000000) >> 31);
	MBit[8167] = (ic == 0);
	return ic;
}

// rotate shift 16-bit integer to right
uint16_t _rorw(uint16_t ia, uint16_t ib)
{
	ib %= 16;
	uint16_t ic = (ia >> ib) | (ia << (16 - ib));
	MBit[8166] = ib == 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}


// rotate shift 64-bit integer to right
uint32_t _rord(uint32_t ia, uint32_t ib)
{
	ib %= 32;
	uint32_t ic = (ia >> ib) | (ia << (32 - ib));
	MBit[8166] = ib == 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}

// rotate shift 32-bit integer to left
uint16_t _rolw(uint16_t ia, uint16_t ib)
{
	ib %= 16;
	uint16_t ic = (ia << ib) | (ia >> (16 - ib));
	MBit[8166] = ib == 0 ? 0 : (((ia << (ib - 1)) & 0x8000) >> 15);
	MBit[8167] = (ic == 0);
	return ic;
}

// rotate shift 64-bit integer to right
uint32_t _rold(uint32_t ia, uint32_t ib)
{
	ib %= 32;
	uint64_t ic = (ia << ib) | (ia >> (32 - ib));
	MBit[8166] = ib == 0 ? 0 : (((ia << (ib - 1)) & 0x80000000) >> 31);
	MBit[8167] = (ic == 0);
	return ic;
}

// shift a series of BITs to left, and fill the blank block from input BITs (keep order)
void _bitshl(int32_t* input, int32_t* addr, int16_t size, int16_t move)
{
	while (--size >= move)
		addr[size] = addr[size - move];
	while (--size >= 0) 
		addr[size] = input[size];
}

// shift a series of BITs to right, and fill the blank block from input BITs (keep order)
void _bitshr(int32_t* input, int32_t* addr, int16_t size, int16_t move)
{
	int i = 0;
	while (i++ < size - move)
		addr[i] = addr[i + move];
	while (i++ < size)
		addr[i] = input[i - (size - move)];
}

/*
	Set a series of BIT to 1
	addr : base address
	size : series size
*/
void _bitset(int32_t* addr, int16_t size)
{
	while (--size >= 0)
		addr[size] = 1;
}

/*
	Reset a series of BIT to 0
	addr : base address
	size : series size
*/
void _bitrst(int32_t* addr, int16_t size)
{
	while (--size >= 0)
		addr[size] = 0;
}

/*
	Duplicate a series of BIT to the targeted memory space
	
	source_addr : the base address of the source datas
	target_addr : the base address of the targeted datas
	size : series size
*/
void _bitcpy(int32_t* source_addr, int32_t* target_addr, int16_t size)
{
	while (--size >= 0)
		target_addr[size] = source_addr[size];
}

// sub function type
typedef void(*vfun)(void);
// attach an interrupt function
void _atch(int id, vfun func) {}
// detach an interrupt function
void _dtch(int id) {}
// enable interrupt
void _ei() {}
// disable interrupt
void _di() {}
// try to invoke interrupt
void _itr_invoke() {}
// Get the system real time
void _trd(int16_t* d) {}
// Set the system real time
void _twr(int16_t* d) {}
// commuticate in COMPort and process the commands in Modbus table
void _mbus(uint16_t com_id, struct ModbusTable* table, int tlen, uint16_t* wr) {}
// commuticate in COMPort and send a series of memory to another
void _send(uint16_t com_id, int16_t* addr, int16_t len) {}
// commuticate in COMPort and reseive a series of memory from another
void _rev(uint16_t com_id, int16_t* addr, int16_t len) {}
// Create a pulse signal by the giving fequency parameter(in 32-bit integer)
void _plsf(uint16_t feq, int32_t* out) {}
// Create a pulse signal by the giving fequency parameter(in 64-bit integer)
void _dplsf(uint32_t feq, int32_t* out) {}
// Extra message : Duty cycle (32-bit integer)
void _pwm(uint16_t feq, uint16_t dc, int32_t* out) {}
// Extra message : Duty cycle (64-bit integer)
void _dpwm(uint32_t feq, uint32_t dc, int32_t* out) {}
// Extra message : Pulse number (32-bit integer)
void _plsy(uint16_t feq, uint16_t* pn, int32_t* out) {}
// Extra message : Pulse number (64-bit integer)
void _dplsy(uint32_t feq, uint32_t* pn, int32_t* out) {}
// Create a segment-divided and linear-faded pulse by 32-bit parameter
void _plsr(uint16_t* msg, int16_t ct, int32_t* out) {}
// Create a segment-divided and linear-faded pulse by 64-bit parameter
void _dplsr(uint32_t* msg, int32_t ct, int32_t* out) {}
// Create a segment-divided, linear-faded and oriented pulse by 32-bit parameter
void _plsrd(uint16_t* msg, int16_t ct, int32_t* out, int32_t* dir) {}
// Create a segment-divided, linear-faded and oriented pulse by 64-bit parameter
void _dplsrd(uint32_t* msg, int32_t ct, int32_t* out, int32_t* dir) {}
// Move to next segment
void _plsnext(int32_t* out) {}
// Stop generating signal
void _plsstop(int32_t* out) {}
// Pulse signal mountain (32-bit)
void _zrn(uint16_t bv, uint16_t cv, int16_t sg, int32_t* out) {}
// Pulse signal mountain (64-bit)
void _dzrn(uint32_t bv, uint32_t cv, int16_t sg, int32_t* out) {}
// Create a segment-divided pulse signal, with the linear fequency in the segment (64-bit)
void _pto(uint32_t* msg, int32_t* out, int32_t* dir) {}
// Set an unique segment, fade the current fequency to it and fulfill its pulse number (32-bit)
void _drvi(uint16_t feq, uint16_t pn, int32_t* out) {}
// Set an unique segment, fade the current fequency to it and fulfill its pulse number (64-bit)
void _ddrvi(uint32_t feq, uint32_t pn, int32_t* out) {}
// High counter
void _hcnt(int32_t* cv, int32_t sv) {}
// Calculate the log10
float _log(float ia) {}
// Calculate the power
float _pow(float ia, float ib) 
{
	float ic = pow(ia, ib);
	return ic;
}
// Calculate the factorial of 32-bit integer
int32_t _fact(int16_t ia) 
{
	int32_t ret = 1;
	while (ia > 1)
	{
		ret *= ia;
		ia--;
	}
	return ret;	
}

// Calculate the squarter of 32-bit float
float _sqrt(float ia)
{
	return sqrtf(ia);
}


// Compare a couple of 32-bit integers(named as ia and ib)
void _cmpw(int16_t ia, int16_t ib, int32_t* output) 
{
	// compare
	output[0] = (ia == ib);
	output[1] = (ia < ib);
	output[2] = (ia > ib);
}
// compare a couple of 64-bit integers(named as ia and ib)
void _cmpd(int32_t ia, int32_t ib, int32_t* output) 
{
	// compare
	output[0] = (ia == ib);
	output[1] = (ia < ib);
	output[2] = (ia > ib);
}
// compare a couple of 64-bit float(named as ia and ib)
void _cmpf(float ia, float ib, int32_t* output) 
{
	// compare
	output[0] = (ia == ib);
	output[1] = (ia < ib);
	output[2] = (ia > ib);
}
// check it if the targeted 32-bit integer is inside the range [il, ir] (16-bit)
void _zcpw(int16_t ia, int16_t il, int16_t ir, int32_t* output) 
{
	output[0] = (ia >= il && ia <= ir);
	output[1] = (ia < il);
	output[2] = (ia > ir);
}
// check it if the targeted 64-bit integer is inside the range [il, ir] (32-bit)
void _zcpd(int32_t ia, int32_t il, int32_t ir, int32_t* output) 
{
	output[0] = (ia >= il && ia <= ir);
	output[1] = (ia < il);
	output[2] = (ia > ir);
}
// check it if the targeted 64-bit float is inside the range [il, ir] (32-bit float)
void _zcpf(float ia, float il, float ir, int32_t* output) 
{
	output[0] = (ia >= il && ia <= ir);
	output[1] = (ia < il);
	output[2] = (ia > ir);
}
// make 32-bit integer ia negative 
int16_t _negw(int16_t ia) 
{
	return -ia;
}
// make 64-bit integer ia negative
int32_t _negd(int32_t ia) 
{
	return -ia;
}
// swap a couple of 16-bit integers
void _xchw(int16_t* pa, int16_t* pb) 
{
	int16_t _tempw = *pa;
	*pa = *pb;
	*pb = _tempw;
}
// swap a couple of 32-bit integer
void _xchd(int32_t* pa, int32_t* pb) 
{
	int32_t _tempd = *pa;
	*pa = *pb;
	*pb = _tempd;
}
// swap a couple of 32-bit float
void _xchf(float* pa, float* pb) 
{
	float _tempf = *pa;
	*pa = *pb;
	*pb = _tempf;
}
// binary reserve 32-bit integer ia 
int16_t _cmlw(int16_t ia) 
{	
	return ~ia;
}
// binary reserve 64-bit integer ia
int32_t _cmld(int32_t ia) 
{
	return ~ia;
}
// copy a series of 32-bit memory
void _smov(int16_t source, int16_t sb1, int16_t sb2, int16_t* target, int16_t tb1) 
{
	source = _WORD_to_BCD(source);
	*target = _WORD_to_BCD(*target);
	*target &= ~(((1 << ((sb2 - sb1 + 1) * 4)) - 1) << ((tb1 - 1) * 4));
	*target |= ((source >> ((sb1 - 1) * 4))&((1 << ((sb2 - sb1 + 1) * 4)) - 1)) << ((tb1 - 1) * 4);
	*target = _BCD_to_WORD(*target);
}
// copy a series of 64-bit memory
void _mvwblk(int16_t* source, int16_t* dest, int16_t size) 
{
	while (--size >= 0)
		dest[size] = source[size];
}
// move the fragment of a BCD code to anothor at the targeted position.
void _mvdblk(int32_t* source, int32_t* dest, int16_t size) 
{
	while (--size >= 0)
		dest[size] = source[size];
}
// set a series of 32-bit memeory to the targeted value
void _fmovw(int16_t source, int16_t* dest, int16_t size) 
{
	while (--size >= 0)
		dest[size] = source;
}
// set a series of 32-bit memeory to the targeted value
void _fmovd(int32_t source, int32_t* dest, int16_t size) 
{
	while (--size >= 0)
		dest[size] = source;
}

void _imyset(int16_t Yn, int16_t size)
{
	while (--size >= 0)
		OutputIm_Y(Yn+size, YBit[Yn+size]=1);
}

void _imyrst(int16_t Yn, int16_t size)
{
	while (--size >= 0)
		OutputIm_Y(Yn+size, YBit[Yn+size]=0);
}

void _set_wbit(int16_t* src, int16_t loc, int16_t size, int16_t value)
{
	int i;
	int lrem = 16 - loc;
	if (size < lrem)
	{
		if (value)
			src[0] |=  (((1<<size)-1) << loc);
		else
			src[0] &= ~(((1<<size)-1) << loc);
		return;
	}
	int bcnt = ((size - lrem) >> 4);
	int rrem = (size - lrem - (bcnt<<4));
	if (value)
		src[0] |=  (((1<<lrem)-1) << loc);
	else
		src[0] &= ~(((1<<lrem)-1) << loc);
	for (i = 1; i <= bcnt; i++)
		src[i] = value ? 0x0000ffff : 0x00000000;
	if (rrem)
	{
		if (value)
			src[bcnt+1] |=  ((1<<rrem)-1);
		else
			src[bcnt+1] &= ~((1<<rrem)-1);
	}
}

int32_t _get_wbit(int16_t* src, int16_t loc)
{
	return (*src>>loc) & 1;
}

void _mov_wbit_to_wbit(int16_t* src, int16_t sloc, int16_t* dst, int16_t dloc, int16_t size)
{
	int i;
	int slrem = 16 - sloc;
	int dlrem = 16 - dloc;
	if (size < slrem && size < dlrem)
	{
		dst[0] &= ~(((1<<size)-1) << dloc);
		dst[0] |= (((src[0]>>sloc) & ((1<<size)-1)) << dloc);
		return;
	}
	int lrem = slrem < dlrem ? slrem : dlrem;
	int delta = slrem + dlrem - (lrem<<1);
	int bcnt = ((size - lrem) >> 4);
	int rrem = (size - lrem - (bcnt<<4));
	dst[0] &= ~(((1<<lrem)-1) << dloc);
	dst[0] |= (((src[0]>>sloc) & ((1<<lrem)-1)) << dloc);
	for (i = 1; i < bcnt; i++)
	{
		if (slrem < dlrem)
		{
			dst[i-1] &= ~(((1<<delta)-1) << (16-delta));
			dst[i-1] |= ((src[i] & ((1<<delta)-1)) << (16-delta));
			dst[i] &= ~((1<<(16-delta))-1);
			dst[i] |= ((src[i]>>delta) & ((1<<(16-delta))-1));	
		}
		else
		{				
			dst[i] = (src[i] & ((1<<(16-delta))-1));
			dst[i] <<= delta;
			dst[i] = ((src[i-1]>>(16-delta)) & ((1<<delta)-1));
		}
	}
	if (rrem)
	{
		if (slrem < dlrem)
		{
			if (delta >= rrem)
			{						
				dst[bcnt] &= ~(((1<<rrem)-1) << (16-delta));
				dst[bcnt] |= ((src[bcnt+1] & ((1<<rrem)-1)) << (16-delta));
			}
			else
			{
				dst[bcnt] &= ~(((1<<delta)-1) << (16-delta));
				dst[bcnt] |= ((src[bcnt+1] & ((1<<delta)-1)) << (16-delta));
				dst[bcnt+1] &= ~((1<<(rrem-delta))-1);
				dst[bcnt+1] |= (((src[bcnt+1]>>delta) & ((1<<(rrem-delta))-1))); 
			}
		}
		else
		{
			if (delta >= rrem)
			{
				dst[bcnt] &= ~(((1<<rrem)-1) << (16-delta));
				dst[bcnt] |= ((src[bcnt+1] & ((1<<rrem)-1)) << (16-delta));
			}
			else
			{
				dst[bcnt] &= ~(((1<<delta)-1) << (16-delta));
				dst[bcnt] |= ((src[bcnt+1] & ((1<<delta)-1)) << (16-delta));
				dst[bcnt+1] &= ~((1<<(rrem-delta))-1);
				dst[bcnt+1] |= (((src[bcnt+1]>>delta) & ((1<<(rrem-delta))-1)));
			}
		}
	}
}

void _mov_wbit_to_bit(int16_t* src, int16_t sloc, int32_t* dst, int16_t size)
{
	int i, loc, bid;
	for (i = 0 ; i < size ; i++)
	{
		bid = ((sloc+i)>>4);
		loc = ((sloc+i)&15);
		dst[i] = ((src[bid]>>loc)&1);
	}
}

void _mov_bit_to_wbit(int32_t* src, int16_t* dst, int16_t dloc, int16_t size)
{
	int i, loc, bid;
	for (i = 0 ; i < size; i++)
	{
		bid = ((dloc+i)>>4);
		loc = ((dloc+i)&15);
		dst[bid] &= ~(1<<loc);
		dst[bid] |= (src[i]<<loc);
	}
}

void _shl_wbit_to_wbit(int16_t* src, int16_t sloc, int16_t* dst, int16_t dloc, int16_t size, int16_t move)
{	
	int bid1, bid2, loc1, loc2;
	while (--size >= move)
	{
		bid1 = ((dloc+size)>>4);
		bid2 = ((dloc+size-move)>>4);
		loc1 = ((dloc+size)&15);
		loc2 = ((dloc+size-move)&15);
		_set_wbit(dst+bid1, loc1, 1, _get_wbit(dst+bid2, loc2));
	}
	_mov_wbit_to_wbit(src, sloc, dst, dloc, move);
}

void _shl_wbit_to_bit(int16_t* src, int16_t sloc, int32_t* dst, int16_t size, int16_t move)
{
	while (--size >= move)
		dst[size] = dst[size - move];
	_mov_wbit_to_bit(src, sloc, dst, move);
}

void _shl_bit_to_wbit(int32_t* src, int16_t* dst, int16_t dloc, int16_t size, int16_t move)
{
	int bid1, bid2, loc1, loc2;
	while (--size >= move)
	{
		bid1 = ((dloc+size)>>4);
		bid2 = ((dloc+size-move)>>4);
		loc1 = ((dloc+size)&15);
		loc2 = ((dloc+size-move)&15);
		_set_wbit(dst+bid1, loc1, 1, _get_wbit(dst+bid2, loc2));
	}
	_mov_bit_to_wbit(src, dst, dloc, move);
}

void _shr_wbit_to_wbit(int16_t* src, int16_t sloc, int16_t* dst, int16_t dloc, int16_t size, int16_t move)
{
	int i = -1;
	int bid1, bid2, loc1, loc2;
	while (++i < size - move)
	{
		bid1 = ((dloc+i)>>4);
		bid2 = ((dloc+i+move)>>4);
		loc1 = ((dloc+i)&15);
		loc2 = ((dloc+i+move)&15);
		_set_wbit(dst+bid1, loc1, 1, _get_wbit(dst+bid2, loc2));
		i++;
	}
	bid1 = ((dloc+size-move)>>4);
	loc1 = ((dloc+size-move)&15);
	_mov_wbit_to_wbit(src, sloc, dst+bid1, loc1, move);
}

void _shr_wbit_to_bit(int16_t* src, int16_t sloc, int32_t* dst, int16_t size, int16_t move)
{
	int i = 0;
	while (i++ < size - move)
		dst[i] = dst[i + move];
	_mov_wbit_to_bit(src, sloc, dst+(size-move), move);
}

void _shr_bit_to_wbit(int32_t* src, int16_t* dst, int16_t dloc, int16_t size, int16_t move)
{
	int i = -1;
	int bid1, bid2, loc1, loc2;
	while (++i < size - move)
	{
		bid1 = ((dloc+i)>>4);
		bid2 = ((dloc+i+move)>>4);
		loc1 = ((dloc+i)&15);
		loc2 = ((dloc+i+move)&15);
		_set_wbit(dst+bid1, loc1, 1, _get_wbit(dst+bid2, loc2));
	}
	bid1 = ((dloc+size-move)>>4);
	loc1 = ((dloc+size-move)&15);
	_mov_bit_to_wbit(src, dst+bid1, loc1, move);
}

void _set_bword(int32_t* src, int16_t size, int16_t value)
{
	while (--size >= 0)
		src[size] = ((value>>size)&1);
}

int16_t _get_bword(int32_t* src, int16_t size)
{
	int16_t ret = 0;
	while (--size >= 0)
		ret = (ret<<1) + (src[size]&1);
	return ret;
}

void _set_bdword(int32_t* src, int16_t size, int32_t value)
{
	while (--size >= 0)
		src[size] = ((value>>size)&1);
}

int32_t _get_bdword(int32_t* src, int16_t size)
{
	int32_t ret = 0;
	while (--size >= 0)
		ret = (ret<<1) + (src[size]&1);
	return ret;
}

void _xch_bword_to_word(int32_t* bit, int16_t size, int16_t* word)
{
	int16_t tmp = *word;
	*word = _get_bword(bit, size);
	_set_bword(bit, size, tmp);
}

void _xch_bword_to_bword(int32_t* bit1, int32_t size1, int32_t* bit2, int32_t size2)
{
	int16_t tmp = _get_bword(bit1, size1);
	_set_bword(bit1, size1, _get_bword(bit2, size2));
	_set_bword(bit2, size2, tmp);
}

void _xchd_bdword_to_dword(int32_t* bit, int32_t size, int32_t* dword)
{
	int32_t tmp = *dword;
	*dword = _get_bdword(bit, size);
	_set_bdword(bit, size, tmp);
}

void _xchd_bdword_to_bdword(int32_t* bit1, int32_t size1, int32_t* bit2, int32_t size2)
{
	int32_t tmp = _get_bdword(bit1, size1);
	_set_bdword(bit1, size1, _get_bword(bit2, size2));
	_set_bdword(bit2, size2, tmp);
}

void _cmpw_wbit(int16_t ia, int16_t ib, int16_t* ic, int16_t loc)
{
	_set_wbit(ic, loc, 1, ia == ib);
	_set_wbit(ic+((loc+1)>>4), (loc+1)&15, 1, ia < ib);
	_set_wbit(ic+((loc+2)>>4), (loc+2)&15, 1, ia > ib);
}

void _cmpd_wbit(int32_t ia, int32_t ib, int16_t* ic, int16_t loc)
{
	_set_wbit(ic, loc, 1, ia == ib);
	_set_wbit(ic+((loc+1)>>4), (loc+1)&15, 1, ia < ib);
	_set_wbit(ic+((loc+2)>>4), (loc+2)&15, 1, ia > ib);
}

void _cmpf_wbit(float ia, float ib, int16_t* ic, int16_t loc)
{
	_set_wbit(ic, loc, 1, ia == ib);
	_set_wbit(ic+((loc+1)>>4), (loc+1)&15, 1, ia < ib);
	_set_wbit(ic+((loc+2)>>4), (loc+2)&15, 1, ia > ib);
}

void _zcpw_wbit(int16_t ia, int16_t il, int16_t ir, int16_t* out, int16_t loc) 
{
	_set_wbit(out, loc, 1, ia >= il && ia <= ir);
	_set_wbit(out+((loc+1)>>4), (loc+1)&15, 1, ia < il);
	_set_wbit(out+((loc+2)>>4), (loc+2)&15, 1, ia > ir);
}

void _zcpd_wbit(int32_t ia, int32_t il, int32_t ir, int16_t* out, int16_t loc) 
{
	_set_wbit(out, loc, 1, ia >= il && ia <= ir);
	_set_wbit(out+((loc+1)>>4), (loc+1)&15, 1, ia < il);
	_set_wbit(out+((loc+2)>>4), (loc+2)&15, 1, ia > ir);
}

void _zcpf_wbit(float ia, float il, float ir, int16_t* out, int16_t loc) 
{
	_set_wbit(out, loc, 1, ia >= il && ia <= ir);
	_set_wbit(out+((loc+1)>>4), (loc+1)&15, 1, ia < il);
	_set_wbit(out+((loc+2)>>4), (loc+2)&15, 1, ia > ir);
}
