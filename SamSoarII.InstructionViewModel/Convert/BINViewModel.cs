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
    public class BINViewModel : OutputRectBaseViewModel
    {
        private BINModel _model;
        private WordValue InputValue
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
                _model = value as BINModel;
                InputValue = _model.InputValue;
                OutputValue = _model.OutputValue;
            }
        }
        public override string InstructionName { get { return "BIN"; } }
        public BINViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new BINModel();
        }

        public override BaseViewModel Clone()
        {
            return new BINViewModel();
        }

        private static int CatalogID { get { return 403; } }

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

        public override bool CheckValueStrings(List<string> valueStrings)
        {
            Match match1 = Regex.Match(valueStrings[0], "^(D|CV|TV|AI|AO)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            Match match2 = Regex.Match(valueStrings[1], "^(D|CV|TV|AO)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
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
            }
            return match1.Success && match2.Success;
        }
        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                InputValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                InputValue = WordValue.Null;
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
            dialog.Title = InstructionName;
            dialog.ShowLine3("In");
            dialog.ShowLine5("Out");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> temp = new List<string>();
                    temp.Add(dialog.ValueString3);
                    temp.Add(dialog.ValueString5);
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
