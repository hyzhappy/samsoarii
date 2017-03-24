#include "simuf.h"
void MFUNC1(BIT B, WORD W)
{
	int i = 0;
	int c1, c2, c3;

	for ( ; i < 8 ; i++)
	{
		c1 = ((W[0]>>i)&1);
		c2 = ((W[1]>>i)&1);
		c3 = ((W[2]>>i)&1);
		if (!c1 && !c2 && !c3)
		{
			W[3] = i+1;
			W[4] = W[0];
			W[5] = (W[1]<<1);
			W[6] = (W[2]>>1);
			MFUNC1(B, W+4);
			if (W[7]) return;
		}
	}
	W[3] = 0;
}