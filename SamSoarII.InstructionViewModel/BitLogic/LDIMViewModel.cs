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

namespace SamSoarII.LadderInstViewModel
{
    public class LDIMViewModel : InputBaseViewModel
    {
        private LDIMModel _model;
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
                _model = value as LDIMModel;
                Value = _model.Value;
            }
        }
        private TextBlock _commentTextBlock = new TextBlock();
        public override string InstructionName { get { return "LDIM"; } }
        public LDIMViewModel()
        {
            Model = new LDIMModel();
            // Draw shapes
            Line line = new Line();
            line.X1 = 50;
            line.Y1 = 20;
            line.X2 = 50;
            line.Y2 = 80;
            line.StrokeThickness = 4;
            line.Stroke = Brushes.Black;
            CenterCanvas.Children.Add(line);
            // 
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
            return new LDIMViewModel();
        }

        public static int CatalogID { get { return 202; } }

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
    }
}
