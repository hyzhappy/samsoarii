﻿using System;
using System.Windows;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;

namespace SamSoarII.InstructionViewModel
{
    public class LDDLEViewModel : InputBaseViewModel
    {
        private LDDLEModel _model;
        private DoubleWordValue Value1
        {
            get
            {
                return _model.Value1;
            }
            set
            {
                _model.Value1 = value;
                ValueTextBlock.Text = _model.Value1.ToShowString();
            }
        }
        private DoubleWordValue Value2
        {
            get
            {
                return _model.Value2;
            }
            set
            {
                _model.Value2 = value;
                Value2TextBlock.Text = _model.Value2.ToShowString();
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
                _model = value as LDDLEModel;
                Value1 = _model.Value1;
                Value2 = _model.Value2;
            }
        }

        public LDDLEViewModel()
        {
            CenterTextBlock.Text = "D<=";
            Model = new LDDLEModel();
        }

        public override BaseViewModel Clone()
        {
            return new LDDLEViewModel();
        }

        private static int CatalogID { get { return 309; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                Value1 = ValueParser.ParseDoubleWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                Value1 = DoubleWordValue.Null;
            }
            try
            {
                Value2 = ValueParser.ParseDoubleWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                Value2 = DoubleWordValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value1.ToString());
            result.Add(Value2.ToString());
            return result;
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = "LDDLE";
            dialog.ShowLine3("DW1");
            dialog.ShowLine5("DW2");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> temp = new List<string>();
                    temp.Add(dialog.ValueString3);
                    temp.Add(dialog.ValueString5);
                    ParseValue(temp);
                    dialog.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            };
            dialog.ShowDialog();
        }
    }
}
