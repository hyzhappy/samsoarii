
int32_t _addw(int32_t ia, int32_t ib);

int64_t _addd(int64_t id, int64_t ib);

double _addf(double id, double ib);

int32_t _subw(int32_t ia, int32_t ib);

int64_t _subd(int64_t ia, int64_t ib);

double _subf(double id, double ib);

int64_t _mulwd(int32_t ia, int32_t ib);

int32_t _mulww(int32_t ia, int32_t ib);

int64_t _muldd(int64_t ia, int64_t ib);

double _mulff(double ia, double ib);

int64_t _divwd(int32_t ia, int32_t ib);

int32_t _divww(int32_t ia, int32_t ib);

int64_t _divdd(int64_t ia, int64_t ib);

int32_t _incw(int32_t ia);

int64_t _incd(int64_t ia);

int32_t _decw(int32_t ia);

int64_t _decd(int64_t ia);

double _sin(double ia);

double _cos(double ia);

double _tan(double ia);

double _ln(double ia);

double _exp(double ia);

int32_t _DWORD_to_WORD(int64_t ia);

int64_t _WORD_to_DWORD(int32_t ia);

int32_t _WORD_to_BCD(int32_t itg);

int32_t _BCD_to_WORD(int32_t BCD);

double _DWORD_to_FLOAT(int64_t ia);

int64_t _FLOAT_to_ROUND(double ia);

int64_t _FLOAT_to_TRUNC(double ia);

int32_t _shrw(int32_t ia, int32_t ib);

int64_t _shrd(int64_t ia, int64_t ib);

int32_t _shlw(int32_t ia, int32_t ib);

int64_t _shld(int64_t ia, int64_t ib);

int32_t _rorw(int32_t ia, int32_t ib);

int64_t _rord(int64_t ia, int64_t ib);

int32_t _rolw(int32_t ia, int32_t ib);

int64_t _rold(int64_t ia, int64_t ib);

void _bitshl(int32_t& input, int32_t* addr, int32_t* enable, int32_t size, int32_t move);

void _bitshr(int32_t& input, int32_t* addr, int32_t* enable, int32_t size, int32_t move);

void _bitset(int32_t* addr, int32_t* enable, int32_t size);

void _bitrst(int32_t* addr, int32_t* enable, int32_t size);

void _bitcpy(int32_t* source_addr, int32_t* target_addr, int32_t* enable, int32_t size);

void _ton(int32_t* tval, int32_t* tbit, int32_t cod, int32_t sv, int32_t* otim);

void _tonr(int32_t* tval, int32_t* tbit, int32_t cod, int32_t sv, int32_t* otim);

typedef void(vfun)(void);

void _atch(vfun func, int id);

void _dtch(int id);

void _ei();

void _di();

void _itr_invoke();

void _trd(int32_t* d);

void _twr(int32_t* d);

struct ModbusTable
{
	uint32_t port;
	uint32_t code;
	uint32_t slave_reg;
	uint32_t slave_len;
	uint32_t maste_reg;
};

void _mbus(uint32_t com_id, struct ModbusTable* table, uint32_t* wr);

void _send(uint32_t com_id, int32_t* addr, int32_t len);

void _rev(uint32_t com_id, int32_t* addr, int32_t len);

void _plsf(uint32_t feq, int32_t* out);

void _dplsf(uint64_t feq, int32_t* out);

void _pwm(uint32_t feq, uint32_t dc, int32_t* out);

void _dpwm(uint64_t feq, uint64_t dc, int32_t* out);

void _plsy(uint32_t feq, uint32_t* pn, int32_t* out);

void _dplsy(uint64_t feq, uint64_t* pn, int32_t* out);

void _plsr(uint32_t* msg, uint32_t ct, int32_t* out);

void _dplsr(uint64_t* msg, uint64_t ct, int32_t* out);

void _plsrd(uint32_t* msg, uint32_t ct, int32_t* out, int32_t* dir);

void _dplsrd(uint64_t* msg, uint64_t ct, int32_t* out, int32_t* dir);

void _plsnext(int32_t* out);

void _plsstop(int32_t* out);

void _zrn(int32_t bv, int32_t cv, int32_t* sg, int32_t* out);

void _dzrn(int64_t bv, int64_t cv, int32_t* sg, int32_t* out);

void _pto(int32_t* msg, int32_t* out, int32_t* dir);

void _drvi(int32_t feq, int32_t pn, int32_t* out);

void _ddrvi(int64_t feq, int64_t pn, int32_t* out);

void _hcnt(int64_t* cv, int64_t sv);

double _log(double* ia);

double _pow(double* ia, double* ib);

int64_t _fact(int32_t ia);

int32_t _cmpw(int32_t ia, int32_t ib);

int32_t _cmpd(int64_t ia, int64_t ib);

int32_t _cmpf(double ia, double ib);

int32_t _zcpw(int32_t ia, int32_t il, int32_t ir);

int32_t _zcpd(int32_t ia, int32_t il, int32_t ir);

int32_t _zcpf(double ia, double il, double ir);

int32_t _negw(int32_t ia);

int64_t _negd(int64_t ia);

void _xchw(int32_t* pa, int32_t* pb);

void _xchd(int64_t* pa, int64_t* pb);

void _xchf(double* pa, double* pb);

int32_t _cmlw(int32_t* ia);

int64_t _cmld(int64_t* ia);

void _smov(int32_t source, int32_t sb1, int32_t sb2, int32_t* target, int32_t tb1);

void _fmovw(int32_t* s, int32_t* t, int32_t len);

void _fmovd(int64_t* s, int64_t* t, int32_t len);