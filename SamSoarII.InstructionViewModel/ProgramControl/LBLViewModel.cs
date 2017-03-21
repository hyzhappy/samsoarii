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
    public class LBLViewModel : OutputRectBaseViewModel
    {
        private LBLModel _model;
        private WordValue LBLIndex
        {
            get
            {
                return _model.LBLIndex;
            }
            set
            {
                _model.LBLIndex = value;
                BottomTextBlock.Text = string.Format("L : {0}", _model.LBLIndex.ToShowString());
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
                _model = value as LBLModel;
                LBLIndex = _model.LBLIndex;
            }
        }
        public override string InstructionName { get { return "LBL"; } }
        public LBLViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new LBLModel();
        }

        public override BaseViewModel Clone()
        {
            return new LBLViewModel();
        }

        private static int CatalogID { get { return 1103; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(LBLIndex.ToString());
            return result;
        }

        public override bool CheckValueStrings(List<string> valueStrings)
        {
            Match match = Regex.Match(valueStrings[0], "^K[-+]?[0-9]+$", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                match = Regex.Match(valueStrings[0], "^H[0-9A-F]+$", RegexOptions.IgnoreCase);
            }
            return match.Success;
        }
        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                LBLIndex = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                LBLIndex = WordValue.Null;
            }
        }
        public override bool Assert()
        {
            return NextElemnets.Count == 1 && NextElemnets.All(x => { return x.Type == ElementType.Null; });
        }
        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = InstructionName;
            dialog.ShowLine4("LBL");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> valuelist = new List<string>();
                    valuelist.Add(dialog.ValueString4);
                    if (!CheckValueStrings(valuelist))
                    {
                        MessageBox.Show(dialog, "参数输入错误,请重新输入!");
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
