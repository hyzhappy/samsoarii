using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;


namespace SamSoarII.LadderInstViewModel
{
    public class JMPViewModel : OutputRectBaseViewModel
    {
        private JMPModel _model;
        private WordValue LBLIndex
        {
            get
            {
                return _model.LBLIndex;
            }
            set
            {
                _model.LBLIndex = value;
                BottomTextBlock.Text = string.Format("LBL : {0}", _model.LBLIndex.ValueShowString);
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
                _model = value as JMPModel;
                LBLIndex = _model.LBLIndex;
            }
        }
        public override string InstructionName { get { return "JMP"; } }
        public JMPViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new JMPModel();
        }

        public override BaseViewModel Clone()
        {
            return new JMPViewModel();
        }

        private static int CatalogID { get { return 1102; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(LBLIndex.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
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

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(1);
            dialog.Title = InstructionName;
            dialog.ShowLine4("LBL");
            return dialog;
        }
    }
}
