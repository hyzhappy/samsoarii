using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// LadderBrpoViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class LadderBrpoViewModel : UserControl, IViewModel, IResource
    {
        #region IResource
        
        public int ResourceID { get; set; }

        public IResource Create(params object[] args)
        {
            return new LadderBrpoViewModel((LadderBrpoModel)(args[0]));
        }

        public void Recreate(params object[] args)
        {
            Core = (LadderBrpoModel)(args[0]);
        }

        #endregion

        public LadderBrpoViewModel(LadderBrpoModel _core)
        {
            InitializeComponent();
            Canvas.SetZIndex(this, 1);
            Recreate(_core);
        }
        
        public void Dispose()
        {
            Core = null;
            AllResourceManager.Dispose(this);
        }

        #region Core

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
                    if (_core.View != null) _core.View = null;
                }
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    if (core.View != this) core.View = this;
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

        #endregion

        #region View

        public LadderUnitViewModel ViewParent { get { return core?.Parent?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        public void Update()
        {
            Opacity = core.IsActive ? 1.0 : 0.4;
            switch (core.Condition)
            {
                case LadderBrpoModel.Conditions.NONE: TB_Cond.Text = ""; break;
                case LadderBrpoModel.Conditions.OFF: TB_Cond.Text = "0"; break;
                case LadderBrpoModel.Conditions.ON: TB_Cond.Text = "1"; break;
                case LadderBrpoModel.Conditions.UPEDGE: TB_Cond.Text = "U"; break;
                case LadderBrpoModel.Conditions.DOWNEDGE: TB_Cond.Text = "D"; break;
                case LadderBrpoModel.Conditions.EDGE: TB_Cond.Text = "E"; break;
            }
        }

        #endregion
    }
}
