using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain
{
    public class LadderElementChangedArgs
    {
        /*
        public InstModel IModel_old { get; set; }
        public InstModel IModel_new { get; set; }
        public InstModel IModel_before { get; set; }
        public InstModel IModel_after { get; set; }
        */
        public BaseViewModel BVModel_old { get; set; }
        public BaseViewModel BVModel_new { get; set; }
    }
}
