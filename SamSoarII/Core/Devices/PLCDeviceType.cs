using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice
{
    public enum PLC_FGs_Type : int
    {
        FGs_16MR_A,
        FGs_16MR_D,
        FGs_16MT_A,
        FGs_16MT_D,
        FGs_32MR_A,
        FGs_32MR_D,
        FGs_32MT_A,
        FGs_32MT_D,
        FGs_64MR_A,
        FGs_64MR_D,
        FGs_64MT_A,
        FGs_64MT_D,
        FGs_32MR_YTJ,
        FGs_32MT_YTJ,
        FGs_20MR_BYK
    }

    public enum PLC_FGm_Type : int
    {
        FGm_32MT_A = 15,
        FGm_48MT_A,
        FGm_64MT_A
    }
}
