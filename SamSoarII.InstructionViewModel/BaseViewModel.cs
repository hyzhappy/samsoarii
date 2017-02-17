using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SamSoarII.UserInterface;
using SamSoarII.InstructionModel;
namespace SamSoarII.InstructionViewModel
{
    public abstract class BaseViewModel : UserControl
    {
        protected int _x;
        protected int _y;
        public virtual int X
        {
            get { return _x; }
            set
            {
                _x = value;
                Canvas.SetLeft(this, X * 300);
            }
        }
        public virtual int Y
        {
            get { return _y; }
            set
            {
                _y = value;
                Canvas.SetTop(this, Y * 300);
            }
        }
        public abstract string InstructionName { get; }
        public abstract BaseModel Model { get; protected set; }
        public List<BaseViewModel> NextElemnets = new List<BaseViewModel>();
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
        }

        public abstract void ShowPropertyDialog(ElementPropertyDialog dialog);

        public void BeginShowPropertyDialog()
        {
            ElementPropertyDialog dialog = new ElementPropertyDialog();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ShowPropertyDialog(dialog);       
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

        public abstract void ParseValue(List<string> valueStrings);

        public abstract IEnumerable<string> GetValueString();
    }
}
