﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;


namespace SamSoarII.InstructionViewModel
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

        public LBLViewModel()
        {
            TopTextBlock.Text = "LBL";
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

        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                LBLIndex = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch(ValueParseException exception)
            {
                LBLIndex = WordValue.Null;
            }
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = "LBL";
            dialog.ShowLine4("LBL");
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
