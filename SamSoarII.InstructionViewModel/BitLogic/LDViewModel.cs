﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System.Windows;
using SamSoarII.UserInterface;
using System.Windows.Controls;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel
{
    public class LDViewModel : InputBaseViewModel
    {
        private LDModel _model = new LDModel();
        private BitValue Value
        {
            get
            {
                return _model.Value;
            }
            set
            {
                _model.Value = value;
                ValueTextBlock.Text = _model.Value.ValueShowString;
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
                _model = value as LDModel;
                Value = _model.Value;
            }
        }

        private TextBlock _commentTextBlock = new TextBlock();

        public override string InstructionName { get { return "LD"; } }
        public LDViewModel()
        {
            Model = new LDModel();
            CommentArea.Children.Add(_commentTextBlock);
        }


        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(1);
            dialog.Title = InstructionName;
            dialog.ShowLine4("Bit", Value);
            return dialog;
        }

        public override BaseViewModel Clone()
        {
            return new LDViewModel();
        }

        public static int CatalogID { get { return 200; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                Value = ValueParser.ParseBitValue(valueStrings[0]);
            }
            catch
            {
                Value = BitValue.Null;
            }
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring = Value.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] { ValueParser.VerifyBitRegex1 }))
            {
                Value = ValueParser.ParseBitValue(valueStrings[0], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring, Value.ValueString);
                ValueCommentManager.UpdateComment(Value, valueStrings[1]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value.ValueString);
            return result;
        }

        public override void UpdateCommentContent()
        {
            if(Value != BitValue.Null)
            {
                _commentTextBlock.Text = string.Format("{0}:{1}", Value.ValueString, Value.Comment);
            }
            else
            {
                _commentTextBlock.Text = string.Empty;
            }
        }
    }
}
