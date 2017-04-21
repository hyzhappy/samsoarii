#include <stdint.h>
#include <math.h>
#include <time.h>
//#include <Windows.h>

//#include "simuc.h"
//#include "simuf.h"
#include "simulib.h"

// externed from simuc.h
// Bit Number of WORD
extern int BaseBit;
// Register Memorys
extern int32_t XBit[128];
extern int32_t YBit[128];
extern int32_t MBit[256 << 5];
extern int32_t CBit[256];
extern int32_t TBit[256];
extern int32_t SBit[32 << 5];
extern int32_t DWord[8192];
extern int32_t CVWord[200];
extern int64_t CVDoubleWord[56];
extern int32_t TVWord[256];
// Register writeable
// Set 1 if you want to modify the value of register
// othervise, Set 0 to lock the register and make it constant
extern int32_t XEnable[128];
extern int32_t YEnable[128];
extern int32_t MEnable[256 << 5];
extern int32_t CEnable[256];
extern int32_t TEnable[256];
extern int32_t SEnable[32 << 5];
extern int32_t DEnable[8192];
extern int32_t CVEnable[256];
extern int32_t TVEnable[256];
// counttime
extern int32_t counttimems;
extern int32_t counttime;

/*
typedef short int32_t;
typedef int int32_t;
typedef long int64_t;
*/
// temporary variable
static int32_t _tempw;
static int64_t _tempd;
static double _tempf;
// interrupt
static vfun itr_funcs[8]
static int32_t itr_times[2];
static int32_t itr_oldXBit[2];
// real timer
static struct tm *rtimer;
static struct tk _rtimer;
static time_t _tiem_t_old;
static time_t _tiem_t_new;
// pulse
static int32_t YTime[4];
static double TCount[4];
static int64_t YTarget[4];
static int32_t YITime[4];
static int32_t YZStatus[4];
static int32_t YZTime[4];
static int64_t YZFeq[4];
static int64_t YFeq[4];
static int64_t YPTarget[4];

// get a result (32-bit integer) by add a couple of 64-bit integers
int32_t _addw(int32_t ia, int32_t ib)
{
	int32_t ic = ia + ib;
	uint32_t _ia = (uint32_t)(ia);
	uint32_t _ib = (uint32_t)(ib);
	uint32_t _ic = (uint32_t)(ic);
	MBit[8169] = (_ic < _ia + _ib);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// get a result (64-bit integer) by add a couple of 64-bit integers
int64_t _addd(int64_t id, int64_t ib)
{
	int64_t ic = ia + ib;
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

// ...
int32_t _subw(int32_t ia, int32_t ib)
{
	int32_t ic = ia - ib;
	uint32_t _ia = (uint32_t)(ia);
	uint32_t _ib = (uint32_t)(ib);
	uint32_t _ic = (uint32_t)(ic);
	MBit[8169] = (_ia < _ib + _ic);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// ...
int64_t _subd(int64_t ia, int64_t ib)
{
	int64_t ic = ia - ib;
	uint64_t _ia = (uint64_t)(ia);
	uint64_t _ib = (uint64_t)(ib);
	uint64_t _ic = (uint64_t)(ic);
	MBit[8169] = (_ic < _ia + _ib);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// ...
double _subf(double id, double ib)
{
	double ic = (ia - ib);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// ...
int64_t _mulwd(int32_t ia, int32_t ib)
{
	int64_t _ia = (int64_t)(ia);
	int64_t _ib = (int64_t)(ib);
	int64_t ic = _ia * _ib;
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// ...
int32_t _mulww(int32_t ia, int32_t ib)
{
	int32_t ic = ia * ib;
	uint32_t _ia = (uint32_t)(ia < 0 ? -ia : ia);
	uint32_t _ib = (uint32_t)(ib < 0 ? -ib : ib);
	uint32_t max = 0x3fffffff;
	MBit[8169] = (max / _ia < _ib);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// ...
int64_t _muldd(int64_t ia, int64_t ib)
{
	int64_t ic = ia * ib;
	uint64_t _ia = (uint32_t)(ia < 0 ? -ia : ia);
	uint64_t _ib = (uint32_t)(ib < 0 ? -ib : ib);
	uint64_t max = 0x3fffffff;
	MBit[8169] = (max / _ia < _ib);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// ...
double _mulff(double ia, double ib)
{
	double ic = ia * ib;
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// ...
int64_t _divwd(int32_t ia, int32_t ib)
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
		int64_t ic = 0;
		int32_t div = ia / ib;
		int32_t mod = ia % ib;
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

// ...
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
		MBit[8169] = 0;
		MBit[8170] = (ic < 0);
		MBit[8171] = (ic == 0);
		MBit[8172] = 0;
		return ic;
	}
}

// ...
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
		MBit[8169] = 0;
		MBit[8170] = (ic < 0);
		MBit[8171] = (ic == 0);
		MBit[8172] = 0;
		return ic;
	}
}

// ...
int32_t _incw(int32_t ia)
{
	int32_t ic = ia + 1;
	MBit[8169] = (ia > 0 && ic < 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// ...
int64_t _incd(int64_t ia)
{
	int64_t ic = ia + 1;
	MBit[8169] = (ia > 0 && ic < 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// ...
int32_t _decw(int32_t ia)
{
	int32_t ic = ia - 1;
	MBit[8169] = (ia < 0 && ic > 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// ...
int64_t _decd(int64_t ia)
{
	int64_t ic = ia - 1;
	MBit[8169] = (ia < 0 && ic > 0);
	MBit[8170] = (ic < 0);
	MBit[8171] = (ic == 0);
	return ic;
}

// ...
double _sin(double ia)
{
	double ic = sin(ia);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// ...
double _cos(double ia)
{
	double ic = cos(ia);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// ...
double _tan(double ia)
{
	double ic = tan(ia);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// ...
double _ln(double ia)
{
	double ic = log(ia);
	MBit[8170] = (ic < 0.0);
	MBit[8171] = (ic == 0.0);
	return ic;
}

// ...
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
	MBit[8170] = ((ia >> 32) != 0);
	return ic;
}

// Convert 64-Bit integer to 32-Bit integer 
int64_t _DWORD_to_WORD(int32_t ia)
{
	int64_t ic = (int64_t)(ia);
	return ic;
}

// Convert 64-Bit integer to 64-Bit float
double _DWORD_to_FLOAT(int64_t ia)
{
	double ic = (double)(ia);
	return ic;
}

// Convert 64-Bit float to 64-Bit integer by ROUND strategy
int64_t _FLOAT_to_ROUND(double ia)
{
	int64_t ic = round(ia);
	return ic;
}

// Convert 64-Bit float to 64-Bit integer by TRUNC strategy
int64_t _FLOAT_to_TRUNC(double ia)
{
	int64_t ic = (int64_t)(ia);
	return ic;
}

// Convert 16-Bit integer to BCD code
int32_t _WORD_to_BCD(int32_t itg)
{
	return (itg % 10) | ((itg / 10 % 10) << 4) | ((itg / 100 % 10) << 8) | ((itg / 1000 % 10) << 12);
}

// Convert BCD code to 16-Bit integer
int32_t _BCD_to_WORD(int32_t BCD)
{
	return ((BCD >> 12) & 15) * 1000 + ((BCD >> 8) & 15) * 100 + ((BCD >> 4) & 15) * 10 + (BCD & 15);
}

// shift 32-Bit integer to right
uint32_t _shrw(uint32_t ia, uint32_t ib)
{
	uint32_t ic = (ia >> ib);
	MBit[8166] = ib <= 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}

// ...
uint64_t _shrd(uint64_t ia, uint64_t ib)
{
	uint64_t ic = (ia >> ib);
	MBit[8166] = ib <= 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}

// ...
uint32_t _shlw(uint32_t ia, uint32_t ib)
{
	uint32_t ic = (ia << ib);
	MBit[8166] = ib <= 0 ? 0 : ((ia << (ib - 1)) & (1<<31));
	MBit[8167] = (ic == 0);
	return ic;
}

// ...
uint64_t _shld(uint64_t ia, uint64_t ib)
{
	uint64_t ic = (ia << ib);
	MBit[8166] = ib <= 0 ? 0 : ((ia << (ib - 1)) & (1L<<63));
	MBit[8167] = (ic == 0);
	return ic;
}

// ...
uint32_t _rorw(uint32_t ia, uint32_t ib)
{
	ib %= 32;
	uint32_t ic = (ia >> ib) | (ia << (32 - ib));
	MBit[8166] = ib == 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}


// ...
uint64_t _rord(uint64_t ia, uint64_t ib)
{
	ib %= 64;
	uint64_t ic = (ia >> ib) | (ia << (64 - ib));
	MBit[8166] = ib == 0 ? 0 : ((ia >> (ib - 1)) & 1);
	MBit[8167] = (ic == 0);
	return ic;
}

// ...
uint32_t _rolw(uint32_t ia, uint32_t ib)
{
	ib %= 32;
	uint32_t ic = (ia << ib) || (ia >> (32 - ib));
	MBit[8166] = ib == 0 ? 0 : ((ia << (ib - 1)) & (1<<31));
	MBit[8167] = (ic == 0);
	return ic;
}

// ...
uint64_t _rolw(uint64_t ia, uint64_t ib)
{
	ib %= 32;
	uint64_t ic = (ia << ib) || (ia >> (64 - ib));
	MBit[8166] = ib == 0 ? 0 : ((ia << (ib - 1)) & (1L<<63));
	MBit[8167] = (ic == 0);
	return ic;
}

// shift a series of BITs to left, and fill the blank block from input BITs (keep order)
void _bitshl(int32_t& input, int32_t* addr, int32_t* enable, int32_t size, int32_t move)
{
	while (--size >= move)
	{
		if (enable[size])
			addr[size] = addr[size - move];
	}
	while (--size >= 0) {
		if (enable[size])
			addr[size] = input[size];
	}
}

// shift a series of BITs to right, and fill the blank block from input BITs (keep order)
void _bitshr(int32_t& input, int32_t* addr, int32_t* enable, int32_t size, int32_t move)
{
	int i = 0;
	while (i++ < size - move)
	{
		if (enable[i])
			addr[i] = addr[i + move];
	}
	while (i++ < size) {
		if (enable[i])
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
		if (enable[size]) addr[size] = 1;
	}
}

/*
	Reset a series of BIT to 0
	addr : base address
	size : series size
*/
void _bitrst(int32_t* addr, int32_t enable, int32_t size)
{
	while (--size >= 0)
	{
		if (enable[size]) addr[size] = 0;
	}
}

/*
	Duplicate a series of BIT to the targeted memory space
	
	source_addr : the base address of the source datas
	target_addr : the base address of the targeted datas
	size : series size
*/
void _bitcpy(int32_t* source_addr, int32_t* target_addr, int32_t size)
{
	while (--size >= 0)
	{
		if (enable[size])
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
		// arrive at the reserve value?
		if (*tval >= sv)
		{
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
			// set the counter bit
			*tbit = 1;
		}
	}
}

// attach an interrupt function
void _atch(int id, vfun func)
{
	itr_func[id] = func;
	if (id >= 6) itr_time[id - 6] = counttimems;
}

// detach an interrupt function
void _dtch(int id)
{
	itr_func[id] = NULL;
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
	int32_t _X0 = XBit[0];
	// interrupt id 0 : X0 up edge
	if (!itr_oldXBit[0] && _X0 &&
		itr_func[0])
	{
		itr_func[0]();
	}
	// interrupt id 1 : X0 down edge
	if (itr_oldXBit[0] && !_X0 &&
		itr_func[1])
	{
		itr_func[1]();
	}
	// interrupt id 2 : X0 all edge
	if (!itr_oldXBit[0] && _X0 || itr_oldXBit[0] && !_X0
		itr_func[2])
	{
		itr_func[2]();
	}
	int32_t _X1 = XBit[0];
	// interrupt id 3 : X0 up edge
	if (!itr_oldXBit[1] && _X1 &&
		itr_func[3])
	{
		itr_func[3]();
	}
	// interrupt id 4 : X0 down edge
	if (itr_oldXBit[1] && !_X1 &&
		itr_func[4])
	{
		itr_func[4]();
	}
	// interrupt id 5 : X0 all edge
	if (!itr_oldXBit[1] && _X1 || itr_oldXBit[1] && !_X1
		itr_func[5])
	{
		itr_func[5]();
	}
	// interrupt id 6 : timer1 arrive
	while (itr_func[6] && DWord[8173] > 0 &&
		counttime - itr_timer[0] > DWord[8173])
	{
		itr_timer[0] += DWord[8173];
		itr_func[6]();
	}
	// interrupt id 6 : timer1 arrive
	while (itr_func[7] && DWord[8174] > 0 &&
		counttime - itr_timer[1] > DWord[8174])
	{
		itr_timer[1] += DWord[8174];
		itr_func[7]();
	}
}

void _trd(int32_t* d)
{
	_time_t = time(NULL);
	rtimer = localtime(_time_t - _time_t_new + _time_t_old);
	d[0] = _WORD_to_BCD(rtimer->tm_year);
	d[1] = _WORD_to_BCD(rtimer->tm_mon);
	d[2] = _WORD_to_BCD(rtimer->tm_mday);
	d[3] = _WORD_to_BCD(rtimer->tm_hour);
	d[4] = _WORD_to_BCD(rtimer->tm_min);
	d[5] = _WORD_to_BCD(rtimer->tm_sec);
	d[6] = 0;
	d[7] = _WORD_to_BCD(rtimer->tm_week);
	free(rtimer);
}

void _twr(int32_t* d)
{
	rtimer = &_rtimer;
	rtimer->tm_year = _BCD_to_WORD(d[0]);
	rtimer->tm_mon = _BCD_to_WORD(d[1]);
	rtimer->tm_mday = _BCD_to_WORD(d[2]);
	rtimer->tm_hour = _BCD_to_WORD(d[3]);
	rtimer->tm_min = _BCD_to_WORD(d[4]);
	rtimer->tm_sec = _BCD_to_WORD(d[5]);
	rtimer->tm_week = _BCD_to_WORD(d[7]);
	_time_t_old = time(NULL);
	_time_t_new = mktime(rtimer);
}

void _mbus(uint32_t com_id, struct ModbusTable* table, uint32_t* wr){}

void _send(uint32_t com_id, int32_t* addr, int32_t len){}

void _rev(uint32_t com_id, int32_t* addr, int32_t len){}

void _plsf(uint32_t feq, int32_t* out)
{
	_dplsf((uint64_t)(feq), out);
}

void _dplsf(uint64_t feq, int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		uint32_t* _of = (&MBit[8118 + id]);
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		if (YTime[id] == 0)
		{
			YTime[id] = counttimems;
			YCount[id] = 0.0;
			*ct = 0;
		}
		else
		{
			YFeq[id] = feq;
			uint64_t _td = counttimems - YTime[id];
			double _pn = (double(td) * feq) / 1000;
			double _yc = YCount[id] + _pn;
			*_of = (_yc > ~((uint64_t)(0)));
			if (!*_of) *_ct = (uint64_t)(_yc);
			if (counttimems - YTime[id] >= 10)
			{
				YTime[id] += 10;
				YCount[id] = _yc;
			}
		}
	}
}

void _pwm(uint32_t feq, uint32_t dc, int32_t* out)
{
	_plsf(feq, out);
}

void _dpwm(uint64_t feq, uint64_t dc, int32_t* out)
{
	_dplsf(feq, out);
}

void _plsy(uint32_t feq, uint32_t* pn, int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		_plsf(feq, out);
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		*pn = (uint32_t)(*_ct);
	}
}

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

void _plsr(uint32_t* msg, uint32_t it, int32_t* out)
{

}

void _dplsr(uint64_t* msg, uint64_t it, int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		uint32_t* _of = &MBit[8118 + id];
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		uint32_t* _active = &MBit[8134 + id];
		uint32_t* _igerr = &MBit[8070 + id];
		uint32_t* _iserr = &MBit[8086 + id];
		uint32_t* _err = &DWord[8176];
		uint32_t* _YIndex = &DWord[8124 + id];
		uint32_t* _errid = &DWord[8108 + id];
		uint64_t _feq_old;
		uint64_t _feq_new;
		uint64_t _pn_itv;
		uint64_t _pn;
		if (!*_active)
		{
			if (*_err && !*_igerr)
			{
				return;
			}
			active = 1;
			if (!*_igerr)
			{
				*_errid = 1; *_err = 0;
				while (!*_err && *_errid < 2048)
				{
					_feq_old = (*_errid > 1 : msg[*_errid * 2 - 4] : 0);
					_feq_new = msg[*_errid * 2 - 2];
					_pn = msg[*_errid * 2 - 1];
					_pn_itv = (_feq_old + _feq_new) * it / 2;
					if (_pn > 1000000) *_err = 23;
					if (*_errid == 1 && _feq_new < 100) *_err = 24;
					if (_feq_new < 50) *_err = 25;
					if (_pn < 1000) *_err = 26;
					if (it < 100) *_err = 27;
					if (_feq_new == 0)
					{
						if (_pn_itv > _pn) *_err = 29;
						break;
					}
					if (_pn_itv > _pn) *_err = 28;
				}
				if (*_err)
				{
					*_iserr = 1;
					return;
				}
				*_errid = 0;
			}
			*_active = 1;
			*_YIndex = 1;
			*_ct = 0;
			YTarget[id] = msg[1];
			YITime[id] = counttimems;
			return;
		}
		_feq_old = (*_Yindex > 1 : msg[*_YIndex * 2 - 4] : 0);
		_feq_new = msg[*_Yindex * 2 - 2];
		_pn = msg[*_YIndex * 2 - 1];
		_pn_itv = (_feq_old + _feq_new) * it / 2;
		if (_feq_new == 0)
		{
			if (*_ct < YTarget[id] + _pn_itv)
			{
				_dplsf(_feq_old + (feq_new - feq_old) * (counttimems - YITime[id]) / it, out);
			}
			else
			{
				*_active = 0;
			}
		}
		if (*_ct < YTarget[id] + _pn_itv)
		{
			_dplsf(_feq_old + (feq_new - feq_old) * (counttimems - YITime[id]) / it, out);
		}
		else if (*_ct < YTarget[id] + _pn)
		{
			_dplsf(_feq_new, out);
		}
		else
		{
			*_YIndex++;
			YTarget[id] += _pn;
			YITime[id] = counttimems;
		}
	}
}

void _plsrd(uint32_t* msg, uint32_t ct, int32_t* out, int32_t* dir){}

void _dplsrd(uint64_t* msg, uint64_t ct, int32_t* out, int32_t* dir){}

void _plsnext(int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		uint32_t* _active = &MBit[8134 + id];
		uint32_t* _YIndex = &DWord[8124 + id];
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		if (*_active)
		{
			*_YIndex++;
			YTarget[id] = *_ct;
			YITime[id] = counttimems;
		}
	}
}

void _plsstop(int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		uint32_t *_active = &MBit[8134 + id];
		*_active = 0;
	}
}

void _zrn(uint32_t bv, uint32_t cv, int32_t sg, int32_t* out)
{
	_dzrn((uint64_t)(bv), (uint64_t)(cv), sg, out);
}

void _dzrn(uint64_t cv, uint64_t bv, int32_t sg, int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		uint32_t* _active = &MBit[8134 + id];
		uint32_t* _zt = &DWord[8076 + id];
		uint64_t _feq_old;
		uint64_t _feq_new;
		if (!*_active)
		{
			*_active = 1;
			YZTime[id] = counttimems;
		}
		switch (YZStatus[id])
		{
		case 0:
			if (sg)
			{
				YZStatus[id] = 1;		
			}
			else if (YZTime[id] - counttimems < *_zt) 
			{
				_dplsf(cv * (counttimems - YITime[id]) / (*_zt), out);
			}
			else
			{
				_dplsf(cv, out);
			}
			break;
		case 1:
			if (!sg) 
			{
				YZStatus[id] = 0;
				*_active = 0;
			}
			else if (YZTime[id] - counttimems < *_zt)
			{
				_dplsf(bv + (cv - bv) * (counttimems - YITime[id]) / (*_zt), out);
			}
			else
			{
				_dplsf(bv, out);
			}
			break;
		}
	}
}

void _pto(uint64_t* msg, int32_t* out, int32_t* dir)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		uint32_t* _active = &MBit[8134 + id];
		uint32_t* _dir = &DWord[8102 + id];
		uint32_t* _YIndex = &DWord[8124 + id];
		uint32_t* _errid = &DWord[8108 + id];
		uint64_t msgct = msg[0];
		if (!*_active) 
		{
			*_active = 1;
			*_errid = 1;
			*_ct = 0;
			YPTarget[id] = msg[3];
			for (; *errid <= msgct; *errid++)
			{
				if (msg[*errid * 3] == 0)
				{
					break;
				}
			}
			if (*errid > msgct)
			{
				*errid = 0;
			}
			else
			{
				*_active = 0;
			}
			return;
		}
		if (*_YIndex > msgct)
		{
			*_active = 0;
			return;
		}
		uint64_t _feq_old = msg[*_YIndex * 3 - 2];
		uint64_t _feq_new = msg[*_YIndex * 3 - 1];
		uint64_t _pn = msg[*_YIndex * 3];
		if (*_ct < YPTarget[id])
		{
			_dplsf(_feq_old + (_feq_new - _feq_old) * sqrt(_pn - (*YPTarget[id] - *_ct)) / sqrt(_pn), out);
		}
		else
		{
			*_YIndex++;
		}
	}
}

void _drvi(uint32_t feq, uint32_t pn, int32_t* out)
{
	_ddrvi((uint64_t)(feq), (uint64_t)(pn), out);
}

void _ddrvi(uint64_t feq, uint64_t pn, int32_t* out)
{
	int id = (int)(out - &YBit[0]) / sizeof(int32_t);
	if (id < 4)
	{
		uint64_t* _ct = ((uint64_t*)(&DWord[8140])) + id;
		uint32_t* _active = &MBit[8134 + id];
		uint32_t* _iserr = &MBit[8086 + id];
		uint32_t* _igerr = &MBit[8070 + id];
		uint32_t* _err = &DWord[8176];
		uint32_t* _it = &DWord[8092 + id];
		uint64_t _feq_old;
		uint64_t _feq_new;
		uint64_t _pn;
		uint64_t _pn_itv;
		if (!*_active)
		{
			if (!*_igerr)
			{
				_feq_old = YFeq[id];
				_feq_new = feq;
				_pn = pn;
				_pn_itv = (_feq_old + _feq_new) * (*_it) / 2;
				if (_pn > 1000000) *_err = 23;
				if (*_errid == 1 && _feq_new < 100) *_err = 24;
				if (_feq_new < 50) *_err = 25;
				if (_pn < 1000) *_err = 26;
				if ((*_it) < 100) *_err = 27;
				if (_feq_new == 0)
				{
					if (_pn_itv > _pn) *_err = 29;
					break;
				}
				if (_pn_itv > _pn) *_err = 28;
				if (*_err)
				{
					*_iserr = 1;
					return;
				}
			}
			*active = 1;
			YDFeq[id] = YFeq[id];
			YDTime[id] = counttimems;
			return;
		}
		_feq_old = YDFeq[id];
		_feq_new = feq;
		_pn = pn;
		_pn_itv = (_feq_old + _feq_new) * (*_it) / 2;
	}
	if (*_ct < YTarget[id] + _pn_itv)
	{
		_dplsf(_feq_old + (feq_new - feq_old) * (counttimems - YDTime[id]) / it, out);
	}
	else if (*_ct < YTarget[id] + _pn)
	{
		_dplsf(_feq_new, out);
	}
	else
	{
		*_active = 0;
	}
}

void _hcnt(int64_t* cv, int64_t sv){}

double _log(double* ia)
{
	return log(ia) / log(10.0);
}

double _pow(double* ia, double* ib)
{
	return power(ia, ib);
}

// Calculate the factorial of the 16-bit integer
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

// Compare a couple of 16-bit integers(named as ia and ib)
// return 0 if ia == ib
// return -1 if ia < ib
// return 1 if ia > ib
int32_t _cmpw(int32_t ia, int32_t ib)
{
	// compare
	if (_ia == _ib)
	{
		return 0;
	}
	if (_ia < _ib)
	{
		return -1;
	}
	if (_ia > _ib)
	{
		return 1;
	}
	return 1;
}

// compare a couple of 32-bit integers(named as ia and ib)
// return 0 if ia == ib
// return -1 if ia < ib
// return 1 if ia > ib
int32_t _cmpd(int64_t ia, int64_t ib)
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
int32_t _cmpf(double ia, double ib)
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
int32_t _zcpw(int32_t ia, int32_t il, int32_t ir)
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
int32_t _zcpd(int64_t ia, int64_t il, int64_t ir)
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
int32_t _zcpf(double ia, double il, double ir)
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


int32_t _negw(int32_t ia)
{
	return -ia;
}

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

int32_t _cmlw(int32_t* ia) 
{
	return ~ia;
}

int64_t _cmld(int64_t* ia)
{
	return ~ib;
}

void _fmovw(int32_t* dest, int32_t* source, int32_t size)
{
	while (--size >= 0)
	{
		dest[size] = source[size];
	}
}

void _fmovd(int64_t* dest, int64_t* source, int32_t size)
{
	while (--size >= 0)
	{
		dest[size] = source[size];
	}
}

void _smov(int32_t source, int32_t sb1, int32_t sb2, int32_t* target, int32_t tb1)
{
	source = _WORD_to_BCD(source);
	*target = _WORD_to_BCD(*target);
	*target &= ~(((1 << ((sb2 - sb1 + 1) * 4)) - 1) << ((tb1 - 1) * 4));
	*target |= ((source >> ((sb1 - 1) * 4))&((1 << ((sb2 - sb1 + 1) * 4)) - 1)) << ((tb1 - 1) * 4);
	*target = _BCD_to_WORD(*target);
}