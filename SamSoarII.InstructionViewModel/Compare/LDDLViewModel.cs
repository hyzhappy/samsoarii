using System;
using System.Windows;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel
{
    public class LDDLViewModel : InputBaseViewModel
    {
        private LDDLModel _model;
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
                _model = value as LDDLModel;
                Value1 = _model.Value1;
                Value2 = _model.Value2;
            }
        }
        public override string InstructionName { get { return "LDDL"; } }
        public LDDLViewModel()
        {
            CenterTextBlock.Text = "D<";
            Model = new LDDLModel();
        }

        public override BaseViewModel Clone()
        {
            return new LDDLViewModel();
        }

        private static int CatalogID { get { return 311; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override bool CheckValueStrings(List<string> valueStrings)
        {
            Match match1 = Regex.Match(valueStrings[0], "^(D|CV)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            Match match2 = Regex.Match(valueStrings[1], "^(D|CV)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            if (!match1.Success)
            {
                match1 = Regex.Match(valueStrings[0], "^K[-+][0-9]+$", RegexOptions.IgnoreCase);
                if (!match1.Success)
                {
                    match1 = Regex.Match(valueStrings[0], "^H[0-9A-F]+$", RegexOptions.IgnoreCase);
                }
            }
            if (!match2.Success)
            {
                match2 = Regex.Match(valueStrings[1], "^K[-+][0-9]+$", RegexOptions.IgnoreCase);
                if (!match2.Success)
                {
                    match2 = Regex.Match(valueStrings[1], "^H[0-9A-F]+$", RegexOptions.IgnoreCase);
                }
            }
            return match1.Success && match2.Success;
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
            dialog.Title = InstructionName;
            dialog.ShowLine3("DW1");
            dialog.ShowLine5("DW2");
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
