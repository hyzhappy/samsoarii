using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSoarII.Core.Models;
using System.ComponentModel;

namespace SamSoarII.Shell.Models.Ladders
{
    public class LadderBrpoVisualModel : IViewModel, IResource
    {
        public LadderBrpoVisualModel(LadderBrpoModel _core)
        {
            Recreate(_core);
        }
        public IResource Create(params object[] args)
        {
            return new LadderBrpoVisualModel((LadderBrpoModel)(args[0]));
        }

        public void Dispose()
        {
            Core = null;
            AllResourceManager.Dispose(this);
        }

        public void Recreate(params object[] args)
        {
            Core = (LadderBrpoModel)(args[0]);
        }

        private LadderBrpoModel core;
        public LadderBrpoModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                LadderBrpoModel _core = core;
                this.core = null;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    if (_core.Visual != null) _core.Visual = null;
                }
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    if (core.Visual != this) core.Visual = this;
                    Update();
                }
            }
        }
        IModel IViewModel.Core { get { return Core; } set { Core = (LadderBrpoModel)value; } }
        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsActive":
                case "Condition":
                    Update();
                    break;
            }
        }


        public int ResourceID
        {
            get;set;
        }
        public LadderUnitViewModel ViewParent { get { return core?.Parent?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }
        public void Update()
        {
            core.Parent.Visual.Update(RenderType.Brpo);
            //Opacity = core.IsActive ? 1.0 : 0.4;
            //switch (core.Condition)
            //{
            //    case LadderBrpoModel.Conditions.NONE: TB_Cond.Text = ""; break;
            //    case LadderBrpoModel.Conditions.OFF: TB_Cond.Text = "0"; break;
            //    case LadderBrpoModel.Conditions.ON: TB_Cond.Text = "1"; break;
            //    case LadderBrpoModel.Conditions.UPEDGE: TB_Cond.Text = "U"; break;
            //    case LadderBrpoModel.Conditions.DOWNEDGE: TB_Cond.Text = "D"; break;
            //    case LadderBrpoModel.Conditions.EDGE: TB_Cond.Text = "E"; break;
            //}
        }

        private LadderDrawingVisual[] visuals = new LadderDrawingVisual[2];
        public LadderDrawingVisual[] Visuals { get { return visuals; }}

    }
}
