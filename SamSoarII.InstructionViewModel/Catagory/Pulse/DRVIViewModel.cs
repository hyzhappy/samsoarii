﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using System.Windows.Controls;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;
using SamSoarII.LadderInstModel.Pulse;

namespace SamSoarII.LadderInstViewModel.Pulse
{
    public class DRVIViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "DRVI";
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
                if (value is DRVIModel)
                {
                    this.model = (DRVIModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except DRVIModel");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new DRVIViewModel();
        }

        public override int GetCatalogID()
        {
            return 1615;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(FreqValue.ValueString);
            result.Add(OutputValue.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(FreqValue);
            result.Add(OutputValue);
            return result;
        }
        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                FreqValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException)
            {
                FreqValue = WordValue.Null;
            }
            try
            {
                PulseValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                PulseValue = WordValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseBitValue(valueStrings[2]);
            }
            catch (ValueParseException)
            {
                OutputValue = BitValue.Null;
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(3);
            dialog.ShowLine2("F:", FreqValue);
            dialog.ShowLine4("P:", PulseValue);
            dialog.ShowLine6("OUT:", OutputValue);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = FreqValue.ValueString;
            var oldvaluestring2 = PulseValue.ValueString;
            var oldvaluestring3 = OutputValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex});
            bool check3 = ValueParser.CheckValueString(valueStrings[4], new Regex[] {
                ValueParser.VerifyBitRegex4});
            if (check1 && check2 && check3) 
            {
                FreqValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                PulseValue = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                OutputValue = ValueParser.ParseBitValue(valueStrings[4], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, FreqValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, PulseValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, OutputValue.ValueString);
                ValueCommentManager.UpdateComment(FreqValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(PulseValue, valueStrings[3]);
                ValueCommentManager.UpdateComment(OutputValue, valueStrings[5]);
            }
            else if (!check1)
            {
                throw new ValueParseException("F格式非法！");
            }
            else if (!check2)
            {
                throw new ValueParseException("P格式非法");
            }
            else if (!check3)
            {
                throw new ValueParseException("OUT格式非法");
            }
        }

        public override void UpdateCommentContent()
        {
            if (FreqValue != WordValue.Null)
            {
                _commentTextBlocks[0].Text = String.Format("{0}:{1}", FreqValue.ValueString, FreqValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = String.Empty;
            }
            if (PulseValue != WordValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0}:{1}", PulseValue.ValueString, PulseValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
            if (OutputValue != BitValue.Null)
            {
                _commentTextBlocks[2].Text = String.Format("{0}:{1}", OutputValue.ValueString, OutputValue.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = String.Empty;
            }
        }

        private DRVIModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock(), new TextBlock() };

        public WordValue FreqValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.FreqValue;
            }
            set
            {
                if (model == null) return;
                model.FreqValue = value;
                MiddleTextBlock1.Text = String.Format("F:{0:s}", value.ValueShowString);
            }
        }

        public WordValue PulseValue
        {
            get
            {
                if (model == null) return WordValue.Null;
                return model.PulseValue;
            }
            set
            {
                if (model == null) return;
                model.PulseValue = value;
                MiddleTextBlock2.Text = String.Format("P:{0:s}", value.ValueShowString);
            }
        }
         
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

        public DRVIViewModel()
        {
            TopTextBlock.Text = InstructionName;
            model = new DRVIModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
        }

    }
}
