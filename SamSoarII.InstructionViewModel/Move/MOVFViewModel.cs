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
    public class MOVFViewModel : OutputRectBaseViewModel
    {
        private MOVFModel _model;
        private FloatValue SourceValue
        {
            get
            {
                return _model.SourceValue;
            }
            set
            {
                _model.SourceValue = value;
                MiddleTextBlock1.Text = _model.SourceValue.ToShowString();
            }
        }
        private FloatValue DestinationValue
        {
            get
            {
                return _model.DestinationValue;
            }
            set
            {
                _model.DestinationValue = value;
                BottomTextBlock.Text = _model.DestinationValue.ToShowString();
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
                _model = value as MOVFModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
            }
        }
        public MOVFViewModel()
        {
            TopTextBlock.Text = "MOVF";
            Model = new MOVFModel();
        }
        public override BaseViewModel Clone()
        {
            return new MOVFViewModel();
        }

        public static int CatalogID { get { return 602; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = "MOVF";
            dialog.ShowLine3("S");
            dialog.ShowLine5("D");
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

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ToString());
            result.Add(DestinationValue.ToString());
            return result;
        }

        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                SourceValue = ValueParser.ParseFloatValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                SourceValue = FloatValue.Null;
            }

            try
            {
                DestinationValue = ValueParser.ParseFloatValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                DestinationValue = FloatValue.Null;
            }
        }

    }
}
