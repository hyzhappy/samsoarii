using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;

namespace SamSoarII.LadderInstViewModel
{
    public class CALLMViewModel : OutputRectBaseViewModel
    {
        private CALLMModel _model;
        public WordValue Value1
        {
            get
            {
                return _model.Value1;
            }
            set
            {
                _model.Value1 = value;
                MiddleTextBlock3.Text = _model.Value1.ToShowString();
            }
        }
        public BitValue Value2
        {
            get
            {
                return _model.Value2;
            }
            set
            {
                _model.Value2 = value;
                BottomTextBlock.Text = _model.Value2.ToShowString();
            }
        }
        public string FunctionName
        {
            get
            {
                return _model.FunctionName;
            }
            set
            {
                _model.FunctionName = value;
                MiddleTextBlock2.Text = string.Format("CALL : {0}",_model.FunctionName);
            }
        }
        public override BaseModel Model
        {
            get
            {
                return _model;
            }
            protected set
            {
                Model = value as CALLMModel;
                FunctionName = _model.FunctionName;
                Value1 = _model.Value1;
                Value2 = _model.Value2;
            }
        }
        public override string InstructionName { get { return "CALLM"; } }
        public override BaseViewModel Clone()
        {
            return new CALLMViewModel();
        }

        public override int GetCatalogID()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetValueString()
        {
            throw new NotImplementedException();
        }
        public override bool CheckValueStrings(List<string> valueStrings)
        {
            throw new NotImplementedException();
        }
        public override void ParseValue(List<string> valueStrings)
        {
            throw new NotImplementedException();
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            throw new NotImplementedException();
        }
    }
}
