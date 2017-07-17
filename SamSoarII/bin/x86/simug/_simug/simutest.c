#include "simulib.h"

extern int currenttime;
extern int beforetime;
extern int aftertime;
extern int deltatime;
extern int counttime;
extern double innertimerate;

//extern void SetClockRate(int rate);
//extern void RunLadder();
//extern void BeforeRunLadder();
//extern void AfterRunLadder();

void RunLadder()
{
}

void main(int argc, char** argv)
{
	SetClockRate(50);
	while (1)
	{
		BeforeRunLadder();
		printf("currenttime = %d\n", currenttime);
		printf("beforetime = %d\n", beforetime);
		printf("aftertime = %d\n", aftertime);
		printf("deltatime = %d\n", deltatime);
		printf("counttime = %d\n", counttime);
		printf("innertimerate = %.3lf\n", innertimerate);
		RunLadder();
		AfterRunLadder();
		system("PAUSE");
	}
}