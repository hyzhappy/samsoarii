using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;

namespace SamSoarII.LadderInstViewModel
{
    public class CALLViewModel : OutputRectBaseViewModel
    {
        private CALLModel _model;
        private string FunctionName
        {
            get
            {
                return _model.FunctionName;
            }
            set
            {
                _model.FunctionName = value;
                BottomTextBlock.Text = _model.FunctionName;
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
                _model = value as CALLModel;
                FunctionName = _model.FunctionName;
            }
        }
        public override string InstructionName { get { return "CALL"; } }
        public CALLViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new CALLModel();
        }

        public override BaseViewModel Clone()
        {
            return new CALLViewModel();
        }

        private static int CatalogID { get { return 1104; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }
        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(FunctionName);
            return result;
        }
        public override bool CheckValueStrings(List<string> valueStrings)
        {
            throw new NotImplementedException();
        }
        public override void ParseValue(List<string> valueStrings)
        {
            FunctionName = valueStrings[0];
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = InstructionName;
            dialog.ShowLine4("Func");
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    List<string> valuelist = new List<string>();
                    valuelist.Add(dialog.ValueString4);
                    ParseValue(valuelist);
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
