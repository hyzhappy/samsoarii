using SamSoarII.Core.Models;
using SamSoarII.Shell.Models;
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

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// MainTabDiagramItem.xaml 的交互逻辑
    /// </summary>
    public partial class MainTabDiagramItem : BaseTabItem
    {
        public MainTabDiagramItem
        (
            MainTabControl _tabcontrol,
            LadderDiagramModel _ldmodel = null,
            InstructionDiagramModel _idmodel = null
        ) : base(_tabcontrol)
        {
            InitializeComponent();
            DataContext = this;
            if (_ldmodel.View == null)
                _ldmodel.View = new LadderDiagramViewModel(_ldmodel);
            if (_idmodel.View == null)
                _idmodel.View = new InstructionDiagramViewModel(_idmodel);
            ldvmodel = _ldmodel.View;
            idvmodel = _idmodel.View;
            ldvmodel.PropertyChanged += OnLadderPropertyChanged;
            _ldmodel.Tab = this;
            ViewMode = _tabcontrol.ViewMode;
            InvokePropertyChanged("TabHeader");
        }
        
        #region Number

        override public string TabHeader { get { return ldvmodel != null ? ldvmodel.ProgramName : ""; } }

        private LadderDiagramViewModel ldvmodel;
        public LadderDiagramViewModel LDVModel { get { return this.ldvmodel; } }

        private InstructionDiagramViewModel idvmodel;
        public InstructionDiagramViewModel IDVModel { get { return this.idvmodel; } }

        #endregion
        
        #region View Mode

        public const int VIEWMODE_LADDER = 0x01;
        public const int VIEWMODE_INST = 0x02;

        private int viewmode;
        public int ViewMode
        {
            get { return this.viewmode; }
            set
            {
                this.viewmode = value;
                if ((viewmode & VIEWMODE_LADDER) == 0
                 && (viewmode & VIEWMODE_INST) == 0)
                {
                    Content = null;
                }
                else if ((viewmode & VIEWMODE_LADDER) != 0
                      && (viewmode & VIEWMODE_INST) == 0)
                {
                    GB_Ladder.Content = null;
                    Content = ldvmodel;
                }
                else if ((viewmode & VIEWMODE_LADDER) == 0
                      && (viewmode & VIEWMODE_INST) != 0)
                {
                    GB_Inst.Content = null;
                    Content = idvmodel;
                }
                else
                {
                    Content = G_Main;
                    GB_Ladder.Content = ldvmodel;
                    GB_Inst.Content = idvmodel;
                }
            }
        }
        #endregion

        #region Event Handler
        
        private void OnLadderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ProgramName": InvokePropertyChanged("TabHeader"); break;
            }
        }
        #endregion

    }
}
