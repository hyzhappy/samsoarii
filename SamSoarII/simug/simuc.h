/*
	The header file for simulate program of user PLC Device
   */

#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <Windows.h>

#ifndef _SIMUC_H__
#define _SIMUC_H__

#ifdef BUILD_DLL
#define DLL_EXPORT __declspec(dllexport)
#else
#define DLL_EXPORT __declspec(dllimport)
#endif // BUILD_DLL

#endif // _SIMUC_H__

#ifdef __cplusplus
#define EXPORT extern "C" DLL_EXPORT __stdcall
#else
#define EXPORT DLL_EXPORT __stdcall
#endif

// Register Memorys
uint32_t XBit[128];
uint32_t YBit[128];
uint32_t MBit[256<<5];
uint32_t CBit[256];
uint32_t TBit[256];
uint32_t SBit[32<<5];
uint16_t DWord[8192];
uint16_t CVWord[200];
uint32_t CVDoubleWord[56];
uint16_t TVWord[256];
// Register writeable
// Set 1 if you want to modify the value of register
// othervise, Set 0 to lock the register and make it constant
uint32_t XEnable[128];
uint32_t YEnable[128];
uint32_t MEnable[256<<5];
uint32_t CEnable[256];
uint32_t TEnable[256];
uint32_t SEnable[32<<5];
uint32_t DEnable[8192];
uint32_t CVEnable[256];
uint32_t TVEnable[256];

EXPORT void BeforeRunLadder();

EXPORT void RunLadder();

EXPORT void AfterRunLadder();

//EXPORT void GetBit(char* name, int size, uint32_t* output);

//EXPORT void GetWord(char* name, int size, uint16_t* output);

//EXPORT void GetDoubleWord(char* name, int size, uint32_t* output);

//EXPORT void GetFloat(char* name, int size, float* output);

//EXPORT void GetDouble(char* name, int size, double* output);

//EXPORT void SetBit(char* name, int size, uint32_t* input);

//EXPORT void SetWord(char* name, int size, uint16_t* input);

//EXPORT void SetDoubleWord(char* name, int size, uint32_t* input);

//EXPORT void SetFloat(char* name, int size, float* input);

//EXPORT void SetDouble(char* name, int size, double* input);

//EXPORT void SetEnable(char* name, int size, int value);

// Get the BIT value from targeted bit register (X/Y/M/C/T/S) 
EXPORT void GetBit(char* name, int size, uint32_t* output)
{
	int addr = 0;
	sscanf(name + 1, "%d", &addr);
	switch (name[0])
	{
	case 'X':
		memcpy(output, XBit + addr, size * sizeof(uint32_t));
		break;
	case 'Y':
		memcpy(output, YBit + addr, size * sizeof(uint32_t));
		break;
	case 'M':
		memcpy(output, MBit + addr, size * sizeof(uint32_t));
		break;
	case 'C':
		memcpy(output, CBit + addr, size * sizeof(uint32_t));
		break;
	case 'T':
		memcpy(output, TBit + addr, size * sizeof(uint32_t));
		break;
	case 'S':
		memcpy(output, SBit + addr, size * sizeof(uint32_t));
		break;
	default:
		break;
	}
}
// Get the WORD value from targeted register (D/CV/TV)
EXPORT void GetWord(char* name, int size, uint16_t* output)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(output, DWord + addr, size * sizeof(uint16_t));
		break;
	case 'C':
		sscanf(name + 2, "%d", &addr);
		memcpy(output, CVWord + addr, size * sizeof(uint16_t));
		break;
	case 'T':
		sscanf(name + 2, "%d", &addr);
		memcpy(output, TVWord + addr, size * sizeof(uint16_t));
		break;
	default:
		break;
	}
}
// Get the DWORD (32 bit unsigned int) value from targeted register (D/CV32)
EXPORT void GetDoubleWord(char* name, int size, uint32_t* output)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(output, DWord + addr, size * sizeof(uint32_t));
		break;
	case 'C':
		sscanf(name + 2, "%d", &addr);
		memcpy(output, CVDoubleWord + (addr - 200), size * sizeof(uint32_t));
		break;
	default:
		break;
	}
}
// Get the FLOAT value from targeted register (D)
EXPORT void GetFloat(char* name, int size, float* output)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(output, DWord + addr, size * sizeof(float));
		break;
	default:
		break;
	}
}
// Get the DOUBLE value from targeted register (D)
EXPORT void GetDouble(char* name, int size, double* output)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(output, DWord + addr, size * sizeof(double));
		break;
	default:
		break;
	}
}
// Set the Bit value to targeted bit register (X/Y/M/C/T/S)
EXPORT void SetBit(char* name, int size, uint32_t* input)
{
	int addr = 0;
	sscanf(name + 1, "%d", &addr);
	switch (name[0])
	{
	case 'X':
		memcpy(XBit + addr, input, size * sizeof(uint32_t));
		break;
	case 'Y':
		memcpy(YBit + addr, input, size * sizeof(uint32_t));
		break;
	case 'M':
		memcpy(MBit + addr, input, size * sizeof(uint32_t));
		break;
	case 'C':
		memcpy(CBit + addr, input, size * sizeof(uint32_t));
		break;
	case 'T':
		memcpy(TBit + addr, input, size * sizeof(uint32_t));
		break;
	case 'S':
		memcpy(SBit + addr, input, size * sizeof(uint32_t));
		break;
	default:
		break;
	}
}
// Set the WORD value to targeted register (D/CV/TV)
EXPORT void SetWord(char* name, int size, uint16_t* input)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(DWord + addr, input, size * sizeof(uint16_t));
		break;
	case 'C':
		sscanf(name + 2, "%d", &addr);
		memcpy(CVWord + addr, input, size * sizeof(uint16_t));
		break;
	case 'T':
		sscanf(name + 2, "%d", &addr);
		memcpy(TVWord + addr, input, size * sizeof(uint16_t));
		break;
	default:
		break;
	}
}
// Set the DWORD value to targeted register (D)
EXPORT void SetDoubleWord(char* name, int size, uint32_t* input)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(DWord + addr, input, size * sizeof(uint32_t));
		break;
	case 'C':
		sscanf(name + 2, "%d", &addr);
		memcpy(CVDoubleWord + (addr - 200), input, size * sizeof(uint32_t));
		break;
	default:
		break;
	}
}
// Set the FLOAT value to targeted register (D)
EXPORT void SetFloat(char* name, int size, float* input)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(DWord + addr, input, size * sizeof(float));
		break;
	default:
		break;
	}
}
// Set the DOUBLE value to targeted register (D)
EXPORT void SetDouble(char* name, int size, double* input)
{
	int addr = 0;
	switch (name[0])
	{
	case 'D':
		sscanf(name + 1, "%d", &addr);
		memcpy(DWord + addr, input, size * sizeof(double));
		break;
	default:
		break;
	}
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
/* 
	The manipulation of lock registers and view registers
	Here is the behaviors
	1. Add an lock event to change the value of the targeted register at a specific time
	2. Delete the event above (unuse now)
	3. Add the view register
	4. Remove the lock register by remove all of its lock events
	5. Remove the view rigister
   */

// the ID of value type
#define DP_TYPE_BIT 0x01
#define DP_TYPE_WORD 0x02
#define DP_TYPE_DOUBLEWORD 0x03
#define DP_TYPE_FLOAT 0x04
#define DP_TYPE_DOUBLE 0x05
// convert the struct DataPoint and get the value of specific type
#define WORDVALUE(dp) *((uint16_t*)(&(dp).rsv1))
#define FLOATVALUE(dp) *((float*)(&(dp).rsv1))
#define DOUBLEVALUE(dp) *((double*)(&(dp).rsv1))
/*
   DataPoint struct :
		type : value type
		name : register name
		time : value changed time
		rsv1, rsv2 : reserve memory, commonly used to store the value 
	*/
struct DataPoint
{
	int type;
	char* name;
	int time;
	int rsv1, rsv2;
};
/*
struct BitDataPoint
{
	char* name;
	int time;
	int value;
};

struct WordDataPoint
{
	char* name;
	int time;
	int value;
};

struct DoubleWordDataPoint
{
	char* name;
	int time;
	int value;
};

struct floatWordDataPoint
{
	char* name;
	int time;
	float value;
};

struct DoubleDataPoint
{
	char* name;
	int time;
	double value;
};
*/
// DataPointInput
static int dpic;
static struct DataPoint dpis[65536];
// DataPointOutput
static int dpoc;
static struct DataPoint dpos[65536];

/*
   the struct to illustrate the view register
	type : register value type
	name : register name
	dp : the start DataPoint
	dpend : the ending DataPoint
	*/
struct DataInputView
{
	int type;
	char* name;
	struct DataPoint* dp;
	struct DataPoint* dpend;
};
// DataViewInput
static int dvic;
static struct DataInputView dvis[1024];
// DataViewOutput
static int dvoc;
static struct DataPoint dvos[1024];
/*	The memory to store register names
	In order to avoid the dynamic allocation and memory expose
	*/
static char nmem[65536];
static char* _nmem;
static char* XName[128];
static char* YName[128];
static char* MName[256 << 5];
static char* CName[256];
static char* TName[256];
static char* SName[32 << 5];
static char* DName[8192];
static char* CVName[256];
static char* TVName[256];
// Initialize the name memory
EXPORT void InitDataPoint()
{
	int i;
	_nmem = &nmem[0];	
	for (i = 0; i < 65536; i++)
		nmem[i] = 0;

	for (i = 0; i < 128; i++)
		XName[i] = 0;
	for (i = 0; i < 128; i++)
		YName[i] = 0;
	for (i = 0; i < (256<<5); i++)
		MName[i] = 0;
	for (i = 0; i < 256; i++)
		CName[i] = 0;
	for (i = 0; i < 256; i++)
		TName[i] = 0;
	for (i = 0; i < (32<<5); i++)
		SName[i] = 0;
	for (i = 0; i < 8192; i++)
		DName[i] = 0;
	for (i = 0; i < 256; i++)
		CVName[i] = 0;
	for (i = 0; i < 256; i++)
		TVName[i] = 0;

	dpic = 0;
}
// clone the targeted name to the name memory
char* _CloneName(char* name)
{
	int offset = 0;
	switch (name[0])
	{
	case 'C': 
		switch (name[1])
		{
		case 'V':
			sscanf(name + 2, "%d", &offset);
			if (!CVName[offset])
			{
				sprintf(_nmem, "CV%d", offset);
				CVName[offset] = _nmem;
				_nmem = _nmem + (strlen(_nmem) + 1);
			}
			return CVName[offset];
		default:
			sscanf(name + 1, "%d", &offset);
			if (!CName[offset])
			{
				sprintf(_nmem, "C%d", offset);
				CName[offset] = _nmem;
				_nmem = _nmem + (strlen(_nmem) + 1);
			}
			return CName[offset];
		}
		break;
	case 'T':
		switch (name[1])
		{
		case 'V':
			sscanf(name + 2, "%d", &offset);
			if (!TVName[offset])
			{
				sprintf(_nmem, "TV%d", offset);
				TVName[offset] = _nmem;
				_nmem = _nmem + (strlen(_nmem) + 1);
			}
			return TVName[offset];
		default:
			sscanf(name + 1, "%d", &offset);
			if (!TName[offset])
			{
				sprintf(_nmem, "T%d", offset);
				TName[offset] = _nmem;
				_nmem = _nmem + (strlen(_nmem) + 1);
			}
			return TName[offset];
		}
		break;
	case 'D':
		sscanf(name + 1, "%d", &offset);
		if (!DName[offset])
		{
			sprintf(_nmem, "D%d", offset);
			DName[offset] = _nmem;
			_nmem = _nmem + (strlen(_nmem) + 1);
		}
		return DName[offset];
	case 'X':
		sscanf(name + 1, "%d", &offset);
		if (!XName[offset])
		{
			sprintf(_nmem, "X%d", offset);
			XName[offset] = _nmem;
			_nmem = _nmem + (strlen(_nmem) + 1);
		}
		return XName[offset];
	case 'Y':
		sscanf(name + 1, "%d", &offset);
		if (!YName[offset])
		{
			sprintf(_nmem, "Y%d", offset);
			YName[offset] = _nmem;
			_nmem = _nmem + (strlen(_nmem) + 1);
		}
		return YName[offset];
	case 'M':
		sscanf(name + 1, "%d", &offset);
		if (!MName[offset])
		{
			sprintf(_nmem, "M%d", offset);
			MName[offset] = _nmem;
			_nmem = _nmem + (strlen(_nmem) + 1);
		}
		return MName[offset];
	case 'S':
		sscanf(name + 1, "%d", &offset);
		if (!SName[offset])
		{
			sprintf(_nmem, "S%d", offset);
			CName[offset] = _nmem;
			_nmem = _nmem + (strlen(_nmem) + 1);
		}
		return SName[offset];
	default:
		return NULL;
	}
}
// Add a DataPoint of BIT value
EXPORT void AddBitDataPoint(char* name, int time, uint32_t value)
{
	// Ignore the equivalance
	/*
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_BIT &&
			!strcmp(dpis[i].name, name) &&
			dpis[i].time == time)
		{
			dpis[i].rsv1 = value;
			return;
		}
	}
	*/
	dpis[dpic].type = DP_TYPE_BIT;
	dpis[dpic].name = _CloneName(name);
	dpis[dpic].time = time;
	dpis[dpic].rsv1 = value;
	dpic++;
}
// Add a DataPoint of WORD value
EXPORT void AddWordDataPoint(char* name, int time, uint16_t value)
{
	/*
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_WORD &&
			!strcmp(dpis[i].name, name) &&
			dpis[i].time == time)
		{
			WORDVALUE(dpis[i]) = value;
			return;
		}
	}
	*/
	dpis[dpic].type = DP_TYPE_WORD;
	dpis[dpic].name = _CloneName(name);
	dpis[dpic].time = time;
	WORDVALUE(dpis[dpic]) = value;
	dpic++;
}
// Add a DataPoint of DOUBLE value
EXPORT void AddDoubleWordDataPoint(char* name, int time, uint32_t value)
{
	/*
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_DOUBLEWORD &&
			!strcmp(dpis[i].name, name) &&
			dpis[i].time == time)
		{
			dpis[i].rsv1 = value;
			return;
		}
	}
	*/
	dpis[dpic].type = DP_TYPE_DOUBLEWORD;
	dpis[dpic].name = _CloneName(name);
	dpis[dpic].time = time;
	dpis[dpic].rsv1 = value;
	dpic++;
}
// Add a DataPoint of FLOAT value
EXPORT void AddFloatDataPoint(char* name, int time, float value)
{
	/*
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_FLOAT &&
			!strcmp(dpis[i].name, name) &&
			dpis[i].time == time)
		{
			FLOATVALUE(dpis[i]) = value;
			return;
		}
	}
	*/
	dpis[dpic].type = DP_TYPE_FLOAT;
	dpis[dpic].name = _CloneName(name);
	dpis[dpic].time = time;
	FLOATVALUE(dpis[dpic]) = value;
	dpic++;
}
// Add a DataPoint of DOUBLE value
EXPORT void AddDoubleDataPoint(char* name, int time, double value)
{
	/*
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_DOUBLE &&
			!strcmp(dpis[i].name, name) &&
			dpis[i].time == time)
		{
			DOUBLEVALUE(dpis[i]) = value;
			return;
		}
	}
	*/
	dpis[dpic].type = DP_TYPE_DOUBLE;
	dpis[dpic].name = _CloneName(name);
	dpis[dpic].time = time;
	DOUBLEVALUE(dpis[dpic]) = value;
	dpic++;
}

void _RemoveDataPoint(int id)
{
	int i;

	for (i = id + 1; i < dpic; i++)
	{
		memcpy(dpis+(i-1), dpis+i, sizeof(struct DataPoint));
	}

	dpic--;
}

EXPORT void RemoveBitDataPoint(char* name, int time, uint32_t value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_BIT &&
			!strcmp(dpis[i].name, name) &&
			dpis[i].time == time)
		{
			_RemoveDataPoint(i);
		}
	}
}

EXPORT void RemoveWordDataPoint(char* name, int time, uint16_t value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_WORD &&
			!strcmp(dpis[i].name, name) &&
			dpis[i].time == time)
		{
			_RemoveDataPoint(i);
		}
	}
}

EXPORT void RemoveDoubleWordDataPoint(char* name, int time, uint32_t value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_DOUBLEWORD &&
			!strcmp(dpis[i].name, name) &&
			dpis[i].time == time)
		{
			_RemoveDataPoint(i);
		}
	}
}

EXPORT void RemoveFloatDataPoint(char* name, int time, float value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_FLOAT &&
			!strcmp(dpis[i].name, name) &&
			dpis[i].time == time)
		{
			_RemoveDataPoint(i);
		}
	}
}

EXPORT void RemoveDoubleDataPoint(char* name, int time, double value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_DOUBLE &&
			!strcmp(dpis[i].name, name) &&
			dpis[i].time == time)
		{
			_RemoveDataPoint(i);
		}
	}
}

EXPORT void AddViewInput(char* name, int type)
{

}

EXPORT void AddViewOutput(char* name, int type)
{
	dvos[dvoc].type = type;
	dvos[dvoc].name = _CloneName(name);
	switch (type)
	{
	case DP_TYPE_BIT:
	case DP_TYPE_DOUBLEWORD:
		dvos[dvoc].rsv1 = 0;
		break;
	case DP_TYPE_WORD:
		WORDVALUE(dvos[dvoc]) = 0;
		break;
	case DP_TYPE_FLOAT:
		FLOATVALUE(dvos[dvoc]) = 0.0;
		break;
	case DP_TYPE_DOUBLE:
		DOUBLEVALUE(dvos[dvoc]) = 0.0;
		break;
	}
	dvoc++;
}

void _RemoveViewPoint(int id)
{
	int i;

	for (i = id + 1; i < dvoc; i++)
	{
		memcpy(dvos+(i-1), dvos+i, sizeof(struct DataPoint));
	}

	dvoc--;

}

EXPORT void RemoveViewInput(char* name, int type)
{
	int i = 0, j = 0;
	for (; i < dpic ; i++)
	{
		if (strcmp(dpis[i].name, name) ||
			dpis[i].type != type)
		{
			memcpy(dpis[j++], dpis[i], sizeof(struct DataPoint));
		}
	}
	dpic = j;
}

EXPORT void RemoveViewOutput(char* name, int type)
{
	int i;
	for (i = 0; i < dvoc; i++)
	{
		if (dvos[i].type == type &&
			!strcmp(dvos[i].name, name))
		{
			_RemoveViewPoint(i);
		}
	}
}

static int currenttime;
static int beforetime;
static int aftertime;
static int deltatime;
static int counttime;
static int counttimems;
static int scanperiod = 1000;

void UpdateClock()
{
	LARGE_INTEGER lpCount;
	QueryPerformanceCounter(&lpCount);
	currenttime = (int)(lpCount.QuadPart / 10);
}

EXPORT void BeforeRunLadder()
{
	UpdateClock();
	beforetime = currenttime;

	MBit[8158] = ((counttimems / 5) & 1);
	MBit[8159] = ((counttimems / 50) & 1);
	MBit[8160] = ((counttimems / 500) & 1);

	//printf("counttime=%d counttimems=%d 10msclock=%d 100msclock=%d 1sclock=%d\n",
	//	counttime, counttimems, MBit[8158], MBit[8159], MBit[8160]);
}

EXPORT void AfterRunLadder()
{
	UpdateClock();
	aftertime = currenttime;
	deltatime = aftertime - beforetime;
	//if (deltatime < 0 || deltatime > 100)
	//	deltatime = 1;
	counttime += deltatime;
	counttimems = counttime / 1000;
}

void _SortDataPoint(int l, int r)
{
	if (l < 0 || r < 0 || l > r)
		return;
	int i = l, j = r;
	struct DataPoint* k = &dpis[(l + r) >> 1];
	struct DataPoint tmp;
	while (i <= j)
	{
		while (dpis[i].name < k->name || dpis[i].name == k->name && dpis[i].time < k->time) i++;
		while (dpis[j].name > k->name || dpis[j].name == k->name && dpis[j].time > k->time) j--;
		if (i <= j)
		{
			memcpy(&tmp, dpis + i, sizeof(struct DataPoint));
			memcpy(dpis + i, dpis + j, sizeof(struct DataPoint));
			memcpy(dpis + j, &tmp, sizeof(struct DataPoint));
			i++; j--;
		}
	}
	if (i < r) _SortDataPoint(i, r);
	if (j > l) _SortDataPoint(l, j);
}

EXPORT void RunData(char* outputFile, int starttime, int endtime)
{
	int i, j;

	FILE* fout = fopen(outputFile, "w");
	if (fout == NULL) return;
	_SortDataPoint(0, dpic - 1);
	dvis[0].name = dpis[0].name;
	dvis[0].type = dpis[0].type;
	dvis[0].dp = dpis;
	dvic = 1;
	for (i = 1; i < dpic; i++)
	{
		if (strcmp(dpis[i-1].name, dpis[i].name))
		{
			dvis[dvic-1].dpend = dpis + i;
			dvis[dvic].name = dpis[i].name;
			dvis[dvic].type = dpis[i].type;
			dvis[dvic++].dp = dpis + i;
		}
	}
	dvis[dvic-1].dpend = dpis + dpic;
	if (dpic == 0) dvic = 0;

	int t1, t2, dt, dc;

	counttime = 0;
	counttimems = 0;
	dt = 0;
	while (counttimems < endtime)
	{
		for (i = 0; i < dvic; i++)
		{
			while (dvis[i].dp != dvis[i].dpend &&
				   dvis[i].dp->time <= counttimems)
			{
				int offset;
				switch (dvis[i].name[0])
				{
				case 'X':
					sscanf(dvis[i].name + 1, "%d", &offset);
					XBit[offset] = dvis[i].dp->rsv1;
					break;
				case 'Y':
					sscanf(dvis[i].name + 1, "%d", &offset);
					YBit[offset] = dvis[i].dp->rsv1;
					break;
				case 'M':
					sscanf(dvis[i].name + 1, "%d", &offset);
					MBit[offset] = dvis[i].dp->rsv1;
					break;
				case 'S':
					sscanf(dvis[i].name + 1, "%d", &offset);
					SBit[offset] = dvis[i].dp->rsv1;
					break;
				case 'T':
					switch (dvis[i].name[1])
					{
					case 'V':
						sscanf(dvis[i].name + 2, "%d", &offset);
						TVWord[offset] = WORDVALUE(*dvis[i].dp);
						break;
					default:
						sscanf(dvis[i].name + 1, "%d", &offset);
						TBit[offset] = dvis[i].dp->rsv1;
						break;
					}
					break;
				case 'C':
					switch (dvis[i].name[1])
					{
					case 'V':
						sscanf(dvis[i].name + 2, "%d", &offset);
						if (offset < 200)
							CVWord[offset] = WORDVALUE(*dvis[i].dp);
						else
							CVDoubleWord[offset] = dvis[i].dp->rsv1;
						break;
					default:
						sscanf(dvis[i].name + 1, "%d", &offset);
						CBit[offset] = dvis[i].dp->rsv1;
						break;
					}
					break;
				case 'D':
					sscanf(dvis[i].name + 1, "%d", &offset);
					switch (dvis[i].type)
					{
					case DP_TYPE_WORD:
						//printf("set word %d\n", WORDVALUE(*dvis[i].dp));
						DWord[offset] = WORDVALUE(*dvis[i].dp);
						break;
					case DP_TYPE_DOUBLEWORD:
						*((uint32_t*)(DWord + offset)) = dvis[i].dp->rsv1;
						break;
					case DP_TYPE_FLOAT:
						*((float*)(DWord + offset)) = FLOATVALUE(*dvis[i].dp);
						break;
					case DP_TYPE_DOUBLE:
						*((double*)(DWord + offset)) = DOUBLEVALUE(*dvis[i].dp);
						break;
					default:
						break;
					}
					break;
				}
				dvis[i].dp++;
			}
		}

		//dt = 0;
		dc = 0;
		while (dt < scanperiod)
		{
			BeforeRunLadder();
			RunLadder();
			AfterRunLadder();
			UpdateClock();
			t2 = currenttime;
			dt += deltatime;
			dc++;
		}
		//printf("CountTime=%d MoveTime=%d MoveCount=%d CurrentTime=%d\n", counttime, dt, dc, currenttime);
		dt -= scanperiod;

		if (counttimems < starttime)
			continue;

		for (i = 0; i < dvoc; i++)
		{
			int offset;
			int value;
			float valuef;
			double valued;

			dvos[i].time = counttimems;
			switch (dvos[i].name[0])
			{
			case 'X':
				sscanf(dvos[i].name + 1, "%d", &offset);
				value = (int)(XBit[offset]);
				break;
			case 'Y':
				sscanf(dvos[i].name + 1, "%d", &offset);
				value = (int)(YBit[offset]);
				break;
			case 'M':
				sscanf(dvos[i].name + 1, "%d", &offset);
				value = (int)(MBit[offset]);
				break;
			case 'S':
				sscanf(dvos[i].name + 1, "%d", &offset);
				value = (int)(SBit[offset]);
				break;
			case 'T':
				switch (dvos[i].name[1])
				{
				case 'V':
					sscanf(dvos[i].name + 2, "%d", &offset);
					value = (int)(TVWord[offset]);
					break;
				default:
					sscanf(dvos[i].name + 1, "%d", &offset);
					value = (int)(TBit);
					break;
				}
				break;
			case 'C':
				switch (dvos[i].name[1]) 
				{
				case 'V':
					sscanf(dvos[i].name + 2, "%d", &offset);
					if (offset < 200)
						value = (int)(CVWord[offset]);
					else
						value = (int)(CVDoubleWord[offset]);
					break;
				default:
					sscanf(dvos[i].name + 1, "%d", &offset);
					value = (int)(CBit);
					break;
				}
				break;
			case 'D':
				sscanf(dvos[i].name + 1, "%d", &offset);
				switch (dvos[i].type)
				{
				case DP_TYPE_WORD:
					value = (int)(DWord[offset]);
					break;
				case DP_TYPE_DOUBLEWORD:
					value = (int)(*((uint32_t*)(DWord + offset)));
					break;
				case DP_TYPE_FLOAT:
					valuef = *((float*)(DWord + offset));
					break;
				case DP_TYPE_DOUBLE:
					valued = *((double*)(DWord + offset));
					break;
				default:
					break;
				}
				break;
			}

			switch (dvos[i].type)
			{
			case DP_TYPE_BIT:
			case DP_TYPE_DOUBLEWORD:
				if (dvos[i].rsv1 != value)
				{
					dvos[i].rsv1 = value;
					memcpy(dpos+(dpoc++), dvos+i, sizeof(struct DataPoint));
				}
				break;
			case DP_TYPE_WORD:
				if (WORDVALUE(dvos[i]) != value)
				{
					WORDVALUE(dvos[i]) = value;
					memcpy(dpos+(dpoc++), dvos+i, sizeof(struct DataPoint));
				}
				break;
			case DP_TYPE_FLOAT:
				if (FLOATVALUE(dvos[i]) != valuef)
				{
					FLOATVALUE(dvos[i]) = valuef;
					memcpy(dpos+(dpoc++), dvos+i, sizeof(struct DataPoint));
				}
				break;
			case DP_TYPE_DOUBLE:
				if (DOUBLEVALUE(dvos[i]) != valued)
				{
					DOUBLEVALUE(dvos[i]) = valued;
					memcpy(dpos+(dpoc++), dvos+i, sizeof(struct DataPoint));
				}
				break;
			}

			if (dpoc >= 60000)
			{
				for (j = 0; j < dpoc; j++)
				{
					fprintf(fout, "%s %d", dpos[j].name, dpos[j].time);
					switch (dpos[j].type)
					{
					case DP_TYPE_BIT:
					case DP_TYPE_DOUBLEWORD:
						fprintf(fout, " %d\n", dpos[j].rsv1);
						break;
					case DP_TYPE_WORD:
						fprintf(fout, " %d\n", WORDVALUE(dpos[j]));
						break;
					case DP_TYPE_FLOAT:
						fprintf(fout, " %f\n", FLOATVALUE(dpos[j]));
						break;
					case DP_TYPE_DOUBLE:
						fprintf(fout, " %lf\n", DOUBLEVALUE(dpos[j]));
						break;
					}
				}
				dpoc = 0;
			}
		}
	}
	for (j = 0; j < dpoc; j++)
	{
		fprintf(fout, "%s %d", dpos[j].name, dpos[j].time);
		switch (dpos[j].type)
		{
		case DP_TYPE_BIT:
		case DP_TYPE_DOUBLEWORD:
			fprintf(fout, " %d\n", dpos[j].rsv1);
			break;
		case DP_TYPE_WORD:
			fprintf(fout, " %d\n", WORDVALUE(dpos[j]));
			break;
		case DP_TYPE_FLOAT:
			fprintf(fout, " %f\n", FLOATVALUE(dpos[j]));
			break;
		case DP_TYPE_DOUBLE:
			fprintf(fout, " %lf\n", DOUBLEVALUE(dpos[j]));
			break;
		}
	}
	fclose(fout);
}
