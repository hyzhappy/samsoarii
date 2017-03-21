﻿using System;
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
    public class TWRViewModel : OutputRectBaseViewModel
    {
        private TWRModel _model;
        private WordValue StartValue
        {
            get
            {
                return _model.StartValue;
            }
            set
            {
                _model.StartValue = value;
                BottomTextBlock.Text = string.Format("T : {0}", _model.StartValue.ToShowString());
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
                _model = value as TWRModel;
                StartValue = _model.StartValue;
            }
        }
        public override string InstructionName { get { return "TWR"; } }
        public TWRViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new TWRModel();
        }

        public override BaseViewModel Clone()
        {
            return new TWRViewModel();
        }

        private static int CatalogID { get { return 1401; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(StartValue.ToString());
            return result;
        }

        public override bool CheckValueStrings(List<string> valueStrings)
        {
            Match match = Regex.Match(valueStrings[0], "^D[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            return match.Success;
        }
        public override void ParseValue(List<string> valueStrings)
        {
            try
            {
                StartValue = ValueParser.ParseWordValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                StartValue = WordValue.Null;
            }
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            dialog.Title = InstructionName;
            dialog.ShowLine4("T");
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
