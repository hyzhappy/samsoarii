..\\Compiler\\arm\\bin\\arm-none-eabi-gcc -mcpu=cortex-m3 -mfloat-abi=soft -mthumb -munaligned-access -std=gnu11 -O2 -fsigned-char -ffunction-sections -fdata-sections -ffreestanding -fsingle-precision-constant -c downc.c -o build\\downc.o
..\\Compiler\\arm\\bin\\arm-none-eabi-gcc -mcpu=cortex-m3 -mfloat-abi=soft -mthumb -munaligned-access -std=gnu11 -O2 -fsigned-char -ffunction-sections -fdata-sections -ffreestanding -fsingle-precision-constant -c downf.c -o build\\downf.o
..\\Compiler\\arm\\bin\\arm-none-eabi-gcc -mcpu=cortex-m3 -mfloat-abi=soft -mthumb -munaligned-access -std=gnu11 -O2 -fsigned-char -ffunction-sections -fdata-sections -ffreestanding -fsingle-precision-constant -c downlib.c -o build\\downlib.o
..\\Compiler\\arm\\bin\\arm-none-eabi-gcc -mcpu=cortex-m3 -mfloat-abi=soft -mthumb -munaligned-access -std=gnu11 -O2 -fsigned-char -ffunction-sections -fdata-sections -ffreestanding -fsingle-precision-constant -nostartfiles -T mem.ld -T libs.ld -T sections.ld -Lldscripts -Xlinker -gc-sections --specs=nano.specs -L .\\ -o ..\\downc.elf build\\downlib.o build\\downc.o build\\downf.o -Wl,-Map,aa.map -Wl,--whole-archive -lF103PLC -larm_cortexM3l_math -Wl,--no-whole-archive -lm
..\\Compiler\\arm\\bin\\arm-none-eabi-objcopy -O ihex ..\\downc.elf ..\\downc.hex
..\\Compiler\\arm\\bin\\arm-none-eabi-objcopy -O binary ..\\downc.elf ..\\downc.hex
..\\Compiler\\arm\\bin\\arm-none-eabi-objcopy -O ihex ..\\downc.elf ..\\downc.bin
..\\Compiler\\arm\\bin\\arm-none-eabi-objcopy -O binary ..\\downc.elf ..\\downc.bin
