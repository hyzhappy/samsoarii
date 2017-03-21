using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SamSoarII.UserInterface;
using SamSoarII.LadderInstModel;
using SamSoarII.PLCDevice;
namespace SamSoarII.LadderInstViewModel
{
    public class ShowPropertyDialogEventArgs : EventArgs
    {
        public IPropertyDialog Dialog { get; set; }

        public ShowPropertyDialogEventArgs(IPropertyDialog dialog)
        {
            Dialog = dialog;
        }
    }
    public delegate void ShowPropertyDialogHandler(BaseViewModel sender, ShowPropertyDialogEventArgs e);

    /// <summary>
    /// 梯形图元件基类，抽象类
    /// </summary>
    public abstract class BaseViewModel : UserControl
    {
        public event ShowPropertyDialogHandler ShowPropertyDialogEvent;
        public abstract int X { get; set; }
        public abstract int Y { get; set; }
        public abstract bool IsCommentMode { get; set; }
        public abstract bool IsMonitorMode { get; set; }
        public abstract string InstructionName { get; }
        public abstract BaseModel Model { get; protected set; }
        public List<BaseViewModel> NextElemnets = new List<BaseViewModel>();
        public List<BaseViewModel> SubElements = new List<BaseViewModel>();
        public bool IsSearched { get; set; }
        public virtual ElementType Type { get; }
        public static NullViewModel Null { get { return _nullViewModel; } }
        private static NullViewModel _nullViewModel = new NullViewModel();
        public BaseViewModel()
        {
            MouseDoubleClick += BaseViewModel_MouseDoubleClick;
        }

        private void BaseViewModel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BeginShowPropertyDialog();
            e.Handled = true;
        }

        public abstract IPropertyDialog PreparePropertyDialog();

        public void BeginShowPropertyDialog()
        {
            var dialog = PreparePropertyDialog();
            if (dialog != null)
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                if (ShowPropertyDialogEvent != null)
                {
                    ShowPropertyDialogEvent.Invoke(this, new ShowPropertyDialogEventArgs(dialog));
                }
            }
        }

        public virtual bool Assert()
        {
            return false;
        }

        public virtual void SetSelect(bool e)
        {

        }

        public abstract BaseViewModel Clone();

        public abstract int GetCatalogID();
        /// <summary>
        /// 不包含注释信息的value string, 用于打开工程时使用
        /// </summary>
        /// <param name="valueStrings"></param>
        public abstract void ParseValue(IList<string> valueStrings);
        /// <summary>
        /// 包含注释信息的value string, 一个value，接着一个注释
        /// </summary>
        /// <param name="valueStrings"></param>
        /// <param name="contextDevice"></param>
        public abstract void AcceptNewValues(IList<string> valueStrings, Device contextDevice);

        public abstract IEnumerable<string> GetValueString();

        public abstract void UpdateCommentContent();
    }
}
