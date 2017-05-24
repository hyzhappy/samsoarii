/*
	The header file for simulate program of user PLC Device
   */

#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <Windows.h>

#ifdef BUILD_DLL
#define DLL_EXPORT __declspec(dllexport)
#else
#define DLL_EXPORT __declspec(dllimport)
#endif // BUILD_DLL

#ifdef __cplusplus
#define EXPORT extern "C" DLL_EXPORT __stdcall
#else
#define EXPORT DLL_EXPORT __stdcall
#endif

// Register Memorys
int32_t XBit[128];
int32_t YBit[128];
int32_t MBit[256<<5];
int32_t CBit[256];
int32_t TBit[256];
int32_t SBit[32<<5];
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
int32_t MEnable[256<<5];
int32_t CEnable[256];
int32_t TEnable[256];
int32_t SEnable[32<<5];
int32_t DEnable[8192];
int32_t CVEnable[256];
int32_t TVEnable[256];
int32_t AIEnable[32];
int32_t AOEnable[32];
int32_t VEnable[8];
int32_t ZEnable[8];
// pulse signal frequency
extern uint64_t YFeq[4];

EXPORT void BeforeRunLadder();

EXPORT void RunLadder();

EXPORT void AfterRunLadder();

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

int basebit = 32;
EXPORT void SetBaseBit(int _basebit)
{
	basebit = _basebit;
}

static int currenttime;
static int beforetime;
static int aftertime;
static int deltatime;
int counttime;
int counttimems;
int timerate = 50;
static int scanperiod = 1000;

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
	UpdateClock();
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
	counttime += deltatime;
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

