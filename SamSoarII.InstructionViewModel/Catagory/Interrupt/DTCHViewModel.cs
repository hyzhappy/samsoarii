using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.LadderInstModel.Interrupt;
using SamSoarII.ValueModel;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel.Interrupt
{
    public class DTCHViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "DTCH";
            }
        }

        public override BaseModel Model
        {
            get
            {
                return this.model;
            }

            protected set
            {
                if (value is DTCHModel)
                {
                    this.model = (DTCHModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except DTCHModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new DTCHViewModel();
        }

        public override int GetCatalogID()
        {
            return 1301;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(IDValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(IDValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                IDValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                IDValue = WordValue.Null;
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(1);
            //dialog.ShowLine3("FUNC:", FuncName);
            dialog.ShowLine4("EVENT:", IDValue);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = IDValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyIntKHValueRegex});
            if (check1)
            {
                IDValue = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                int id = int.Parse(IDValue.GetValue());
                if (id < 0 || id > 7)
                {
                    throw new ValueParseException("EV超出范围（0-7）");
                }
            }
            else
            {
                throw new ValueParseException("EV格式非法！");
            }
        }

        private DTCHModel model;
        
        public WordValue IDValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.IDValue;
            }
            set
            {
                if (model == null) return;
                model.IDValue = value;
                MiddleTextBlock2.Text = String.Format("EV:{0:s}", model.IDValue.ValueShowString);
            }
        }

        public DTCHViewModel()
        {
            TopTextBlock.Text = InstructionName;
            model = new DTCHModel();
        }
    }
}
