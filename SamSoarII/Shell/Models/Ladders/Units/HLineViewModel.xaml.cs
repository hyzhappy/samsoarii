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
using SamSoarII.Utility;

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// HLineViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class HLineViewModel : LadderUnitViewModel, IResource
    {
        #region IResource

        public override IResource Create(params object[] args)
        {
            return new HLineViewModel((LadderUnitModel)args[0]);
        }

        public override void Recreate(params object[] args)
        {
            base.Recreate(args);
            recreating = true;
            DataContext = this;
            Update();
            recreating = false;
        }
        
        #endregion

        public HLineViewModel(LadderUnitModel _core)
        {
            InitializeComponent();
            if (_core != null) Recreate(_core);
        }

        public override void Dispose()
        {
            base.Dispose();
            AllResourceManager.Dispose(this);
        }
    }
}
