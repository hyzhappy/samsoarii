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
    public class ATCHViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "ATCH";
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
                if (value is ATCHModel)
                {
                    this.model = (ATCHModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except ATCHModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new ATCHViewModel();
        }

        public override int GetCatalogID()
        {
            return 1300;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(IDValue.ValueString);
            result.Add(FuncName);
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
            FuncName = valueStrings[1];
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2, ElementPropertyDialog.INST_ATCH);
            dialog.ShowLine3("EVENT:", IDValue);
            dialog.ShowLine5("FUNC:");
            dialog.ValueString5 = FuncName;
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = IDValue.ValueString;
            var oldvaluestring2 = FuncName;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyIntKHValueRegex});
            bool check2 = true;
            if (check1)
            {
                IDValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                int id = int.Parse(IDValue.GetValue());
                if (id < 0 || id > 7)
                {
                    throw new ValueParseException("ID超出范围（0-7）");
                }
            }
            if (check1 && check2)
            {
                FuncName = valueStrings[2];
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, IDValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, FuncName);
                //ValueCommentManager.UpdateComment(InputValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(IDValue, valueStrings[1]);
            }
            else if (!check2)
            {
                throw new ValueParseException(String.Format("找不到函数{0:s}", valueStrings[2]));
            }
            else if (!check1)
            {
                throw new ValueParseException("EVENT格式非法！");
            }
        }

        private ATCHModel model;

        public string FuncName
        {
            get
            {
                if (model == null) return String.Empty;
                return model.FuncName;
            }
            set
            {
                if (model == null) return;
                model.FuncName = value;
                MiddleTextBlock1.Text = String.Format("INT:{0:s}", FuncName);
            }
        }

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
                MiddleTextBlock2.Text = String.Format("EVENT:{0:s}", model.IDValue.ValueShowString);
            }
        }

        public ATCHViewModel()
        {
            TopTextBlock.Text = InstructionName;
            model = new ATCHModel();
        }
    }
}
