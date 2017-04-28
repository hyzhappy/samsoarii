using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.LadderInstModel.Pulse;
using SamSoarII.ValueModel;
using System.Windows.Controls;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel.Pulse
{
    public class PLSSTOPViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "PLSSTOP";
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
                if (value is PLSSTOPModel)
                {
                    this.model = (PLSSTOPModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except PLSSTOPModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new PLSSTOPViewModel();
        }

        public override int GetCatalogID()
        {
            return 1611;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(OutputValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                OutputValue = ValueParser.ParseBitValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                OutputValue = BitValue.Null;
            }
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(1);
            dialog.ShowLine4("OUT:", OutputValue);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = OutputValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyBitRegex4});
            if (check1)
            {
                OutputValue = ValueParser.ParseBitValue(valueStrings[6], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, OutputValue.ValueString);
                ValueCommentManager.UpdateComment(OutputValue, valueStrings[7]);
            }
            else
            {
                throw new ValueParseException("OUT格式非法！");
            }
        }

        public override void UpdateCommentContent()
        {
            if (OutputValue != BitValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0}:{1}", OutputValue.ValueString, OutputValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
        }

        private PLSSTOPModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock() };

        public BitValue OutputValue
        {
            get
            {
                if (model == null) return BitValue.Null;
                return model.OutputValue;
            }
            set
            {
                if (model == null) return;
                model.OutputValue = value;
                BottomTextBlock.Text = String.Format("OUT:{0:s}", value.ValueShowString);
            }
        }

        public PLSSTOPViewModel()
        {
            TopTextBlock.Text = InstructionName;
            model = new PLSSTOPModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
        }
    }
}
