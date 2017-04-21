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

#define DP_TYPE_BIT 0x01
#define DP_TYPE_WORD 0x02
#define DP_TYPE_DOUBLEWORD 0x03
#define DP_TYPE_FLOAT 0x04
#define DP_TYPE_DOUBLE 0x05
#define WORDVALUE(dp) *((uint16_t*)(&dp.rsv1))
#define FLOATVALUE(dp) *((float*)(&dp.rsv1))
#define DOUBLEVALUE(dp) *((double*)(&dp.rsv1))

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

static int dpic;
static struct DataPoint dpis[65536];
static int dpoc;
static struct DataPoint dpos[65536];

struct DataInputView
{
	char* name;
	struct DataPoint* dp;
	struct DataPoint* dpend;
};

static int dvic;
static struct DataInputView dvis[1024];
static int dvoc;
static struct DataPoint dvos[1024];

static char nmem[65536];
static char* XName[128];
static char* YName[128];
static char* MName[256 << 5];
static char* CName[256];
static char* TName[256];
static char* SName[32 << 5];
static char* DName[8192];
static char* CVName[256];
static char* TVName[256];

EXPORT void InitDataPoint()
{
	int i;
	char* _nmem = nmem;
	
	for (i = 0; i < 128; i++)
	{
		sprintf(_nmem, "X%d", i);
		XName[i] = _nmem;
		_nmem = _nmem + (strlen(_nmem) + 1);
	}
	for (i = 0; i < 128; i++)
	{
		sprintf(_nmem, "Y%d", i);
		YName[i] = _nmem;
		_nmem = _nmem + (strlen(_nmem) + 1);
	}
	for (i = 0; i < (256<<5); i++)
	{
		sprintf(_nmem, "M%d", i);
		MName[i] = _nmem;
		_nmem = _nmem + (strlen(_nmem) + 1);
	}
	for (i = 0; i < 256; i++)
	{
		sprintf(_nmem, "C%d", i);
		CName[i] = _nmem;
		_nmem = _nmem + (strlen(_nmem) + 1);
	}
	for (i = 0; i < 256; i++)
	{
		sprintf(_nmem, "T%d", i);
		TName[i] = _nmem;
		_nmem = _nmem + (strlen(_nmem) + 1);
	}
	for (i = 0; i < (32<<5); i++)
	{
		sprintf(_nmem, "S%d", i);
		SName[i] = _nmem;
		_nmem = _nmem + (strlen(_nmem) + 1);
	}
	for (i = 0; i < 8192; i++)
	{
		sprintf(_nmem, "D%d", i);
		DName[i] = _nmem;
		_nmem = _nmem + (strlen(_nmem) + 1);
	}
	for (i = 0; i < 256; i++)
	{
		sprintf(_nmem, "CV%d", i);
		CVName[i] = _nmem;
		_nmem = _nmem + (strlen(_nmem) + 1);
	}
	for (i = 0; i < 256; i++)
	{
		sprintf(_nmem, "TV%d", i);
		TVName[i] = _nmem;
		_nmem = _nmem + (strlen(_nmem) + 1);
	}

	dpic = 0;
}

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
			return CVName[offset];
		default:
			sscanf(name + 1, "%d", &offset);
			return CName[offset];
		}
		break;
	case 'T':
		switch (name[1])
		{
		case 'V':
			sscanf(name + 2, "%d", &offset);
			return TVName[offset];
		default:
			sscanf(name + 1, "%d", &offset);
			return TName[offset];
		}
		break;
	case 'D':
		sscanf(name + 1, "%d", &offset);
		return DName[offset];
	case 'X':
		sscanf(name + 1, "%d", &offset);
		return XName[offset];
	case 'Y':
		sscanf(name + 1, "%d", &offset);
		return YName[offset];
	case 'M':
		sscanf(name + 1, "%d", &offset);
		return MName[offset];
	case 'S':
		sscanf(name + 1, "%d", &offset);
		return SName[offset];
	default:
		return NULL;
	}
}

EXPORT void AddBitDataPoint(char* name, int time, uint32_t value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_BIT &&
			!strcmp(dpis[i].name, name) &&
			dips[i].time == time)
		{
			dpis[i].rsv1 = value;
			return;
		}
	}
	dpis[dpic].type = DP_TYPE_BIT;
	dpis[dpic].name = _CloneName(name);
	dpis[dpic].time = time;
	dpis[dpic].rsv1 = value;
}

EXPORT void AddWordDataPoint(char* name, int time, uint16_t value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_WORD &&
			!strcmp(dpis[i].name, name) &&
			dips[i].time == time)
		{
			WORDVALUE(dpis[i]) = value;
			return;
		}
	}
	dpis[dpic].type = DP_TYPE_WORD;
	dpis[dpic].name = _CloneName(name);
	dpis[dpic].time = time;
	WORDVALUE(dpis[dpic]) = value;
}

EXPORT void AddDoubleWordDataPoint(char* name, int time, uint32_t value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_DOUBLEWORD &&
			!strcmp(dpis[i].name, name) &&
			dips[i].time == time)
		{
			dpis[i].rsv1 = value;
			return;
		}
	}
	dpis[dpic].type = DP_TYPE_DOUBLEWORD;
	dpis[dpic].name = _CloneName(name);
	dpis[dpic].time = time;
	dpis[dpic].rsv1 = value;
}

EXPORT void AddFloatDataPoint(char* name, int time, float value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_FLOAT &&
			!strcmp(dpis[i].name, name) &&
			dips[i].time == time)
		{
			FLOATVALUE(dpis[i]) = value;
			return;
		}
	}
	dpis[dpic].type = DP_TYPE_FLOAT;
	dpis[dpic].name = _CloneName(name);
	dpis[dpic].time = time;
	FLOATVALUE(dpis[dpic]) = value;
}

EXPORT void AddDoubleDataPoint(char* name, int time, double value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpis[i].type == DP_TYPE_DOUBLE &&
			!strcmp(dpis[i].name, name) &&
			dips[i].time == time)
		{
			DOUBLEVALUE(dpis[i]) = value;
			return;
		}
	}
	dpis[dpic].type = DP_TYPE_DOUBLE;
	dpis[dpic].name = _CloneName(name);
	dpis[dpic].time = time;
	DOUBLEVALUE(dpis[dpic]) = value;
}

void _RemoveDataPoint(int id)
{
	int i;

	for (i = id + 1; i < dpic; i++)
	{
		memcpy(dpis[i - 1], dpis[i], sizeof(struct DataPoint));
	}

	dpic--;
}

EXPORT void RemoveBitDataPoint(char* name, int time, uint32_t value)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpic[i].type == DP_TYPE_BIT &&
			!strcmp(dpic[i].name, name) &&
			dpic[i].time == time)
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
		if (dpic[i].type == DP_TYPE_WORD &&
			!strcmp(dpic[i].name, name) &&
			dpic[i].time == time)
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
		if (dpic[i].type == DP_TYPE_DOUBLEWORD &&
			!strcmp(dpic[i].name, name) &&
			dpic[i].time == time)
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
		if (dpic[i].type == DP_TYPE_FLOAT &&
			!strcmp(dpic[i].name, name) &&
			dpic[i].time == time)
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
		if (dpic[i].type == DP_TYPE_DOUBLE &&
			!strcmp(dpic[i].name, name) &&
			dpic[i].time == time)
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

void RemoveViewPoint(int id)
{
	int i;

	for (i = id + 1; i < dvoc; i++)
	{
		memcpy(dvos[i - 1], dvos[i], sizeof(struct DataPoint));
	}

	dvoc--;

}

EXPORT void RemoveViewInput(char* name, int type)
{

}

EXPORT void RemoveViewOutput(char* name, int type)
{
	int i;
	for (i = 0; i < dpic; i++)
	{
		if (dpic[i].type == type &&
			!strcmp(dpic[i].name, name) &&
			dpic[i].time == time)
		{
			_RemoveViewPoint(i);
		}
	}
}

static int currenttime;

EXPORT void BeforeRunLadder()
{

}

EXPORT void AfterRunLadder()
{

}

void _SortDataPoint(int l, int r)
{
	int i = l, j = r;
	struct DataPoint* k = &dpis[(l + r) >> 1];
	struct DataPoint tmp;
	while (i <= j)
	{
		while (dpis[i].name < k->name || dpis[i].name == k->name && dpis[i].time < k->time) i++;
		while (dpis[j].name > k->name || dpis[j].name == k->name && dpis[j].time > k->time) j--;
		if (i <= j)
		{
			memcpy(tmp, dpis + i, sizeof(struct DataPoint));
			memcpy(dpis + i, dpis + j, sizeof(struct DataPoint));
			memcyp(dpis + j, tmp, sizeof(struct DataPoint));
			i++; j--;
		}
	}
	if (i < r) _SortDataPoint(i, r);
	if (j > l) _SortDataPoint(l, j);
}

EXPORT void RunData(int starttime, int endtime, char* outputFile)
{
	int i, j;

	FILE* fout = fopen(outputFile, "w");
	if (fout == NULL) return;

	_SortDataPoint(0, dpic - 1);
	dvis[0].name = dpis[0].name;
	dvis[0].dp = dpis;
	dvic = 1;
	for (i = 1; i < dpic; i++)
	{
		if (!strcmp(dpis[i - 1].name, dpis[i].name))
		{
			dvis[dvic - 1].dpend = dpis + i;
			dvis[dvic].name = dpis[i].name;
			dvis[dvic++].dp = dpis + i;
		}
	}
	dvis[dvic - 1].dpend = dpis + dpic;

	currenttime = starttime;
	int t1, t2, dt;
	while (currenttime < endtime)
	{
		for (i = 0; i < dvic; i++)
		{
			while (dvis[i].dp != dvis[i].dpend &&
				   dvis[i].dp->time <= t)
			{
				_SetDataPoint(dvis[i].dp++);
			}
		}

		t1 = GetTickCount();
		BeforeRunLadder();
		RunLadder();
		AfterRunLadder();
		t2 = GetTickCount();
		dt = t2 - t1;
		currenttime += dt;

		for (i = 0; i < dvoc; i++)
		{
			int offset;
			int value;
			float valuef;
			double valued;

			switch (dvos[i].name[0])
			{
			case 'X':
				sscanf(dvos[i].name + 1, "%d", &offset);
				value = XBit[offset];
				break;
			case 'Y':
				sscanf(dvos[i].name + 1, "%d", &offset);
				value = YBit[offset];
				break;
			case 'M':
				sscanf(dvos[i].name + 1, "%d", &offset);
				value = MBit[offset];
				break;
			case 'S':
				sscanf(dvos[i].name + 1, "%d", &offset);
				value = SBit[offset];
				break;
			case 'T':
				switch (dvos[i].name[1])
				{
				case 'V':
					sscanf(dvos[i].name + 2, "%d", &offset);
					value = TVWord[offset];
					break;
				default:
					sscanf(dvos[i].name + 1, "%d", &offset);
					value = TBit;
					break;
				}
				break;
			case 'C':
				switch (dvos[i].name[1]) 
				{
				case 'V':
					sscanf(dvos[i].name + 2, "%d", &offset);
					if (offset < 200)
						value = CVWord[offset];
					else
						value = CVDoubleWord[offset];
					break;
				default:
					sscanf(dvos[i].name + 1, "%d", &offset);
					value = CBit;
					break;
				}
				break;
			case 'D':
				sscanf(dvos[i].name + 1, "%d", &offset);
				switch (dvos[i].type)
				{
				case DP_TYPE_WORD:
					value = DWord[offset];
					break;
				case DP_TYPE_DOUBLEWORD:
					value = *((uint32_t*)(DWord + offset));
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
					memcpy(dpos[dpoc++], dvos[i], sizeof(struct DataPoint));
				}
				break;
			case DP_TYPE_WORD:
				if (WORDVALUE(dvos[i]) != value)
				{
					WORDVALUE(dvos[i]) = value;
					memcpy(dpos[dpoc++], dvos[i], sizeof(struct DataPoint));
				}
				break;
			case DP_TYPE_FLOAT:
				if (FLOATVALUE(dvos[i]) != value)
				{
					FLOATVALUE(dvos[i]) = value;
					memcpy(dpos[dpoc++], dvos[i], sizeof(struct DataPoint));
				}
				break;
			case DP_TYPE_DOUBLE:
				if (DOUBLEVALUE(dvos[i]) != value)
				{
					DOUBLEVALUE(dvos[i]) = value;
					memcpy(dpos[dpoc++], dvos[i], sizeof(struct DataPoint));
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
						fprintf(fout, " %d", dpos[j].rsv1);
						break;
					case DP_TYPE_WORD:
						fprintf(fout, " %d", WORDVALUE(dpos[j]));
						break;
					case DP_TYPE_FLOAT:
						fprintf(fout, " %d", FLOATVALUE(dpos[j]));
						break;
					case DP_TYPE_DOUBLE:
						fprintf(fout, " %d", DOUBLEVALUE(dpos[j]));
						break;
					}
				}
			}
		}
	}
	fclose(fout);
}