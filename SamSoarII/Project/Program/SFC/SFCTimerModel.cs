using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.Project.Program.SFC
{
    public class SFCTimerModel
    {
        private TONModel model;
        public TONModel Model
        {
            get
            {
                return this.model;
            }
            set
            {
                this.model = value;
            }
        }
        
        public int Index
        {
            get
            {
                return (int)(model.TimerValue.Index);
            }
        }
        
        public string Limit
        {
            get
            {
                return model.EndValue.ValueString;
            }
        }

        public SFCTimerModel(TONModel _model)
        {
            model = _model;
        }
    }
}
