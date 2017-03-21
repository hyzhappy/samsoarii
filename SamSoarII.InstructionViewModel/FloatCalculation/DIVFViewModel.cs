﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;

using System.Windows;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel
{
    public class DIVFViewModel : OutputRectBaseViewModel
    {
        private DIVFModel _model;
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
                _model = value as DIVFModel;
                InputValue1 = _model.InputValue1;
                InputValue2 = _model.InputValue2;
                OutputValue = _model.OutputValue;
            }
        }
        public override string InstructionName { get { return "DIVF"; } }
        public DIVFViewModel()
        {
            TopTextBlock.Text = InstructionName;
            _model = new DIVFModel();
        }


        public override BaseViewModel Clone()
        {
            return new DIVFViewModel();
        }

        private static int CatalogID { get { return 703; } }

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

        public override bool CheckValueStrings(List<string> valueStrings)
        {
            Match match1 = Regex.Match(valueStrings[0], "^(D|CV)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            Match match2 = Regex.Match(valueStrings[1], "^(D|CV)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            Match match3 = Regex.Match(valueStrings[2], "^(D|CV)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            if (!match1.Success)
            {
                match1 = Regex.Match(valueStrings[0], "^K[+-]?([0-9]*[.])?[0-9]+$", RegexOptions.IgnoreCase);
            }
            if (!match2.Success)
            {
                match2 = Regex.Match(valueStrings[1], "^K[+-]?([0-9]*[.])?[0-9]+$", RegexOptions.IgnoreCase);
            }
            return match1.Success && match2.Success && match3.Success;
        }
        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                InputValue1 = ValueParser.ParseFloatValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
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
            dialog.Title = InstructionName;
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
                    if (!CheckValueStrings(temp))
                    {
                        MessageBox.Show(dialog, "参数输入错误,请重新输入!");
                    }
                    else
                    {
                        ParseValue(temp);
                        dialog.Close();
                    }
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
