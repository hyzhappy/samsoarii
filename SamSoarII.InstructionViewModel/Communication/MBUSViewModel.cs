using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.LadderInstModel.Auxiliar;
using System.Windows.Controls;
using SamSoarII.ValueModel;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;
using SamSoarII.LadderInstModel.Communication;

namespace SamSoarII.LadderInstViewModel
{
    public class MBUSViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "MBUS";
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
                if (value is MBUSModel)
                {
                    this.model = (MBUSModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except MBUSModel.");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new MBUSViewModel();
        }

        public override int GetCatalogID()
        {
            return 1500;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(COMPort.ValueString);
            result.Add(Table);
            result.Add(Message.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(COMPort);
            result.Add(Message);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                COMPort = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                COMPort = WordValue.Null;
            }
            Table = valueStrings[1];
            try
            {
                Message = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException)
            {
                Message = WordValue.Null;
            }

        }

        public override IPropertyDialog PreparePropertyDialog()
        {

            var dialog = new ElementPropertyDialog(3, ElementPropertyDialog.INST_MBUS);
            dialog.Title = InstructionName;
            dialog.ShowLine2("COM:", COMPort);
            dialog.ShowLine4("TABLE:");
            dialog.ValueString4 = Table;
            dialog.ShowLine6("WR:", Message);
            return dialog;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = COMPort.ValueString;
            var oldvaluestring2 = Table;
            var oldvaluestring3 = Message.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyWordRegex1, ValueParser.VerifyIntKHValueRegex });
            bool check3 = ValueParser.CheckValueString(valueStrings[4], new Regex[] {
                ValueParser.VerifyWordRegex3 });
            if (check1 && check3)
            {
                COMPort = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                Table = valueStrings[2];
                Message = ValueParser.ParseWordValue(valueStrings[4], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, COMPort.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, Table);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, Message.ValueString);
                ValueCommentManager.UpdateComment(COMPort, valueStrings[1]);
                ValueCommentManager.UpdateComment(Message, valueStrings[5]);
            }
            else if (!check1)
            {
                throw new ValueParseException("COM格式错误！");
            }
            else if (!check3)
            {
                throw new ValueParseException("WR格式错误！");
            }
        }
        
        public override void UpdateCommentContent()
        {
            if (COMPort != WordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0:s}:{1:s}", COMPort.ValueString, COMPort.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (Message != WordValue.Null)
            {
                _commentTextBlocks[2].Text = String.Format("{0:s}:{1:s}", Message.ValueString, Message.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = String.Empty;
            }
        }

        private MBUSModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] {new TextBlock(), new TextBlock(), new TextBlock()};

        public WordValue COMPort
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.COMPort;
            }
            set
            {
                if (model == null) return;
                model.COMPort = value;
                MiddleTextBlock1.Text = String.Format("PORT:{0:s}", value.ValueShowString);
            }
        }

        public string Table
        {
            get
            {
                if (model == null) return String.Empty;
                return model.Table;
            }
            set
            {
                if (model == null) return;
                model.Table = value;
                MiddleTextBlock2.Text = String.Format("TABLE:{0:s}", value);
            }
        }

        public WordValue Message
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.Message;
            }
            set
            {
                if (model == null) return;
                model.Message = value;
                BottomTextBlock.Text = String.Format("WR:{0:s}", value.ValueShowString);
            }
        }

        public MBUSViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new MBUSModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
        }
        
    }
}
