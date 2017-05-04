#include "simuf.h"
#include<assert.h>

void MIN(_WORD* inputs, _WORD* count, _WORD* output)
{
	int result = 0x3fffffff;
	int i = 0;
	for (i = 0 ; i < *count ; i++)
	{
		if (inputs[i] < result)
		{
			result = inputs[i];
		}
	}
	*output = result;
}

void MAX(_WORD* inputs, _WORD* count, _WORD* output)
{
	int result = -0x3fffffff;
	int i = 0;
	for (i = 0 ; i < *count ; i++)
	{
		if (inputs[i] > result)
		{
			result = inputs[i];
		}
	}
	*output = result;
}

void AVE(_WORD* input, _WORD* count, _WORD* output)
{
	int result = 0;
	int i = 0;
	for (i = 0 ; i < *count ; i++)
	{
		result += input[i];
	}
	result /= *count;
	*output = result;
}

void STACK_OVERFLOW()
{
	STACK_OVERFLOW();
}

void ASSERT_TEST()
{
	int i = 0;
	assert(i);
}
