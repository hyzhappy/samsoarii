using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
using SamSoarII.ValueModel;
using SamSoarII.UserInterface;
namespace SamSoarII.InstructionViewModel
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

        public ALTPViewModel()
        {
            TopTextBlock.Text = "ALTP";
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
            dialog.Title = "ALTP";
            dialog.ShowLine4("Bit");
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
            //dialog.Title = "ALTP";
            //dialog.label4.Visibility = Visibility.Visible;
            //dialog.label4.Content = "Bit";
            //dialog.textBox4.Visibility = Visibility.Visible;
            //dialog.EnsureButton.Click += (sender, e) =>
            //{
            //    try
            //    {
            //        List<string> valuelist = new List<string>();
            //        valuelist.Add(dialog.textBox4.Text);
            //        ParseValue(valuelist);
            //        dialog.Close();
            //    }
            //    catch (Exception exception)
            //    {
            //        MessageBox.Show(exception.Message);
            //    }
            //};
            //dialog.ShowDialog();
        }

    }
}
