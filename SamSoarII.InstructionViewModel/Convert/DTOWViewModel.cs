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
    public class DTOWViewModel : OutputRectBaseViewModel
    {
        private DTOWModel _model;
        private DoubleWordValue InputValue
        {
            get
            {
                return _model.InputValue;
            }
            set
            {
                _model.InputValue = value;
                MiddleTextBlock1.Text = _model.InputValue.ToShowString();
            }
        }
        private WordValue OutputValue
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
                _model = value as DTOWModel;
                InputValue = _model.InputValue;
                OutputValue = _model.OutputValue;
            }
        }
        public DTOWViewModel()
        {
            TopTextBlock.Text = "DTOW";
            Model = new DTOWModel();
        }

        public override BaseViewModel Clone()
        {
            return new DTOWViewModel();
        }

        private static int CatalogID { get { return 401; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(InputValue.ToString());
            result.Add(OutputValue.ToString());
            return result;
        }

        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                InputValue = ValueParser.ParseDoubleWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                InputValue = DoubleWordValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                OutputValue = WordValue.Null;
            }
        }


        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = "DTOW";
            dialog.ShowLine3("In");
            dialog.ShowLine5("Out");
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
