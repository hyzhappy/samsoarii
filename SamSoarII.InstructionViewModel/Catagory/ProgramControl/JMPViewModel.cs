﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SamSoarII.LadderInstViewModel
{
    public class JMPViewModel : OutputRectBaseViewModel
    {
        private JMPModel _model;
        public WordValue LBLIndex
        {
            get
            {
                return _model.LBLIndex;
            }
            set
            {
                _model.LBLIndex = value;
                BottomTextBlock.Text = string.Format("LBL:{0}", _model.LBLIndex.ValueShowString);
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
        private TextBlock[] _commentTextBlocks = new TextBlock[] { new TextBlock()};
        public override string InstructionName { get { return "JMP"; } }
        public JMPViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new JMPModel();
            CommentArea.Children.Add(_commentTextBlocks[0]);
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
        public override IEnumerable<IValueModel> GetValueModels()
        {
            List<IValueModel> result = new List<IValueModel>();
            result.Add(LBLIndex);
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
        /*
        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(1);
            dialog.Title = InstructionName;
            dialog.ShowLine4("LBL",LBLIndex);
            return dialog;
        }
        */
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {
            var oldvaluestring = LBLIndex.ValueString;
            if (ValueParser.CheckValueString(valueStrings[0], new Regex[] {ValueParser.VerifyIntKHValueRegex }))
            {
                LBLIndex = ValueParser.ParseWordValue(valueStrings[0], contextDevice);
                InstructionCommentManager.ModifyValue(this, oldvaluestring, LBLIndex.ValueString);
                ValueCommentManager.UpdateComment(LBLIndex, valueStrings[1]);
            }
            else
            {
                throw new ValueParseException("Unexpected input");
            }
        }
        public override void UpdateCommentContent()
        {
            if (LBLIndex != WordValue.Null)
            {
                _commentTextBlocks[0].Text = string.Format("{0}:{1}", LBLIndex.ValueString, LBLIndex.Comment);
            }
            else
            {
                _commentTextBlocks[0].Text = string.Empty;
            }
        }
    }
}
