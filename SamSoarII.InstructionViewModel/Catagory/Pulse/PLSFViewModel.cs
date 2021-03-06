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

namespace SamSoarII.LadderInstViewModel.Pulse
{
    public class PLSFViewModel : OutputRectBaseViewModel
    {
        public override string InstructionName
        {
            get
            {
                return "PLSF";
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
                if (value is PLSFModel)
                {
                    this.model = (PLSFModel)(value);
                }
                else
                {
                    throw new ArgumentException("Cannot assign value except PLSFModel");
                }
            }
        }

        public override BaseViewModel Clone()
        {
            return new PLSFViewModel();
        }

        public override int GetCatalogID()
        {
            return 1600;
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
                OutputValue = ValueParser.ParseBitValue(valueStrings[1]);
            }
            catch (ValueParseException)
            {
                OutputValue = BitValue.Null;
            }
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.ShowLine3("F:", FreqValue);
            dialog.ShowLine5("OUT:", OutputValue);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = FreqValue.ValueString;
            var oldvaluestring2 = OutputValue.ValueString;
            bool check1 = ValueParser.CheckValueString(valueStrings[0], new Regex[] {
                ValueParser.VerifyWordRegex3, ValueParser.VerifyIntKHValueRegex});
            bool check2 = ValueParser.CheckValueString(valueStrings[2], new Regex[] {
                ValueParser.VerifyBitRegex4});
            if (check1 && check2)
            {
                FreqValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                OutputValue = ValueParser.ParseBitValue(valueStrings[2], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, FreqValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, OutputValue.ValueString);
                ValueCommentManager.UpdateComment(FreqValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(OutputValue, valueStrings[3]);
            }
            else if (!check1)
            {
                throw new ValueParseException("F格式非法！");
            }
            else if (!check2)
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

            if (OutputValue != BitValue.Null)
            {
                _commentTextBlocks[1].Text = String.Format("{0}:{1}", OutputValue.ValueString, OutputValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = String.Empty;
            }
        }

        private PLSFModel model;

        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock() };

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

        public PLSFViewModel()
        {
            TopTextBlock.Text = InstructionName;
            model = new PLSFModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
        }

    }
}
