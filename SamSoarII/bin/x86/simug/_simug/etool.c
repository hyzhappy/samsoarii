#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

extern void Encode(char* ifile, char* ofile);

void main(int argc, char** argv)
{
	if (argc != 3)
	{
		return 1;
	}
	Encode(argv[1], argv[2]);
	return 0;
}