﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;

using System.Windows;

namespace SamSoarII.LadderInstViewModel
{
    public class MOVFViewModel : OutputRectBaseViewModel
    {
        private MOVFModel _model;
        private FloatValue SourceValue
        {
            get
            {
                return _model.SourceValue;
            }
            set
            {
                _model.SourceValue = value;
                MiddleTextBlock1.Text = _model.SourceValue.ValueShowString;
            }
        }
        private FloatValue DestinationValue
        {
            get
            {
                return _model.DestinationValue;
            }
            set
            {
                _model.DestinationValue = value;
                BottomTextBlock.Text = _model.DestinationValue.ValueShowString;
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
                _model = value as MOVFModel;
                SourceValue = _model.SourceValue;
                DestinationValue = _model.DestinationValue;
            }
        }
        public override string InstructionName { get { return "MOVF"; } }
        public MOVFViewModel()
        {
            TopTextBlock.Text = InstructionName;
            Model = new MOVFModel();
        }
        public override BaseViewModel Clone()
        {
            return new MOVFViewModel();
        }

        public static int CatalogID { get { return 602; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            var dialog = new ElementPropertyDialog(2);
            dialog.Title = InstructionName;
            dialog.ShowLine3("S");
            dialog.ShowLine5("D");
            return dialog;
        }

        public override IEnumerable<string> GetValueString()
        {
            List<string> result = new List<string>();
            result.Add(SourceValue.ValueString);
            result.Add(DestinationValue.ValueString);
            return result;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            try
            {
                SourceValue = ValueParser.ParseFloatValue(valueStrings[0]);
            }
            catch (ValueParseException exception)
            {
                SourceValue = FloatValue.Null;
            }

            try
            {
                DestinationValue = ValueParser.ParseFloatValue(valueStrings[1]);
            }
            catch (ValueParseException exception)
            {
                DestinationValue = FloatValue.Null;
            }
        }

    }
}
