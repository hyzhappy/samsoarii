#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <time.h>
#include <Windows.h>

#include "simulib.h"

// externed from simuc.h
// Bit number of WORD
int basebit;
// Register Memorys
int32_t XBit[128];
int32_t YBit[128];
int32_t MBit[256 << 5];
int32_t CBit[256];
int32_t TBit[256];
int32_t SBit[32 << 5];
int32_t DWord[8192];
int32_t CVWord[200];
int64_t CVDoubleWord[56];
int32_t TVWord[256];
int32_t AIWord[32];
int32_t AOWord[32];
int32_t VWord[8];
int32_t ZWord[8];
// Register writeable
// Set 1 if you want to modify the value of register
// othervise, Set 0 to lock the register and make it constant
int32_t XEnable[128];
int32_t YEnable[128];
int32_t MEnable[256 << 5];
int32_t CEnable[256];
int32_t TEnable[256];
int32_t SEnable[32 << 5];
int32_t DEnable[8192];
int32_t CVEnable[256];
int32_t TVEnable[256];
int32_t AIEnable[32];
int32_t AOEnable[32];
int32_t VEnable[8];
int32_t ZEnable[8];
// counttime
static int currenttime;
static int beforetime;
static int aftertime;
static int deltatime;
static int counttime;
static int counttimems;
static int timerate = 50;
static double innertimerate = 1.0;
static int scanperiod = 1000;
// temporary variable
static int32_t _tempw;
static int64_t _tempd;
static double _tempf;
// interrupt
static int32_t itr_enable;
static vfun itr_funcs[8];
static int32_t itr_times[2];
static int32_t itr_oldXBit[2];
// real timer
static struct tm *rtimer;
static struct tm _rtimer;
static time_t _time_t;
static time_t _time_t_old;
static time_t _time_t_new;
// pulse
int64_t YFeq[4];
static int32_t YTime[4];
static double YCount[4];
static uint64_t YTarget[4];
static int32_t YITime[4];
static int32_t YZStatus[4];
static int32_t YZTime[4];
static int64_t YZFeq[4];
static int64_t YPTarget[4];
static int64_t YDTime[4];
static uint64_t YDFeq[4];

int _Assert(
	char* name, int size, 
	void** addr, int* offset)
{
	int len = strlen(name);
	int div1 = 0;
	for ( ; div1 < len ; div1++)
	{
		if (!isalpha(name[div1])) 
		{
			if (!isdigit(name[div1]))
				return 1;
			if (div1 == 0)
				return 2;
			break;
		}
	}
	if (div1 >= len)
		return 1;
	int div2 = div1;
	for ( ; div2 < len ; div2++)
	{
		if (!isdigit(name[div2]))
		{
			if (!isalpha(name[div2]))
				return 1;
			if (div2 == div1)
				return 2;
			break;
		}
	}
	sscanf(name + div1, "%d", offset);
	if (div2 < len)
	{
		int div3 = div2;
		for ( ; div3 < len ; div3++)
		{
			if (!isalpha(name[div3]))
			{
				if (!isdigit(name[div3]))
					return 1;
				if (div3 == div2)
					return 2;
				break;
			}
		}
		if (div3 >= len)
			return 1;
		int div4 = div3;
		for ( ; div4 < len ; div4++)
		{
			if (!isdigit(name[div4]))
			{
				if (!isalpha(name[div4]))
					return 1;
				if (div4 == div3)
					return 2;
			}
		}
		if (div4 < len)
			return 2;
		int vzoffset = 0;
		sscanf(name + div3, "%d", &vzoffset);
		if (vzoffset < 0 || vzoffset >= 8)
			return 4;
		switch (div3 - div2)
		{
			case 1:
				switch (name[div2])
				{
					case 'V': *offset += VWord[vzoffset]; break;
					case 'Z': *offset -= ZWord[vzoffset]; break;
					default: return 3;
				}
				break;
			default:
				return 3;
		}
	}
	switch (div1)
	{
		case 1:
			switch (name[0])
			{
				case 'X': 
					*addr = XBit;
					if (*offset < 0 || *offset + size > 128)
						return 4;
					break;
				case 'Y': 
					*addr = YBit; 
					if (*offset < 0 || *offset + size > 128)
						return 4;
					break;
				case 'C': 
					*addr = CBit; 
					if (*offset < 0 || *offset + size > 256)
						return 4;
					break;
				case 'T': 
					*addr = TBit; 
					if (*offset < 0 || *offset + size > 256)
						return 4;
					break;
				case 'S': 
					*addr = SBit; 
					if (*offset < 0 || *offset + size > 1024)
						return 4;
					break;
				case 'M': 
					*addr = MBit; 
					if (*offset < 0 || *offset + size > 8192)
						return 4;
					break;
				case 'D': 
					*addr = DWord; 
					if (*offset < 0 || *offset + size > 8192)
						return 4;
					break;
				case 'V': 
					*addr = VWord; 
					if (*offset < 0 || *offset + size > 8)
						return 4;
					break;
				case 'Z': 
					*addr = ZWord; 
					if (*offset < 0 || *offset + size > 8)
						return 4;
					break;
				default: 
					return 3;
			}
			break;
		case 2:
			if (name[0] == 'C' && name[1] == 'V')
			{
				if (*offset >= 0 && *offset < 200)
				{
					*addr = CVWord;
				}
				else if (*offset >= 200 || *offset + size < 256)
				{
					*addr = CVDoubleWord;
					*offset -= 200;
				}
				else
				{
					return 4;
				}
			}
			if (name[0] == 'T' && name[1] == 'V')
			{
				*addr = TVWord;
				if (*offset < 0 || *offset + size > 256)
					return 4;
			}
			if (name[0] == 'A' && name[1] == 'I')
			{
				*addr = AIWord;
				if (*offset < 0 || *offset + size > 32)
					return 4;
			}
			if (name[0] == 'A' && name[1] == 'O')
			{
				*addr = AOWord;
				if (*offset < 0 || *offset + size > 32)
					return 4;
			}
			break;
		default:
			return 3;
	}
	return 0;
}
// Get the BIT value from targeted bit register (X/Y/M/C/T/S) 
EXPORT int GetBit(char* name, int size, uint32_t* output)
{
	int32_t* addr; int offset;
	int ret = _Assert(name, size, (void**)(&addr), &offset);
	if (ret) return ret;
	memcpy(output, addr + offset, size * sizeof(uint32_t));
	return 0;
}
// Get the WORD value from targeted register (D/CV/TV)
EXPORT int GetWord(char* name, int size, uint32_t* output)
{
	int32_t* addr; int offset;
	int ret = _Assert(name, size, (void**)(&addr), &offset);
	if (ret) return ret;
	memcpy(output, addr + offset, size * sizeof(uint32_t));
	return 0;
}
// Get the DWORD (32 bit unsigned int) value from targeted register (D/CV32)
EXPORT int GetDoubleWord(char* name, int size, uint64_t* output)
{
	int64_t* addr; int offset;
	int ret = _Assert(name, size*2, (void**)(&addr), &offset);
	if (ret) 
	{
		if (addr != CVDoubleWord
		 || _Assert(name, size, (void**)(&addr), &offset))
		{
			return ret;
		}
	}
	memcpy(output, addr + offset, size * sizeof(uint64_t));
	return 0;
}
// Get the FLOAT value from targeted register (D)
EXPORT int GetFloat(char* name, int size, double* output)
{
	double* addr; int offset;
	int ret = _Assert(name, size*2, (void**)(&addr), &offset);
	if (ret) return ret;
	memcpy(output, addr + offset, size * sizeof(double));
	return 0;
}
// Get the signal frequency
EXPORT int GetFeq(char* name, uint64_t* output)
{
	void* addr; int offset;
	int ret = _Assert(name, 1, (void**)(&addr), &offset);
	if (ret) return ret;
	if (offset > 4) return 4;
	*output = YFeq[offset];
	return 0;
}
// Set the signal frequency
EXPORT int SetFeq(char* name, uint64_t input)
{
	void* addr; int offset;
	int ret = _Assert(name, 1, (void**)(&addr), &offset);
	if (ret) return ret;
	if (offset > 4) return 4;
	YFeq[offset] = input;
	return 0;
}
// Set the Bit value to targeted bit register (X/Y/M/C/T/S)
EXPORT int SetBit(char* name, int size, uint32_t* input)
{
	int32_t* addr; int offset;
	int ret = _Assert(name, size, (void**)(&addr), &offset);
	if (ret) return ret;
	memcpy(addr + offset, input, size * sizeof(uint32_t));
	return 0;
}
// Set the WORD value to targeted register (D/CV/TV)
EXPORT int SetWord(char* name, int size, uint32_t* input)
{
	int32_t* addr; int offset;
	int ret = _Assert(name, size, (void**)(&addr), &offset);
	if (ret) return ret;
	memcpy(addr + offset, input, size * sizeof(uint32_t));
	return 0;
}
// Set the DWORD value to targeted register (D)
EXPORT int SetDoubleWord(char* name, int size, uint64_t* input)
{
	int64_t* addr; int offset;
	int ret = _Assert(name, size, (void**)(&addr), &offset);
	if (ret) 
	{
		if (addr != CVDoubleWord
		 || _Assert(name, size, (void**)(&addr), &offset))
		{
			return ret;
		}
	}
	memcpy(addr + offset, input, size * sizeof(uint64_t));
	return 0;
}
// Set the FLOAT value to targeted register (D)
EXPORT int SetFloat(char* name, int size, double* input)
{
	double* addr; int offset;
	int ret = _Assert(name, size, (void**)(&addr), &offset);
	if (ret) return ret;
	memcpy(addr + offset, input, size * sizeof(double));
	return 0;
}
// Set the writeable enable value of targeted register
EXPORT void SetEnable(char* name, int size, int value)
{
	int addr = 0;
	switch (name[0])
	{
	case 'X':
		sscanf(name + 1, "%d", &addr);
		while (size--) XEnable[addr + size] = value;
		break;
	case 'Y':
		sscanf(name + 1, "%d", &addr);
		while (size--) YEnable[addr + size] = value;
		break;
	case 'M':
		sscanf(name + 1, "%d", &addr);
		while (size--) MEnable[addr + size] = value;
		break;
	case 'S':
		sscanf(name + 1, "%d", &addr);
		while (size--) SEnable[addr + size] = value;
		break;
	case 'D':
		sscanf(name + 1, "%d", &addr);
		while (size--) DEnable[addr + size] = value;
		break;
	case 'V':
		sscanf(name + 1, "%d", &addr);
		while (size--) VEnable[addr + size] = value;
		break;
	case 'Z':
		sscanf(name + 1, "%d", &addr);
		while (size--) ZEnable[addr + size] = value;
		break;
	case 'A':
		switch (name[1])
		{
		case 'I':
			sscanf(name + 2, "%d", &addr);
			while (size--) AIEnable[addr + size] = value;
			break;
		case 'O':
			sscanf(name + 2, "%d", &addr);
			while (size--) AOEnable[addr + size] = value;
			break;
		}
	case 'C':
		switch (name[1])
		{
		case 'V':
			sscanf(name + 2, "%d", &addr);
			while (size--) CVEnable[addr + size] = value;
			break;
		default:
			sscanf(name + 1, "%d", &addr);
			while (size--) CEnable[addr + size] = value;
			break;
		}
		break;
	case 'T':
		switch (name[1])
		{
		case 'V':
			sscanf(name + 2, "%d", &addr);
			while (size--) TVEnable[addr + size] = value;
			break;
		default:
			sscanf(name + 1, "%d", &addr);
			while (size--) TEnable[addr + size] = value;
			break;
		}
		break;
	}
}

EXPORT void SetBaseBit(int _basebit)
{
	basebit = _basebit;
}

EXPORT void InitClock(int _counttimems)
{
	counttimems = _counttimems;
	counttime = counttimems * 1000;
}

void UpdateClock()
{
	LARGE_INTEGER lpCount;
	QueryPerformanceCounter(&lpCount);
	currenttime = (int)(lpCount.QuadPart / timerate);
}

EXPORT int GetClock()
{
	return counttimems;
}

EXPORT void SetClockRate(int _timerate)
{
	timerate = _timerate;
}

int32_t bpenable = 1;
int32_t bpdatas[1<<13];
int32_t bpaddr;
int32_t bpstep = 0;
int32_t bpjump = -1;
int32_t bpostep = 0;
int32_t bpcstep = 0;
int32_t bppause = 0;
int32_t cpdatas[1<<16];
int32_t cpstack[1<<8];
int32_t cpsttop = 0;
int32_t cpenable;
int32_t callcount = 0;

void callinto()
{
	callcount++;
	bpostep = bpstep;
	bpstep = 0;
	if (bpenable && callcount > 256)
	{
		bppause = 1;
		while (bpenable && bppause) ;
	}
}

void callleave()
{
	callcount--;
	bpstep |= bpostep;
}

void bpcycle(int32_t _bpaddr)
{
	bpaddr = _bpaddr;
	if (!bpenable) return;
	if (bpstep
	|| bpcstep
	|| bpjump < 0 && (bpdatas[bpaddr>>5] & (1<<(bpaddr&31)))
	|| bpjump == bpaddr)
	{
		bpjump = -1;
		bpstep = 0;
		bpcstep = 0;
		bppause = 1;
		while (bpenable && bppause) ;
	}
}

void cpcycle(int32_t _bpaddr, int32_t value)
{
	if (!bpenable || !cpenable) return;
	bpaddr = _bpaddr;
	int32_t cpmsg = ((cpdatas[bpaddr>>3]>>((bpaddr&7)<<2)) & 7);
	if (cpmsg <= 0) return;
	int32_t prevalue = cpstack[cpsttop++];
	int32_t cond = 0;
	if (cpmsg & 0x01)
		cond |= (value == 0);
	if (cpmsg & 0x02)
		cond |= (value == 1);
	if (cpmsg & 0x04)
		cond |= (prevalue == 0 && value == 1);
	if (cpmsg & 0x08)
		cond |= (prevalue == 1 && value == 0);
	if (bpjump >= 0)
		cond &= (bpjump == bpaddr);
	if (cond)
	{
		bpjump = -1;
		bppause = 2;
		while (bpenable && bppause);	
	}
}

EXPORT int GetCallCount()
{
	return callcount;
}

EXPORT int GetBPAddr()
{
	return bpaddr;
}

EXPORT void SetBPAddr(int32_t _bpaddr, int32_t islock)
{
	if (islock)
		bpdatas[_bpaddr>>5] |= (1<<(_bpaddr&31));
	else
		bpdatas[_bpaddr>>5] &= ~(1<<(_bpaddr&31));
}

EXPORT void SetBPEnable(int _bpenable)
{
	bpenable = _bpenable;
}
	
EXPORT int GetBPPause()
{
	return bppause;
}

EXPORT void SetBPPause(int _bppause)
{
	bppause = _bppause;
}

EXPORT void MoveStep()
{
	bpstep = 1;
	bppause = 0;
}

EXPORT void CallStep()
{
	bpcstep = 1;
	bppause = 0;
}

EXPORT void JumpTo(int _bpaddr)
{
	bpjump = _bpaddr;
}

EXPORT void JumpOut()
{
	bpostep = 1;
	bppause = 0;
}

EXPORT void SetCPAddr(int32_t _cpaddr, int32_t _cpmsg)
{
	cpdatas[_cpaddr>>3] &= ~(15<<((_cpaddr&7)<<2));
	cpdatas[_cpaddr>>3] |= _cpmsg<<((_cpaddr&7)<<2);
	cpenable = 0;
}

void InitRegisters()
{
	memset(XBit, 0, sizeof(XBit));
	memset(YBit, 0, sizeof(YBit));
	memset(SBit, 0, sizeof(SBit));
	memset(MBit, 0, sizeof(MBit));
	memset(TBit, 0, sizeof(TBit));
	memset(CBit, 0, sizeof(CBit));
	memset(DWord, 0, sizeof(DWord));
	memset(CVWord, 0, sizeof(CVWord));
	memset(TVWord, 0, sizeof(TVWord));
	memset(CVDoubleWord, 0, sizeof(CVDoubleWord));
	
	MBit[8151] = 0;
	MBit[8152] = 1;
}

EXPORT void BeforeRunLadder()
{
	cpsttop = 0;
	cpenable = 1;
	
	UpdateClock();
	innertimerate = (double)(currenttime - beforetime) / deltatime;
	beforetime = currenttime;
	
	MBit[8158] = ((counttimems / 5) & 1);
	MBit[8159] = ((counttimems / 50) & 1);
	MBit[8160] = ((counttimems / 500) & 1);
	MBit[8161] = ((counttimems / 30000) & 1);
}

EXPORT void AfterRunLadder()
{
	UpdateClock();
	aftertime = currenttime;
	deltatime = aftertime - beforetime;
	counttime += (int32_t)(deltatime * innertimerate);
	counttimems = counttime / 1000;
}

EXPORT void InitRunLadder()
{
	InitRegisters();
	InitClock(0);
	BeforeRunLadder();
	RunLadder();
	AfterRunLadder();
	MBit[8151] = 1;
	MBit[8152] = 0;
}

// get a result (32-bit integer) by add a couple of 32-bit integers
int32_t _addw(int32_t ia, int32_t ib)
{
	// calculate the result
	int32_t ic = ia + ib;
	ic <<= 32 - basebit;
	ic >>= 32 - basebit;
	// convert to unsigned format
	uint32_t _ia = (uint32_t)(ia);
	uint32_t _ib = (uint32_t)(ib);
	uint32_t _ic = (uint32_t)(ic);
	// M8169 : is overflowed
	MBit[8169] = (_ic < _ia + _ib);
	// M8170 : is less than 0
	MBit[8170] = (ic < 0);
	// M8171 : is 0
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (64-bit integer) by add a couple of 64-bit integers
int64_t _addd(int64_t ia, int64_t ib)
{
	int64_t ic = ia + ib;
	ic <<= 64 - basebit * 2;
	ic >>= 64 - basebit * 2;
	uint64_t _ia = (uint64_t)(ia);
	uint64_t _ib = (uint64_t)(ib);
	uint64_t _ic = (uint64_t)(ic);
	MBit[8169] = (_ic < _ia + _ib);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (64-bit float) by add a couple of 64-bit floats
double _addf(double ia, double ib)
{
	double ic = (ia + ib);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// get a result (32-bit integer) by sub a couple of 32-bit integers
int32_t _subw(int32_t ia, int32_t ib)
{
	int32_t ic = ia - ib;
	ic <<= 32 - basebit;
	ic >>= 32 - basebit;
	uint32_t _ia = (uint32_t)(ia);
	uint32_t _ib = (uint32_t)(ib);
	uint32_t _ic = (uint32_t)(ic);
	MBit[8169] = (_ia < _ib + _ic);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (64-bit integer) by sub a couple of 64-bit integers
int64_t _subd(int64_t ia, int64_t ib)
{
	int64_t ic = ia - ib;
	ic <<= 64 - basebit * 2;
	ic >>= 64 - basebit * 2;
	uint64_t _ia = (uint64_t)(ia);
	uint64_t _ib = (uint64_t)(ib);
	uint64_t _ic = (uint64_t)(ic);
	MBit[8169] = (_ic < _ia + _ib);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (64-bit float) by sub a couple of 64-bit floats
double _subf(double ia, double ib)
{
	double ic = ia - ib;
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// get a result (64-bit integer) by mul a couple of 32-bit integers
int64_t _mulwd(int32_t ia, int32_t ib)
{
	int64_t _ia = (int64_t)(ia);
	int64_t _ib = (int64_t)(ib);
	int64_t ic = _ia * _ib;
	int32_t ich = (ic >> 32);
	int32_t icl = (ic & 0xffffffff);
	ich <<= 32 - basebit;
	ich >>= 32 - basebit;
	icl <<= 32 - basebit;
	icl >>= 32 - basebit;
	ic = ((((int64_t)(ich)) << 32) | icl);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (32-bit integer) by mul a couple of 32-bit integers
int32_t _mulww(int32_t ia, int32_t ib)
{
	int32_t ic = ia * ib;
	ic <<= 32 - basebit;
	ic >>= 32 - basebit;
	uint32_t _ia = (uint32_t)(ia < 0 ? -ia : ia);
	uint32_t _ib = (uint32_t)(ib < 0 ? -ib : ib);
	// get the maximum number in 64-bit integer
	uint32_t max = 0x3fffffff;
	MBit[8169] = (ia ? (max / _ia < _ib) : 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (64-bit integer) by mul a couple of 64-bit integers
int64_t _muldd(int64_t ia, int64_t ib)
{
	int64_t ic = ia * ib;
	ic <<= 64 - basebit * 2;
	ic >>= 64 - basebit * 2;
	uint64_t _ia = (uint32_t)(ia < 0 ? -ia : ia);
	uint64_t _ib = (uint32_t)(ib < 0 ? -ib : ib);
	// get the maximum number in 64-bit integer
	uint64_t max = 0x3fffffffffffffffL;
	MBit[8169] = (ia ? (max / _ia < _ib) : 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (64-bit float) by mul a couple of 64-bit floats
double _mulff(double ia, double ib)
{
	double ic = ia * ib;
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// get a result data structure (store in 64-bit integer) by div a couple of 32-bit integers
// low 32-bit store the div result when high one store mod result.
int64_t _divwd(int32_t ia, int32_t ib)
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
		int64_t ic = 0;
		int32_t div = ia / ib;
		int32_t mod = ia % ib;
		div <<= 32 - basebit;
		div >>= 32 - basebit;
		mod <<= 32 - basebit;
		mod >>= 32 - basebit;
		ic = mod;
		ic <<= 32;
		ic |= div;
		MBit[8169] = 0;
		MBit[8170] = (div < 0);
		MBit[8171] = (div == 0);
		MBit[8172] = 0;
		return ic;
	}
}

// get a result (32-bit integer) by div a couple of 32-bit integers
int32_t _divww(int32_t ia, int32_t ib)
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
		ic <<= 32 - basebit;
		ic >>= 32 - basebit;
		MBit[8169] = 0;
		MBit[8170] = (ic < 0);
		MBit[8171] = (ic == 0);
		MBit[8172] = 0;
		return ic;
	}
}

// get a result (64-bit integer) by div a couple of 64-bit integers
int64_t _divdd(int64_t ia, int64_t ib)
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
		int64_t ic = ia / ib;
		ic <<= 64 - basebit * 2;
		ic >>= 64 - basebit * 2;
		MBit[8169] = 0;
		MBit[8170] = (ic < 0);
		MBit[8171] = (ic == 0);
		MBit[8172] = 0;
		return ic;
	}
}

// increase a 32-bit integer by 1
int32_t _incw(int32_t ia)
{
	int32_t ic = ia + 1;
	ic <<= 32 - basebit;
	ic >>= 32 - basebit;
	MBit[8169] = (ia > 0 && ic < 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// increase a 64-bit integer by 1
int64_t _incd(int64_t ia)
{
	int64_t ic = ia + 1;
	ic <<= 32 - basebit;
	ic >>= 32 - basebit;
	MBit[8169] = (ia > 0 && ic < 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// decrease a 32-bit integer by 1
int32_t _decw(int32_t ia)
{
	int32_t ic = ia - 1;
	ic <<= 32 - basebit;
	ic >>= 32 - basebit;
	MBit[8169] = (ia < 0 && ic > 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// decrease a 64-bit integer by 1
int64_t _decd(int64_t ia)
{
	int64_t ic = ia - 1;
	ic <<= 32 - basebit;
	ic >>= 32 - basebit;
	MBit[8169] = (ia < 0 && ic > 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// PI (The rate between radios and perimeter)
double PI = 3.1415926; 

// sin a 64-bit float (radian)
double _sin(double ia)
{
	double ic = sin(ia * PI / 180);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// cos a 64-bit float
double _cos(double ia)
{
	double ic = cos(ia * PI / 180);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// tan a 64-bit float
double _tan(double ia)
{
	double ic = tan(ia * PI / 180);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// ln a 64-bit float
double _ln(double ia)
{
	double ic = log(ia) / log(exp(1.0));
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// exp a 64-bit float
double _exp(double ia)
{
	double ic = exp(ia);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// Convert 64-Bit integer to 32-Bit integer 
int32_t _DWORD_to_WORD(int64_t ia)
{
	int32_t ic = (int32_t)(ia);
	ic <<= 32 - basebit;
	ic >>= 32 - basebit;
	MBit[8170] = ((ia >> 32) != 0);
	return ic;
}

// Convert 32-Bit integer to 64-Bit integer 
int64_t _WORD_to_DWORD(int32_t ia)
{
	int64_t ic = (int64_t)(ia);
	ic <<= 64 - basebit * 2;
	ic >>= 64 - basebit * 2;
	return ic;
}

// Convert 64-Bit integer to 64-Bit float
double _DWORD_to_FLOAT(int64_t ia)
{
	double ic = (double)(ia);
	return ic;
}

int64_t _round(double d)
{
	int64_t i = (int64_t)(d);
	if (d - i > 0.5) i++;
	return i;
}

// Convert 64-Bit float to 64-Bit integer by ROUND strategy
int64_t _FLOAT_to_ROUND(double ia)
{
	int64_t ic = _round(ia);
	ic <<= 64 - basebit * 2;
	ic >>= 64 - basebit * 2;
	return ic;
}

// Convert 64-Bit float to 64-Bit integer by TRUNC strategy
int64_t _FLOAT_to_TRUNC(double ia)
{
	int64_t ic = (int64_t)(ia);
	ic <<= 64 - basebit * 2;
	ic >>= 64 - basebit * 2;
	return ic;
}

// Convert 32-Bit integer to BCD code
int32_t _WORD_to_BCD(int32_t itg)
{
	// M8168 : BCD converting error
	MBit[8168] = (itg < 0 || itg > 9999);
	int32_t ret = (itg % 10) | ((itg / 10 % 10) << 4) | ((itg / 100 % 10) << 8) | ((itg / 1000 % 10) << 12);
	ret <<= 32 - basebit;
	ret >>= 32 - basebit;
	return ret;
}

// Convert BCD code to 32-Bit integer
int32_t _BCD_to_WORD(int32_t BCD)
{
	MBit[8168] = 0;
	MBit[8168] |= (((BCD >> 12) & 15) > 9);
	MBit[8168] |= (((BCD >> 8) & 15) > 9);
	MBit[8168] |= (((BCD >> 4) & 15) > 9);
	MBit[8168] |= ((BCD & 15) > 9);
	int32_t ret = ((BCD >> 12) & 15) * 1000 + ((BCD >> 8) & 15) * 100 + ((BCD >> 4) & 15) * 10 + (BCD & 15);
	ret <<= 32 - basebit;
	ret >>= 32 - basebit;
	return ret;
}

// shift 32-Bit integer to right
uint32_t _shrw(uint32_t ia, uint32_t ib)
{
	uint32_t ic = (ia >> ib);
	if (basebit < 32)
		ic &= (1 << basebit) - 1;
	// M8166 : over bit 
	MBit[8166] = ib <= 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}

// shift 64-Bit integer to right
uint64_t _shrd(uint64_t ia, uint64_t ib)
{
	uint64_t ic = (ia >> ib);
	if (basebit < 32)
		ic &= (1L << (basebit*2)) - 1;
	MBit[8166] = ib <= 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}

// shift 32-Bit integer to left
uint32_t _shlw(uint32_t ia, uint32_t ib)
{
	uint32_t ic = (ia << ib);
	if (basebit < 32)
		ic &= (1 << basebit) - 1;
	MBit[8166] = ib <= 0 ? 0 : (((ia << (ib - 1)) & 0x80000000) >> 31);
	MBit[8167] = (ic == 0);
	return ic;
}

// shift 64-Bit integer to left
uint64_t _shld(uint64_t ia, uint64_t ib)
{
	uint64_t ic = (ia << ib);
	if (basebit < 32)
		ic &= (1L << (basebit*2)) - 1;
	MBit[8166] = ib <= 0 ? 0 : (((ia << (ib - 1)) & 0x8000000000000000L) >> 63);
	MBit[8167] = (ic == 0);
	return ic;
}

// rotate shift 32-bit integer to right
uint32_t _rorw(uint32_t ia, uint32_t ib)
{
	ib %= 32;
	uint32_t ic = (ia >> ib) | (ia << (32 - ib));
	if (basebit < 32)
		ic &= (1 << basebit) - 1;
	MBit[8166] = ib == 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}


// rotate shift 64-bit integer to right
uint64_t _rord(uint64_t ia, uint64_t ib)
{
	ib %= 64;
	uint64_t ic = (ia >> ib) | (ia << (64 - ib));
	if (basebit < 32)
		ic &= (1L << (basebit*2)) - 1;
	MBit[8166] = ib == 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}

// rotate shift 32-bit integer to left
uint32_t _rolw(uint32_t ia, uint32_t ib)
{
	ib %= 32;
	uint32_t ic = (ia << ib) | (ia >> (32 - ib));
	if (basebit < 32)
		ic &= (1 << basebit) - 1;
	MBit[8166] = ib == 0 ? 0 : (((ia << (ib - 1)) & 0x80000000) >> 31);
	MBit[8167] = (ic == 0);
	return ic;
}

// rotate shift 64-bit integer to right
uint64_t _rold(uint64_t ia, uint64_t ib)
{
	ib %= 32;
	uint64_t ic = (ia << ib) | (ia >> (64 - ib));
	if (basebit < 32)
		ic &= (1L << (basebit*2)) - 1;
	MBit[8166] = ib == 0 ? 0 : (((ia << (ib - 1)) & 0x8000000000000000L) >> 63);
	MBit[8167] = (ic == 0);
	return ic;
}

// shift a series of BITs to left, and fill the blank block from input BITs (keep order)
void _bitshl(int32_t* input, int32_t* addr, int32_t* enable, int32_t size, int32_t move)
{
	while (--size >= move)
	{
		if (!enable[size])
			addr[size] = addr[size - move];
	}
	while (--size >= 0) {
		if (!enable[size])
			addr[size] = input[size];
	}
}

// shift a series of BITs to right, and fill the blank block from input BITs (keep order)
void _bitshr(int32_t* input, int32_t* addr, int32_t* enable, int32_t size, int32_t move)
{
	int i = 0;
	while (i++ < size - move)
	{
		if (!enable[i])
			addr[i] = addr[i + move];
	}
	while (i++ < size) {
		if (!enable[i])
			addr[i] = input[i - (size - move)];
	}
}

/*
	Set a series of BIT to 1
	addr : base address
	size : series size
*/
void _bitset(int32_t* addr, int32_t* enable, int32_t size)
{
	while (--size >= 0)
	{
		if (!enable[size]) addr[size] = 1;
	}
}

/*
	Reset a series of BIT to 0
	addr : base address
	size : series size
*/
void _bitrst(int32_t* addr, int32_t* enable, int32_t size)
{
	while (--size >= 0)
	{
		if (!enable[size]) addr[size] = 0;
	}
}

/*
	Duplicate a series of BIT to the targeted memory space
	
	source_addr : the base address of the source datas
	target_addr : the base address of the targeted datas
	size : series size
*/
void _bitcpy(int32_t* source_addr, int32_t* target_addr, int32_t* enable, int32_t size)
{
	while (--size >= 0)
	{
		if (!enable[size])
			target_addr[size] = source_addr[size];
	}
}

// Time counter
// tval : counter value
// tbit : the position of the counter bit in the block
// cod  : input condition
// sv   : reserve value
// otim : old time(ms)
void _ton(int32_t* tval, int32_t* tbit, int32_t cod, int32_t sv, int32_t* otim)
{
	// if condition is TRUE
	if (cod)
	{
		// get the new time
		int32_t ntim = counttimems;
		// get the inteval from old to new
		int32_t itv = (ntim - *otim) / 100;
		// increase the counter value and old time
		*tval += itv;
		*otim += itv * 100;
		// arrive at the end value?
		if (*tval >= sv)
		{
			// keep it no more than end value
			*tval = sv;
			// set the counter bit
			*tbit = 1;
		}
	}
	else
	{
		// reset the counter value
		*tval = 0;
		// reset the counter bit to zero
		*tbit = 0;
		// update the old time
		*otim = counttimems;
	}
}

// Time counter (opened preservation)
// tval : counter value
// tbit : the position of the counter bit in the block
// cod  : input condition
// sv   : reserve value
// otim : old time(ms)
void _tonr(int32_t* tval, int32_t* tbit, int32_t cod, int32_t sv, int32_t* otim)
{
	// if condition is TRUE
	if (cod)
	{
		// get the new time
		int32_t ntim = counttimems;
		// get the inteval from old to new
		int32_t itv = (ntim - *otim) / 100;
		// increase the counter value and old time
		*tval += itv;
		*otim += itv * 100;
		// arrive at the reserve value?
		if (*tval >= sv)
		{
			// keep it no more than end value
			*tval = sv;
			// set the counter bit
			*tbit = 1;
		}
	}
	else
	{
		// update the old time
		*otim = counttimems;
	}
}

// attach an interrupt function
void _atch(int id, vfun func)
{
	if (id >= 6 && itr_funcs[id] != func) 
	{
		itr_times[id - 6] = counttimems;
	}
	itr_funcs[id] = func;
}

// detach an interrupt function
void _dtch(int id)
{
	itr_funcs[id] = NULL;
}

// enable interrupt
void _ei()
{
	itr_enable = 1;
}

// disable interrupt
void _di()
{
	itr_enable = 0;
}

// try to invoke interrupt
void _itr_invoke()
{
	if (!itr_enable)
		return;
	int32_t _X0 = XBit[0];
	// interrupt id 0 : X0 up edge
	if (!itr_oldXBit[0] && _X0 &&
		itr_funcs[0])
	{
		itr_funcs[0]();
	}
	// interrupt id 1 : X0 down edge
	if (itr_oldXBit[0] && !_X0 &&
		itr_funcs[1])
	{
		itr_funcs[1]();
	}
	// interrupt id 2 : X0 all edge
	if ((!itr_oldXBit[0] && _X0 || itr_oldXBit[0] && !_X0) &&
		itr_funcs[2])
	{
		itr_funcs[2]();
	}
	itr_oldXBit[0] = _X0;
	int32_t _X1 = XBit[0];
	// interrupt id 3 : X0 up edge
	if (!itr_oldXBit[1] && _X1 &&
		itr_funcs[3])
	{
		itr_funcs[3]();
	}
	// interrupt id 4 : X0 down edge
	if (itr_oldXBit[1] && !_X1 &&
		itr_funcs[4])
	{
		itr_funcs[4]();
	}
	// interrupt id 5 : X0 all edge
	if ((!itr_oldXBit[1] && _X1 || itr_oldXBit[1] && !_X1) &&
		itr_funcs[5])
	{
		itr_funcs[5]();
	}
	itr_oldXBit[1] = _X1;
	// interrupt id 6 : timer1 arrive
	while (itr_funcs[6] && DWord[8173] > 0 &&
		counttimems - itr_times[0] > DWord[8173])
	{
		itr_times[0] += DWord[8173];
		itr_funcs[6]();
	}
	// interrupt id 6 : timer1 arrive
	while (itr_funcs[7] && DWord[8174] > 0 &&
		counttimems - itr_times[1] > DWord[8174])
	{
		itr_times[1] += DWord[8174];
		itr_funcs[7]();
	}
}

// Get the system real time
void _trd(int32_t* d)
{
	// get time_t
	_time_t = time(NULL);
	_time_t = _time_t - _time_t_new + _time_t_old;
	// translate to setting clock and convert it to struct tm
	rtimer = localtime(&_time_t);
	// set them to D-base memory
	d[0] = _WORD_to_BCD(rtimer->tm_year);
	d[1] = _WORD_to_BCD(rtimer->tm_mon);
	d[2] = _WORD_to_BCD(rtimer->tm_mday);
	d[3] = _WORD_to_BCD(rtimer->tm_hour);
	d[4] = _WORD_to_BCD(rtimer->tm_min);
	d[5] = _WORD_to_BCD(rtimer->tm_sec);
	d[6] = 0;
	d[7] = _WORD_to_BCD(rtimer->tm_wday);
	//free(rtimer);
}

// Set the system real time
void _twr(int32_t* d)
{
	// create a new struct tm
	rtimer = &_rtimer;
	rtimer->tm_year = _BCD_to_WORD(d[0]);
	rtimer->tm_mon = _BCD_to_WORD(d[1]);
	rtimer->tm_mday = _BCD_to_WORD(d[2]);
	rtimer->tm_hour = _BCD_to_WORD(d[3]);
	rtimer->tm_min = _BCD_to_WORD(d[4]);
	rtimer->tm_sec = _BCD_to_WORD(d[5]);
	rtimer->tm_wday = _BCD_to_WORD(d[7]);
	// set the old time and new time
	_time_t_old = time(NULL);
	_time_t_new = mktime(rtimer);
}

// commuticate in COMPort and process the commands in Modbus table
void _mbus(uint32_t com_id, struct ModbusTable* table, int tlen, uint32_t* wr){}

// commuticate in COMPort and send a series of memory to another
void _send(uint32_t com_id, int32_t* addr, int32_t len){}

// commuticate in COMPort and reseive a series of memory from another
void _rev(uint32_t com_id, int32_t* addr, int32_t len){}

// Create a pulse signal by the giving fequency parameter(in 32-bit integer)
void _plsf(uint32_t feq, int32_t* out)
{
	_dplsf((uint64_t)(feq), out);
}

// Create a pulse signal by the giving fequency parameter(in 64-bit integer)
void _dplsf(uint64_t feq, int32_t* out)
{
	// get the Y-base offset from program address
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	// signal zone (Y000 ~ Y003)
	if (id < 4)
	{
		// overflow flag
		uint32_t* _of = (&MBit[8118 + id]);
		// pulse number counter
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		// if signal culculous time recorder is not used
		if (YTime[id] == 0)
		{
			// initialize it
			YTime[id] = counttimems;
			// initialize the float accuracy pulse counter
			YCount[id] = 0.0;
			// initialize original counter
			*_ct = 0;
		}
		// is used and legal
		else
		{
			// set the current fequency
			YFeq[id] = feq;
			// get the culculous time inteval
			uint64_t _td = counttimems - YTime[id];
			// get the inteval value delta
			double _pn = ((double)(_td) * feq) / 1000;
			// get the changed value (in float)
			double _yc = YCount[id] + _pn;
			// check if it overflow 64-bit integer dimension 
			*_of = (_yc > (~((uint64_t)(0))));
			// convert to integer and set it if it not overflow
			if (!*_of) *_ct = (uint64_t)(_yc);
			// over the inteval time
			if (counttimems - YTime[id] >= 10)
			{
				// to the next inteval
				YTime[id] += 10;
				YCount[id] = _yc;
			}
		}
	}
}

// Extra message : Duty cycle (32-bit integer)
void _pwm(uint32_t feq, uint32_t dc, int32_t* out)
{
	_plsf(feq, out);
}

// Extra message : Duty cycle (64-bit integer)
void _dpwm(uint64_t feq, uint64_t dc, int32_t* out)
{
	_dplsf(feq, out);
}

// Extra message : Pulse number to output (32-bit integer)
void _plsy(uint32_t feq, uint32_t* pn, int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		// create the signal
		_plsf(feq, out);
		// set the pulse number
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		*pn = (uint32_t)(*_ct);
	}
}

// Extra message : Pulse number to output (64-bit integer)
void _dplsy(uint64_t feq, uint64_t* pn, int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		_dplsf(feq, out);
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		*pn = *_ct;
	}
}

static uint64_t _plsrmsgbuff[8196];
// Create a segment-divided and linear-faded pulse by 32-bit parameter
void _plsr(uint32_t* msg, int32_t it, int32_t* out)
{
	int i = 0;
	while (msg[i] != 0)
	{
		_plsrmsgbuff[i] = (uint64_t)(msg[i]);
	}
	_dplsr(_plsrmsgbuff, it, out);
}

// Create a segment-divided and linear-faded pulse by 64-bit parameter
// Each segment has it own fequency and pulse number
// msg : message array
// it : inteval time to fade the fequency
// out : the port to output the generated pulse signal
void _dplsr(uint64_t* msg, int64_t it, int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		// overflow flag
		uint32_t* _of = &MBit[8118 + id];
		// counter
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		// active flag
		uint32_t* _active = &MBit[8134 + id];
		// ignore error flag
		uint32_t* _igerr = &MBit[8070 + id];
		// error flag
		uint32_t* _iserr = &MBit[8086 + id];
		// error code flag
		uint32_t* _err = &DWord[8176];
		// current index flag
		uint32_t* _YIndex = &DWord[8124 + id];
		// error index flag
		uint32_t* _errid = &DWord[8108 + id];
		// previous fequency
		int64_t _feq_old;
		// current fequency
		int64_t _feq_new;
		// inteval pulse number
		int64_t _pn_itv;
		// the all of pulse number in this segment
		int64_t _pn;
		// check and initialize it if not active
		if (!*_active)
		{
			// check it if cannot ignore error
			if (!*_igerr)
			{
				// start the error checking
				*_errid = 1; *_err = 0;
				while (!*_err && *_errid < 2000)
				{
					// get the old fequency
					_feq_old = (*_errid > 1 ? msg[*_errid * 2 - 4] : 0);
					// get the new fequency
					_feq_new = msg[*_errid * 2 - 2];
					// get the maxinum pulse number
					_pn = msg[*_errid * 2 - 1];
					// get the inteval pulse number
					_pn_itv = (_feq_old + _feq_new) * it / 2000;
					// Error 23 : Pulse number is over the allowed limit.
					if (_pn > 1000000) *_err = 23;
					// Error 24 : The fequency of first segment is less than the minimum.
					if (*_errid == 1 && _feq_new < 100) *_err = 24;
					// Error 25 : the fequency is less than the minimum.
					if (_feq_new < 1) *_err = 25;
					// Error 26 : the pulse number is less than the minimum.
					if (_pn < 1000) *_err = 26;
					// Error 27 : the inteval time is less than the minimum.
					if (it < 100) *_err = 27;
					// the last fequency (with zero fequency)
					if (_feq_new == 0)
					{
						// Error 29 : not enough pluse to fade the fequency
						if (_pn_itv > _pn) *_err = 29;
						break;
					}
					// Error 28 : not enough pluse to fade the fequency
					if (_pn_itv > _pn) *_err = 28;
				}
				// error happen and set the flag
				if (*_err)
				{
					*_iserr = 1;
					return;
				}
				*_iserr = 0;
				*_errid = 0;
			}
			// initialize it 
			*_active = 1;
			*_YIndex = 1;
			*_ct = 0;
			YTarget[id] = 0;		    // the target number to move it to the next segment
			YITime[id] = counttimems;   // the start time when it come into this segment
			return;
		}
		
		// get the segment infomation
		_feq_old = ((*_YIndex) > 1 ? msg[(*_YIndex) * 2 - 4] : 0);
		_feq_new = msg[(*_YIndex) * 2 - 2];
		_pn = msg[(*_YIndex) * 2 - 1];
		_pn_itv = (_feq_old + _feq_new) * it / 2000;
		
		// the last segment?
		if (_feq_new == 0)
		{
			// in the inteval zone
			if (*_ct < YTarget[id] + _pn_itv)
			{
				_dplsf(_feq_old + (_feq_new - _feq_old) * (counttimems - YITime[id]) / it, out);
			}
			// end the pulse signal
			else
			{
				*_active = 0;
			}
		}
		// in the inteval zone
		if (*_ct < YTarget[id] + _pn_itv)
		{
			_dplsf(_feq_old + (_feq_new - _feq_old) * (counttimems - YITime[id]) / it, out);
		}
		// in the inner zone
		else if (*_ct < YTarget[id] + _pn)
		{
			_dplsf(_feq_new, out);
		}
		// move to next xegment
		else
		{
			//_dplsf(_feq_new, out);
			(*_YIndex) = (*_YIndex) + 1;
			YTarget[id] = *_ct;
			YITime[id] = counttimems;
		}
	}
}

// Create a segment-divided, linear-faded and oriented pulse by 32-bit parameter
void _plsrd(uint32_t* msg, int32_t ct, int32_t* out, int32_t* dir)
{
	_plsr(msg, ct, out);
}

// Create a segment-divided, linear-faded and oriented pulse by 64-bit parameter
void _dplsrd(uint64_t* msg, int64_t ct, int32_t* out, int32_t* dir)
{
	_dplsr(msg, ct, out);
}

// Move to next segment
void _plsnext(int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		// active flag
		uint32_t* _active = &MBit[8134 + id];
		// current index
		uint32_t* _YIndex = &DWord[8124 + id];
		// counter
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		// move it when active
		if (*_active)
		{
			*_YIndex++;
			YTarget[id] = *_ct;
			YITime[id] = counttimems;
		}
	}
}

// Stop generating signal
void _plsstop(int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		// set the active flag to 0
		uint32_t *_active = &MBit[8134 + id];
		// counter
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		// overflow flag
		uint32_t* _of = &MBit[8118 + id];
		*_active = 0;
		*_ct = 0;
		*_of = 0;
		YFeq[id] = 0;
		YTime[id] = 0;
	}
}

// Pulse signal mountain (32-bit)
void _zrn(uint32_t bv, uint32_t cv, int32_t sg, int32_t* out)
{
	_dzrn((uint64_t)(bv), (uint64_t)(cv), sg, out);
}

// Pulse signal mountain (64-bit)
void _dzrn(uint64_t cv, uint64_t bv, int32_t sg, int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		// active flag
		uint32_t* _active = &MBit[8134 + id];
		// zrn inteval time
		uint32_t* _zt = &DWord[8076 + id];
		// old fequency 
		int64_t _feq_old;
		// new fequency
		int64_t _feq_new;
		// initialize
		if (!*_active)
		{
			*_active = 1;
			YZTime[id] = counttimems;
			return;
		}
		// depend on zrn status
		switch (YZStatus[id])
		{
		// climb status
		case 0:
			// move to next status if signal ON
			if (sg)
			{
				YZStatus[id] = 1;		
			}
			// on climbing
			else if (counttimems - YZTime[id] < *_zt) 
			{
				_dplsf(cv * (counttimems - YITime[id]) / (*_zt), out);
			}
			// on the top
			else
			{
				_dplsf(cv, out);
			}
			break;
		// slide status
		case 1:
			// stop if singal OFF
			if (!sg) 
			{
				YZStatus[id] = 0;
				*_active = 0;
			}
			// on sliding
			else if (YZTime[id] - counttimems < *_zt)
			{
				_dplsf(bv + (cv - bv) * (counttimems - YITime[id]) / (*_zt), out);
			}
			// on the bottom
			else
			{
				_dplsf(bv, out);
			}
			break;
		}
	}
}

// Create a segment-divided pulse signal, with the linear fequency in the segment (64-bit)
void _pto(uint64_t* msg, int32_t* out, int32_t* dir)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		// counter
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		// active flag
		uint32_t* _active = &MBit[8134 + id];
		// direction
		uint32_t* _dir = &DWord[8102 + id];
		// current segment index
		uint32_t* _YIndex = &DWord[8124 + id];
		// error segment index
		uint32_t* _errid = &DWord[8108 + id];
		// messages count
		uint64_t msgct = msg[0];
		// check and initialize
		if (!*_active) 
		{
			*_errid = 1;
			// find the error segment
			for (; *_errid <= msgct; *_errid++)
			{
				// error when pulse number is zero
				if (msg[*_errid * 3] == 0)
				{
					break;
				}
			}
			// set it 0 if cannot found
			if (*_errid > msgct)
			{
				*_errid = 0;
				// initialize first
				*_active = 1;
				*_ct = 0;
				// initialize the pulse number target
				YPTarget[id] = msg[3];
			}
			return;
		}
		// close it when over the end segment
		if (*_YIndex > msgct)
		{
			*_active = 0;
			return;
		}
		// start fequency
		int64_t _feq_old = msg[*_YIndex * 3 - 2];
		// end fequency
		int64_t _feq_new = msg[*_YIndex * 3 - 1];
		// pulse number
		int64_t _pn = msg[*_YIndex * 3];
		// unarrive at the target?
		if (*_ct < YPTarget[id])
		{
			// seem the fequency as velocity, number as length, we can get the square relationship between them
			// use "sqrt" in "math.h" to calculate the fequency
			_dplsf(_feq_old + (_feq_new - _feq_old) * sqrt(_pn - (YPTarget[id] - *_ct)) / sqrt(_pn), out);
		}
		else
		{
			// move to next segment
			*_YIndex++; 
			YPTarget[id] = msg[*_YIndex * 3];
		}
	}
}

// Set an unique segment, fade the current fequency to it and fulfill its pulse number (32-bit)
void _drvi(uint32_t feq, uint32_t pn, int32_t* out)
{
	_ddrvi((uint64_t)(feq), (uint64_t)(pn), out);
}

// Set an unique segment, fade the current fequency to it and fulfill its pulse number (64-bit)
void _ddrvi(uint64_t feq, uint64_t pn, int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		// counter
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		// active flag
		uint32_t* _active = &MBit[8134 + id];
		// error flag
		uint32_t* _iserr = &MBit[8086 + id];
		// ignore error flag
		uint32_t* _igerr = &MBit[8070 + id];
		// error code
		uint32_t* _err = &DWord[8176];
		// interval time
		int32_t* _it = &DWord[8092 + id];
		// old fequency
		int64_t _feq_old;
		// new fequency
		int64_t _feq_new;
		// pulse number
		int64_t _pn;
		// interval pulse number
		int64_t _pn_itv;
		// check and initialize
		if (!*_active)
		{
			// check if we don't wanna ignore it
			if (!*_igerr)
			{
				// same as DPLSR
				_feq_old = YFeq[id];
				_feq_new = feq;
				_pn = pn;
				_pn_itv = (_feq_old + _feq_new) * (*_it) / 2000;
				if (_pn > 1000000) *_err = 23;
				if (_feq_new < 50) *_err = 25;
				if (_pn < 1000) *_err = 26;
				if ((*_it) < 100) *_err = 27;
				if (_feq_new == 0)
				{
					if (_pn_itv > _pn) *_err = 29;
				}
				else
				{
					if (_pn_itv > _pn) *_err = 28;
				}
				if (*_err)
				{
					*_iserr = 1;
					return;
				}
			}
			// initialize if accept
			*_active = 1;
			YDFeq[id] = YFeq[id];
			YDTime[id] = counttimems;
			return;
		}
		// infomations
		_feq_old = YDFeq[id];
		_feq_new = feq;
		_pn = pn;
		_pn_itv = (_feq_old + _feq_new) * (*_it) / 2000;
		// in the interval zone
		if (*_ct < YTarget[id] + _pn_itv)
		{
			_dplsf(_feq_old + (_feq_new - _feq_old) * (counttimems - YDTime[id]) / (*_it), out);
		}
		// in the inner zone
		else if (*_ct < YTarget[id] + _pn)
		{
			_dplsf(_feq_new, out);
		}
		// stop it when out of nubmer limit
		else
		{
			*_active = 0;
		}
	}
}

// High counter
void _hcnt(int64_t* cv, int64_t sv){}

// Calculate the log10
double _log(double ia)
{
	return log(ia) / log(10.0);
}

// Calculate the power
double _pow(double ia, double ib)
{
	return pow(ia, ib);
}

// Calculate the factorial of 32-bit integer
int64_t _fact(int32_t itg)
{
	int64_t ret = 1;
	while (itg > 1)
	{
		ret *= itg;
		itg--;
	}
	return ret;
}

// Compare a couple of 32-bit integers(named as ia and ib)
// return 0 if ia == ib
// return -1 if ia < ib
// return 1 if ia > ib
void _cmpw(int32_t ia, int32_t ib, int32_t* output)
{
	// compare
	output[0] = (ia == ib);
	output[1] = (ia < ib);
	output[2] = (ia > ib);
}

// compare a couple of 64-bit integers(named as ia and ib)
// return 0 if ia == ib
// return -1 if ia < ib
// return 1 if ia > ib
void _cmpd(int64_t ia, int64_t ib, int32_t* output)
{
	output[0] = (ia == ib);
	output[1] = (ia < ib);
	output[2] = (ia > ib);
}

// compare a couple of 64-bit float(named as ia and ib)
// return 0 if ia == ib
// return -1 if ia < ib
// return 1 if ia > ib
void _cmpf(double ia, double ib, int32_t* output)
{
	output[0] = (ia == ib);
	output[1] = (ia < ib);
	output[2] = (ia > ib);
}

// check it if the targeted 32-bit integer is inside the range [il, ir] (16-bit)
// return 0 if the range indeedly include it
// return -1 if it is less than left border
// return 1 if it is more than right border
void _zcpw(int32_t ia, int32_t il, int32_t ir, int32_t* output)
{
	output[0] = (ia >= il && ia <= ir);
	output[1] = (ia < il);
	output[2] = (ia > ir);
}

// check it if the targeted 64-bit integer is inside the range [il, ir] (32-bit)
// return 0 if the range indeedly include it
// return -1 if it is less than left border
// return 1 if it is more than right border
void _zcpd(int64_t ia, int64_t il, int64_t ir, int32_t* output)
{
	output[0] = (ia >= il && ia <= ir);
	output[1] = (ia < il);
	output[2] = (ia > ir);
}

// check it if the targeted 64-bit float is inside the range [il, ir] (32-bit float)
// return 0 if the range indeedly include it
// return -1 if it is less than left border
// return 1 if it is more than right border
void _zcpf(double ia, double il, double ir, int32_t* output)
{
	output[0] = (ia >= il && ia <= ir);
	output[1] = (ia < il);
	output[2] = (ia > ir);
}

// make 32-bit integer ia negative 
int32_t _negw(int32_t ia)
{
	return -ia;
}

// make 64-bit integer ia negative
int64_t _negd(int64_t ia)
{
	return -ia;
}

// swap a couple of 16-bit integers
void _xchw(int32_t* pa, int32_t* pb)
{
	_tempw = *pa;
	*pa = *pb;
	*pb = _tempw;
}

// swap a couple of 32-bit integer
void _xchd(int64_t* pa, int64_t* pb)
{
	_tempd = *pa;
	*pa = *pb;
	*pb = _tempd;
}

// swap a couple of 32-bit float
void _xchf(double* pa, double* pb)
{
	_tempf = *pa;
	*pa = *pb;
	*pb = _tempf;
}

// binary reserve 32-bit integer ia 
int32_t _cmlw(int32_t ia) 
{
	return ~ia;
}

// binary reserve 64-bit integer ia
int64_t _cmld(int64_t ia)
{
	return ~ia;
}
// copy a series of 32-bit memory
void _mvwblk(int32_t* source, int32_t* dest, int32_t* enable, int32_t size)
{
	while (--size >= 0)
	{
		if (!enable[size])
			dest[size] = source[size];
	}
}

// copy a series of 64-bit memory
void _mvdblk(int64_t* source, int64_t* dest, int32_t* enable, int32_t size)
{
	while (--size >= 0)
	{
		if (!enable[size])
			dest[size] = source[size];
	}
}

// set a series of 32-bit memeory to the targeted value
void _fmovw(int32_t source, int32_t* dest, int32_t* enable, int32_t size)
{
	while (--size >= 0)
	{
		if (!enable[size])
			dest[size] = source;
	}
}

// set a series of 32-bit memeory to the targeted value
void _fmovd(int64_t source, int64_t* dest, int32_t* enable, int32_t size)
{
	while (--size >= 0)
	{
		if (!enable[size])
			dest[size] = source;
	}
}
// move the fragment of a BCD code to anothor at the targeted position.
void _smov(int32_t source, int32_t sb1, int32_t sb2, int32_t* target, int32_t tb1)
{
	source = _WORD_to_BCD(source);
	*target = _WORD_to_BCD(*target);
	*target &= ~(((1 << ((sb2 - sb1 + 1) * 4)) - 1) << ((tb1 - 1) * 4));
	*target |= ((source >> ((sb1 - 1) * 4))&((1 << ((sb2 - sb1 + 1) * 4)) - 1)) << ((tb1 - 1) * 4);
	*target = _BCD_to_WORD(*target);
}

