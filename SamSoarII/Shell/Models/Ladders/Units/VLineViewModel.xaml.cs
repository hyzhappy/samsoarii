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

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// VLineViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class VLineViewModel : LadderUnitViewModel, IResource
    {
        #region IResource

        public override IResource Create(params object[] args)
        {
            return new VLineViewModel((LadderUnitModel)args[0]);
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

        public VLineViewModel(LadderUnitModel _core)
        {
            InitializeComponent();
            if (_core != null) Recreate(_core);
        }

        public override void Dispose()
        {
            base.Dispose();
            AllResourceManager.Dispose(this);
        }

        public override void Update(int flags = UPDATE_ALL)
        {
            switch (flags)
            {
                case UPDATE_TOP:
                    Canvas.SetTop(this, (Core.Parent.IsExpand ? Core.Parent.UnitBaseTop : 0) 
                        + Y * (IsCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit) + 100);
                    break;
                case UPDATE_LEFT:
                    Canvas.SetLeft(this, X * Global.GlobalSetting.LadderWidthUnit + Global.GlobalSetting.LadderWidthUnit / 2);
                    break;
                case UPDATE_HEIGHT:
                    Height = IsCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit;
                    break;
                default:
                    base.Update(flags);
                    break;
            }
        }
    }
}
