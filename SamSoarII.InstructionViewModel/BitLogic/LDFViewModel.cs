﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using SamSoarII.UserInterface;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using SamSoarII.PLCDevice;

namespace SamSoarII.LadderInstViewModel
{
    public class LDFViewModel : InputBaseViewModel
    {
        private LDFModel _model;
        public BitValue Value
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
                _model = value as LDFModel;
                Value = _model.Value;
            }
        }
        private TextBlock _commentTextBlock = new TextBlock();
        public override string InstructionName { get { return "LDF"; } }
        public LDFViewModel()
        {
            Model = new LDFModel();
            // Draw shapes
            Line line1 = new Line();
            line1.X1 = 50;
            line1.X2 = 50;
            line1.Y1 = 0;
            line1.Y2 = 100;
            line1.StrokeThickness = 4;
            line1.Stroke = Brushes.Black;
            CenterCanvas.Children.Add(line1);

            Line line2 = new Line();
            line2.X1 = 50;
            line2.X2 = 70;
            line2.Y1 = 100;
            line2.Y2 = 80;
            line2.StrokeThickness = 4;
            line2.Stroke = Brushes.Black;
            CenterCanvas.Children.Add(line2);

            Line line3 = new Line();
            line3.X1 = 50;
            line3.X2 = 30;
            line3.Y1 = 100;
            line3.Y2 = 80;
            line3.StrokeThickness = 4;
            line3.Stroke = Brushes.Black;
            CenterCanvas.Children.Add(line3);

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
            return new LDFViewModel();
        }

        public static int CatalogID { get { return 205; } }

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

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value.ValueString);
            return result;
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
        public override void UpdateCommentContent()
        {
            if (Value != BitValue.Null)
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
