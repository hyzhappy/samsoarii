using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace SamSoarII.CompilerCommand
{
    class FGs16MRCompiler : BaseCompiler
    {       
        public FGs16MRCompiler()
        {
            //string objfile1 = string.Format(@"{0}\mems.o", CompilerUtility.FGs16R_Base_Dir);
            string objfile = string.Format(@"{0}\startup.o", CompilerUtility.FGs16MR_Base_Dir);
            CC_FLAGS = string.Format("-mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -munaligned-access -std=gnu11 -O2 -fsigned-char -ffunction-sections -fdata-sections -ffreestanding -fsingle-precision-constant -c -I{0}", CompilerUtility.FGs16MR_Base_Dir);
            CXX_FLAGS = string.Format("-mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -munaligned-access -std=c++11 -O2 -fsigned-char -ffunction-sections -fdata-sections -ffreestanding -fsingle-precision-constant -c -I{0}", CompilerUtility.FGs16MR_Base_Dir);
            LD_FLAGS = string.Format("-mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -munaligned-access -fsigned-char -ffunction-sections -fdata-sections -ffreestanding -fsingle-precision-constant -nostartfiles -Xlinker -gc-section -T mem.ld -T sections.ld -T libs.ld {1} -lFGs16MR -L {0} --specs=nosys.specs", CompilerUtility.FGs16MR_Base_Dir, objfile);
            OBJCOPY_FLAGS = string.Format(" -O binary ");
            _compileBehavior = new CortexMBehavior(this);           
        }
    }
}
