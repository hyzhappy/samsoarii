﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;

using System.Windows;

namespace SamSoarII.InstructionViewModel
{
    public class ADDFViewModel : OutputRectBaseViewModel
    {
        private ADDFModel _model;
        private FloatValue InputValue1
        {
            get
            {
                return _model.InputValue1;
            }
            set
            {
                _model.InputValue1 = value;
                MiddleTextBlock1.Text = _model.InputValue1.ToShowString();
            }
        }
        private FloatValue InputValue2
        {
            get
            {
                return _model.InputValue2;
            }
            set
            {
                _model.InputValue2 = value;
                MiddleTextBlock2.Text = _model.InputValue2.ToShowString();
            }
        }
        private FloatValue OutputValue
        {
            get
            {
                return _model.OutputValue;
            }
            set
            {
                _model.OutputValue = value;
                BottomTextBlock.Text = _model.OutputValue.ToShowString();
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
                _model = value as ADDFModel;
                InputValue1 = _model.InputValue1;
                InputValue2 = _model.InputValue2;
                OutputValue = _model.OutputValue;
            }
        }

        public ADDFViewModel()
        {
            TopTextBlock.Text = "ADDF";
            _model = new ADDFModel();
        }


        public override BaseViewModel Clone()
        {
            return new ADDFViewModel();
        }

        private static int CatalogID { get { return 700; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(InputValue1.ToString());
            result.Add(InputValue2.ToString());
            result.Add(OutputValue.ToString());
            return result;
        }

        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                InputValue1 = ValueParser.ParseFloatValue(valueStrings[0]);
            }
            catch(ValueParseException exception)
            {
                InputValue1 = FloatValue.Null;
            }
            try
            {
                InputValue2 = ValueParser.ParseFloatValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                InputValue2 = FloatValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseFloatValue(valueStrings[2]);
            }
            catch (ValueParseException exception)
            {
                OutputValue = FloatValue.Null;
            }
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = "ADDF";
            dialog.ShowLine2("In1");
            dialog.ShowLine4("In2");
            dialog.ShowLine6("Out");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> temp = new List<string>();
                    temp.Add(dialog.ValueString2);
                    temp.Add(dialog.ValueString4);
                    temp.Add(dialog.ValueString6);
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
