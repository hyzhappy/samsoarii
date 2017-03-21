using System;
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
    public class XORWViewModel : OutputRectBaseViewModel
    {
        private XORWModel _model;
        private WordValue InputValue1
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
        private WordValue InputValue2
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
                _model = value as XORWModel;
                InputValue1 = _model.InputValue1;
                InputValue2 = _model.InputValue2;
                OutputValue = _model.OutputValue;
            }
        }
        public override string InstructionName { get { return "XORW"; } }
        public XORWViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new XORWModel();
        }

        public override BaseViewModel Clone()
        {
            return new XORWViewModel();
        }

        private static int CatalogID { get { return 506; } }

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
            Match match1 = Regex.Match(valueStrings[0], "^(D|CV|TV|AI|AO)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            Match match2 = Regex.Match(valueStrings[1], "^(D|CV|TV|AI|AO)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            Match match3 = Regex.Match(valueStrings[2], "^(D|CV|TV|AO)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            if (!match1.Success)
            {
                match1 = Regex.Match(valueStrings[0], "^V[0-9]+$", RegexOptions.IgnoreCase);
                if (!match1.Success)
                {
                    match1 = Regex.Match(valueStrings[0], "^K[-+]?[0-9]+$", RegexOptions.IgnoreCase);
                    if (!match1.Success)
                    {
                        match1 = Regex.Match(valueStrings[0], "^H[0-9A-F]+$", RegexOptions.IgnoreCase);
                    }
                }
            }
            if (!match2.Success)
            {
                match2 = Regex.Match(valueStrings[1], "^V[0-9]+$", RegexOptions.IgnoreCase);
                if (!match2.Success)
                {
                    match2 = Regex.Match(valueStrings[1], "^K[-+]?[0-9]+$", RegexOptions.IgnoreCase);
                    if (!match2.Success)
                    {
                        match2 = Regex.Match(valueStrings[1], "^H[0-9A-F]+$", RegexOptions.IgnoreCase);
                    }
                }
            }
            if (!match3.Success)
            {
                match3 = Regex.Match(valueStrings[2], "^V[0-9]+$", RegexOptions.IgnoreCase);
            }
            return match1.Success && match2.Success && match3.Success;
        }
        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                InputValue1 = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                InputValue1 = WordValue.Null;
            }
            try
            {
                InputValue2 = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                InputValue2 = WordValue.Null;
            }
            try
            {
                OutputValue = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException exception)
            {
                OutputValue = WordValue.Null;
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
