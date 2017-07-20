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
using SamSoarII.Core.Models;
using SamSoarII.Core.Update;
using System.ComponentModel;

namespace SamSoarII.Shell.Windows.Update
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : UserControl,IWindow, IViewModel, INotifyPropertyChanged
    {
        private InteractionFacade ifParent;
        public UpdateWindow()
        {
            InitializeComponent();
        }
        public UpdateWindow(InteractionFacade _ifParent)
        {
            ifParent = _ifParent;
            core = ifParent.UDManager;
            InitializeComponent();
            DataContext = this;
            ifParent.PostIWindowEvent += _ifParent_PostIWindowEvent;
            Core.InformationsCountChanged += Core_InformationsCountChanged;
        }

        private void Core_InformationsCountChanged(object sender, EventArgs e)
        {
            if (Informations.Count == 0)
            {
                TB_Inform.Visibility = Visibility.Visible;
                LB_Inform.Visibility = Visibility.Collapsed;
            }
            else
            {
                TB_Inform.Visibility = Visibility.Collapsed; 
                LB_Inform.Visibility = Visibility.Visible;
            }
            PropertyChanged(this,new PropertyChangedEventArgs("Informations"));
        }

        private void _ifParent_PostIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
            
        }

        public void Dispose()
        {
            ifParent.PostIWindowEvent -= _ifParent_PostIWindowEvent;
            Core.InformationsCountChanged -= Core_InformationsCountChanged;
            ifParent = null;
            Core = null;
        }

        private List<InformationUnit> informations = new List<InformationUnit>();
        public List<InformationUnit> Informations
        {
            get
            {
                informations.Clear();
                foreach (var information in Core.Informations)
                {
                    if(!information.Ignore)
                        informations.Add(information.View);
                }
                return informations;
            }
        }

        public InteractionFacade IFParent
        {
            get
            {
                return ifParent;
            }
        }
        private UpdateManager core;
        public UpdateManager Core
        {
            get
            {
                return core;
            }
            set
            {
                core = value;
            }
        }
        
        public IViewModel ViewParent
        {
            get
            {
                return null;
            }
        }

        IModel IViewModel.Core
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        public event IWindowEventHandler Post = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
