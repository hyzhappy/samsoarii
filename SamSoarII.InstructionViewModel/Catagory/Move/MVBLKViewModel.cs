﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.UserInterface;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System.Windows;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SamSoarII.LadderInstViewModel
{
    public class MVBLKViewModel : OutputRectBaseViewModel
    {
        private MVBLKModel _model = new MVBLKModel();
        public WordValue SourceValue
        {
            get
            {
                return _model.SourceValue;
            }
            set
            {
                _model.SourceValue = value;
                MiddleTextBlock2.Text = string.Format("S:{0}", _model.SourceValue.ValueShowString);
            }
        }
        public WordValue DestinationValue
        {
            get
            {
                return _model.DestinationValue;
            }
            set
            {
                _model.DestinationValue = value;
                MiddleTextBlock3.Text = string.Format("D:{0}", _model.DestinationValue.ValueShowString);
            }
        }
        public WordValue Count
        {
            get
            {
                return _model.Count;
            }
            set
            {
                _model.Count = value;
                BottomTextBlock.Text = string.Format("N:{0}", _model.Count.ValueShowString);
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
                _model = value as MVBLKModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
                Count = _model.Count;
            }
        }
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock(), new TextBlock(), new TextBlock() };
        public override string InstructionName { get { return "MVBLK"; } }
        public MVBLKViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new MVBLKModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
            CommentArea.Children.Add(_commentTextBlocks[1]);
            CommentArea.Children.Add(_commentTextBlocks[2]);
        }
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(3);
            dialog.Title = InstructionName;
            dialog.ShowLine2("S",SourceValue);
            dialog.ShowLine4("D",DestinationValue);
            dialog.ShowLine6("N",Count);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring1 = SourceValue.ValueString;
            var oldvaluestring2 = DestinationValue.ValueString;
            var oldvaluestring3 = Count.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyWordRegex1}) && ValueParser.CheckValueString(valueStrings[2], new Regex[] { ValueParser.VerifyWordRegex2 }) && ValueParser.CheckValueString(valueStrings[4], new Regex[] { ValueParser.VerifyWordRegex3,ValueParser.VerifyIntKHValueRegex }))
            {
                SourceValue = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                DestinationValue = ValueParser.ParseWordValue(valueStrings[2], contextDevice);
                Count = ValueParser.ParseWordValue(valueStrings[4], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring1, SourceValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring2, DestinationValue.ValueString);
                InstructionCommentManager.ModifyValue(this, oldvaluestring3, Count.ValueString);
                ValueCommentManager.UpdateComment(SourceValue, valueStrings[1]);
                ValueCommentManager.UpdateComment(DestinationValue, valueStrings[3]);
                ValueCommentManager.UpdateComment(Count, valueStrings[5]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }
        public override BaseViewModel Clone()
        {
            return new MVBLKViewModel();
        }

        public static int CatalogID { get { return 603; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                SourceValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                SourceValue = WordValue.Null;
            }
            try
            {
                DestinationValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                DestinationValue = WordValue.Null;
            }
            try
            {
                Count = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException exception)
            {
                Count = WordValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(DestinationValue.ValueString);
            result.Add(Count.ValueString);
            return result;
        }
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(SourceValue);
            result.Add(DestinationValue);
            result.Add(Count);
            return result;
        }
        public override void UpdateCommentContent()
        {
            if (SourceValue != WordValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", SourceValue.ValueString, SourceValue.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
            if (DestinationValue != WordValue.Null)
            {
                _commentTextBlocks[1].Text = string.Format("{0}:{1}", DestinationValue.ValueString, DestinationValue.Comment);
            }
            else
            {
                _commentTextBlocks[1].Text = string.Empty;
            }
            if (Count != WordValue.Null)
            {
                _commentTextBlocks[2].Text = string.Format("{0}:{1}", Count.ValueString, Count.Comment);
            }
            else
            {
                _commentTextBlocks[2].Text = string.Empty;
            }
        }
    }
}
