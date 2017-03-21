using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using SamSoarII.UserInterface;
using System.Text.RegularExpressions;

namespace SamSoarII.LadderInstViewModel
{
    public class ALTPViewModel : OutputRectBaseViewModel
    {
        private ALTPModel _model;
        private BitValue Value
        {
            get
            {
                return _model.Value;
            }
            set
            {
                _model.Value = value;
                BottomTextBlock.Text = _model.Value.ToShowString();
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
                _model = value as ALTPModel;
                Value = _model.Value;
            }
        }
        public override string InstructionName { get { return "ALTP"; } }
        public ALTPViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new ALTPModel();
        }

        public override BaseViewModel Clone()
        {
            return new ALTPViewModel();
        }

        private int CatalogID { get { return 216; } }
        public override int GetCatalogID()
        {
            return CatalogID;
        }
        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(Value.ToString());
            return result;
        }
        public override bool CheckValueStrings(List<string> valueStrings)
        {
            Match match = Regex.Match(valueStrings[0], "^(Y|M|S)[0-9]+(V[0-9]+)?$",RegexOptions.IgnoreCase);
            return match.Success;
        }
        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                Value = ValueParser.ParseBitValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                Value = BitValue.Null;
            }
        }
        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = InstructionName;
            dialog.ShowLine4("Bit");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> valuelist = new List<string>();
                    valuelist.Add(dialog.ValueString4);
                    if(!CheckValueStrings(valuelist))
                    {
                        MessageBox.Show(dialog,"参数输入错误,请重新输入!");
                    }
                    else
                    {
                        ParseValue(valuelist);
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
