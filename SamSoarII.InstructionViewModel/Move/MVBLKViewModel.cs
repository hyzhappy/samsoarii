using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.UserInterface;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using SamSoarII.UserInterface;
using System.Windows;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel
{
    public class MVBLKViewModel : OutputRectBaseViewModel
    {
        private MVBLKModel _model = new MVBLKModel();
        private WordValue SourceValue
        {
            get
            {
                return _model.SourceValue;
            }
            set
            {
                _model.SourceValue = value;
                MiddleTextBlock2.Text = string.Format("S : {0}", _model.SourceValue.ToShowString());
            }
        }
        private WordValue DestinationValue
        {
            get
            {
                return _model.DestinationValue;
            }
            set
            {
                _model.DestinationValue = value;
                MiddleTextBlock3.Text = string.Format("D : {0}", _model.DestinationValue.ToShowString());
            }
        }
        private WordValue Count
        {
            get
            {
                return _model.Count;
            }
            set
            {
                _model.Count = value;
                BottomTextBlock.Text = string.Format("N : {0}", _model.Count.ToShowString());
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
                _model = value as MVBLKModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
                Count = _model.Count;
            }
        }
        public override string InstructionName { get { return "MVBLK"; } }
        public MVBLKViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new MVBLKModel();
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = InstructionName;
            dialog.ShowLine2("S");
            dialog.ShowLine4("D");
            dialog.ShowLine6("N");
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

        public override BaseViewModel Clone()
        {
            return new MVBLKViewModel();
        }

        public static int CatalogID { get { return 603; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override bool CheckValueStrings(List<string> valueStrings)
        {
            Match match1 = Regex.Match(valueStrings[0], "^(D|CV|TV|AI|AO)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            Match match2 = Regex.Match(valueStrings[1], "^(D|CV|TV|AO)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            Match match3 = Regex.Match(valueStrings[2], "^D[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            if (!match1.Success)
            {
                match1 = Regex.Match(valueStrings[0], "^V[0-9]+$", RegexOptions.IgnoreCase);
            }
            if (!match2.Success)
            {
                match2 = Regex.Match(valueStrings[1], "^V[0-9]+$", RegexOptions.IgnoreCase);
            }
            if (!match3.Success)
            {
                if (!match3.Success)
                {
                    match3 = Regex.Match(valueStrings[2], "^K[-+]?[0-9]+$", RegexOptions.IgnoreCase);
                    if (!match3.Success)
                    {
                        match3 = Regex.Match(valueStrings[2], "^H[0-9A-F]+$", RegexOptions.IgnoreCase);
                    }
                }
            }
            return match1.Success && match2.Success && match3.Success;
        }
        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                SourceValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                SourceValue = WordValue.Null;
            }
            try
            {
                DestinationValue = ValueParser.ParseWordValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                DestinationValue = WordValue.Null;
            }
            try
            {
                Count = ValueParser.ParseWordValue(valueStrings[2]);
            }
            catch (ValueParseException exception)
            {
                Count = WordValue.Null;
            }
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ToString());
            result.Add(DestinationValue.ToString());
            result.Add(Count.ToString());
            return result;
        }
    }
}
