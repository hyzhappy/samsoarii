using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamSoarII.LadderInstViewModel;
using System.Configuration;
using System.ComponentModel;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// FontSelectionWidget.xaml 的交互逻辑
    /// </summary>
    public partial class FontSelectionWidget : UserControl
    {
        private FontDataItem current;
        public FontDataItem Current
        {
            get
            {
                return this.current;
            }
            set
            {
                this.current = value;
                for (int i = 0; i < CB_Size.Items.Count; i++)
                {
                    ComboBoxItem cbitem = (ComboBoxItem)(CB_Size.Items[i]);
                    int size = (int)(cbitem.Content);
                    if (size == current.Data.FontSize)
                    {
                        CB_Size.SelectedIndex = i;
                        break;
                    }
                }
                for (int i = 0; i < CB_Family.Items.Count; i++)
                {
                    FontFamilyItem ffitem = (FontFamilyItem)(CB_Family.Items[i]);
                    if (ffitem.Family.Name.Equals(current.Data.FontFamily.ToString()))
                    {
                        CB_Family.SelectedIndex = i;
                        break;
                    }
                }
                BD_Color.Background = current.Data.FontColor;
                if (current.Data.Name == Properties.Resources.FuncBlock)
                    Demo.ShowFuncBlock();
                else
                    Demo.ShowDiagram();
            }
        }

        public FontSelectionWidget()
        {
            InitializeComponent();
            Initialize();
        }
        
        private void Initialize()
        {
            FontManager.GetLadder().Name = Properties.Resources.Element;
            FontManager.GetTitle().Name = Properties.Resources.Ladder_Title;
            FontManager.GetComment().Name = Properties.Resources.Comment;
            FontManager.GetFunc().Name = Properties.Resources.FuncBlock;
            DemoFontManager.GetLadder().Setup(FontManager.GetLadder());
            DemoFontManager.GetTitle().Setup(FontManager.GetTitle());
            DemoFontManager.GetComment().Setup(FontManager.GetComment());
            DemoFontManager.GetFunc().Setup(FontManager.GetFunc());
            CB_Range.Items.Add(new FontDataItem(DemoFontManager.GetLadder()));
            CB_Range.Items.Add(new FontDataItem(DemoFontManager.GetTitle()));
            CB_Range.Items.Add(new FontDataItem(DemoFontManager.GetComment()));
            CB_Range.Items.Add(new FontDataItem(DemoFontManager.GetFunc()));
            foreach (var fontFamily in (new InstalledFontCollection()).Families)
            {
                FontFamilyItem ffitem = new FontFamilyItem(fontFamily);
                CB_Family.Items.Add(ffitem);
            }
            for (int i = 12; i <= 50; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i;
                CB_Size.Items.Add(item);
            }
            CB_Range.SelectedIndex = 0;
        }
        
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == CB_Range)
            {
                FontDataItem fditem = (FontDataItem)(CB_Range.SelectedItem);
                Current = fditem;
                Demo.SetFontColor(Current.Data.FontColor, CB_Range.SelectedIndex);
            }
            if (sender == CB_Size)
            {
                ComboBoxItem cbitem = (ComboBoxItem)(CB_Size.SelectedItem);
                Current.Data.FontSize = (int)(cbitem.Content);
                Demo.SetFontSize(Current.Data.FontSize,CB_Range.SelectedIndex);
            }
            if (sender == CB_Family)
            {
                FontFamilyItem ffitem = (FontFamilyItem)(CB_Family.SelectedItem);
                Current.Data.FontFamily = new FontFamily(ffitem.Family.Name);
                Demo.SetFontFamily(ffitem.Family.Name, CB_Range.SelectedIndex);
            }
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorSettingDialog csdialog = new ColorSettingDialog(
                Current.Data, ColorSettingDialog.TYPE_FONT);
            csdialog.Closed += (sender1, e1) =>
            {
                BD_Color.Background = Current.Data.FontColor;
                Demo.SetFontColor(Current.Data.FontColor,CB_Range.SelectedIndex);
            };
            csdialog.ShowDialog();
        }
    }
    
    public class FontDataItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        private FontData data;
        public FontData Data
        {
            get { return this.data; }
            private set
            {
                this.data = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Data"));
            }
        }

        public override string ToString()
        {
            return Data.Name;
        }
        
        public FontDataItem(FontData _data)
        {
            Data = _data;
        }
    }
    
    public class FontFamilyItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private System.Drawing.FontFamily family;
        public System.Drawing.FontFamily Family
        {
            get
            {
                return this.family;
            }
            private set
            {
                this.family = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Family"));
            }
        }

        public override string ToString()
        {
            return Family.Name;
        }

        public FontFamilyItem(System.Drawing.FontFamily _family)
        {
            Family = _family;
        }
    }
}
