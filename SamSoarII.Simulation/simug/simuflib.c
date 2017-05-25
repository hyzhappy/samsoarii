#define PI 3.1415926

// 计算整数半径的圆的面积
void CACI(WORD* in, FLOAT* out)
{
	*out = (*in) * (*in) * PI;
}

// 计算整数半径的圆的周长
void RPCI(WORD* in, FLOAT* out)
{
	*out = (*in) * 2 * PI;
}

// 计算浮点半径的圆的面积
void CACF(FLOAT* in, FLOAT* out)
{
	*out = (*in) * (*in) * PI;
}

// 计算浮点半径的圆的周长
void RPCF(FLOAT* in, FLOAT* out)
{
	*out = (*in) * 2 * PI;
}

// 反三角函数
void ITF(FLOAT* in, FLOAT* out)
{
	out[0] = asinf(in[0]);
	out[1] = acosf(in[1]);
	out[2] = atanf(in[2]);
}

// 求阶乘
void FAC(WORD* in, WORD* out)
{
	WORD _in = *in;
	WORD ans = _in;
	while (--_in)
	{
		ans *= _in;
	}
	return ans;
}

// 整数底(d)整数幂(e)的幂运算
void PCI(WORD* d, WORD* e, WORD* out)
{
	WORD ans = 1;
	WORD _d = *d;
	WORD _e = *e;
	while (_e)
	{
		if (_e&1) ans *= _d;
		_d <<= 1;
		_e >>= 1;
	}
	*out = ans;
}

// 浮点底(d)整数幂(e)的幂运算
void PCF(FLOAT* d, WORD* e, FLOAT* out)
{
	FLOAT ans = 1.0;
	WORD _d = *d;
	WORD _e = *e;
	while (_e)
	{
		if (_e&1) ans *= _d;
		_d <<= 1;
		_e >>= 1;
	}
	*out = ans;
}

// 以10为底的对数
void NL10(FLOAT* in, FLOAT* out)
{
	*out = log10(*in);
}

// 以e为底的对数
void NLO(FLOAT* in, FLOAT* out)
{
	*out = logf(*in);
}

// 求直角三角形的斜边
void ASSH(FLOAT* in1, FLOAT* in2, FLOAT* out)
{
	*out = sqrt((*in1) * (*in1) + (*in2) * (*in2));
}

// 求直角三角形的直角边
void FAAS(FLOAT* in1, FLOAT* in2, FLOAT* out)
{
	*out = sqrt((*in2) * (*in2) - (*in1) * (*in1));
}

// 求浮点一元二次方程
void QEF(FLOAT* in, FLOAT* out)
{
	FLOAT a = in[0];
	FLOAT b = in[1];
	FLOAT c = in[2];
	FLOAT x = in[3];
	*out = a * x * x + b * x + c;
}

// 求整数一元二次方程
void QEI(WORD* in, WORD* out)
{
	WORD a = in[0];
	WORD b = in[1];
	WORD c = in[2];
	WORD x = in[3];
	*out = a * x * x + b * x + c;
}

// 求浮点一元一次方程
void EIOF(FLOAT* in, FLOAT* out)
{
	FLOAT b = in[0];
	FLOAT c = in[1];
	FLOAT x = in[2];
	*out = b * x + c;
}

// 求整数一元一次方程
void EIOI(WORD* in, WORD* out)
{
	WORD b = in[0];
	WORD c = in[1];
	WORD x = in[2];
	*out = b * x + c;
}

// 求多个整数的和
void SUMI16(WORD* in, WORD* ct, WORD* out)
{
	*out = 0;
	int i = 0;
	for ( ; i < *ct ; i++)
	{
		*out += in[i];
	}
}

// 求多个浮点的和
void DSF32(FLOAT* in, WORD* ct, FLOAT* out)
{
	*out = 0;
	int i = 0; 
	for ( ; i < *ct ; i++)
	{
		*out += in[i];
	}
}

// 求多个整数的乘积
void MULI(WORD* in, WORD* ct, WORD* out)
{
	*out = 1;
	int i = 0;
	for ( ; i < *ct ; i++)
	{
		*out *= in[i];
	}
}

// 求多个浮点的乘积
void MDPF(FLOAT* in, WORD* ct, FLOAT* out)
{
	*out = 1;
	int i = 0; 
	for ( ; i < *ct ; i++)
	{
		*out *= in[i];
	}
}

// CRC校验生成
void CRCC(WORD* in, WORD* ct, WORD* out)
{
	int i, j, m, n;
	unsigned int crc_reg = 0xffff, k = 0;

	for (i=0; i<*ct; i++)
	{
		crc_reg ^= in[i];
		for (j=0; j<8; j++)
		{
			if (crc_reg&0x01)
				crc_reg = (crc_reg>>1)^0xa001;
			else
				crc_reg = crc_reg>>1;
		}
	}
	m 	  = *ct;
	n 	  = *ct + 1;
	k 	  = crc_reg&0xff00;
	in[n] = k>>8;
	in[m] = crc_reg&0x00ff;
}

// 从大到小排序
void DDO(WORD* in, WORD* ct)
{
	WORD l = 0, r = *ct - 1;
	WORD mid = in[*ct>>1];
	WORD tmp = 0;
	while (l <= r)
	{
		while (in[l] > mid) l++;
		while (in[r] < mid) r--;
		if (l <= r)
		{
			tmp   = in[l];
			in[l] = in[r];
			in[r] = tmp;
			l++; r--;
		}
	}
	WORD _ct = 0;
	if (l < *ct - 1)
	{
		_ct = *ct - l;
		DDO(in+l, &_ct);
	}
	if (r > 0)
	{
		_ct = r + 1;
		DDO(in, &_ct);
	}
}

// 从小到大排序
void DSL(WORD* in, WORD* ct)
{
	WORD l = 0, r = *ct - 1;
	WORD mid = in[*ct>>1];
	WORD tmp = 0;
	while (l <= r)
	{
		while (in[l] < mid) l++;
		while (in[r] > mid) r--;
		if (l <= r)
		{
			tmp   = in[l];
			in[l] = in[r];
			in[r] = tmp;
			l++; r--;
		}
	}
	WORD _ct = 0;
	if (l < *ct - 1)
	{
		_ct = *ct - l;
		DSL(in+l, &_ct);
	}
	if (r > 0)
	{
		_ct = r + 1;
		DSL(in, &_ct);
	}
}

