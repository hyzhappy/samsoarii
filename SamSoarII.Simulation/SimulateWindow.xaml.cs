using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Configuration;
using SamSoarII.Simulation.Shell.Event;

namespace SamSoarII.Simulation
{
    /// <summary>
    /// SimulateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SimulateWindow : Window
    {
        public SimulateWindow()
        {
            InitializeComponent();
        }

        private void OnSaveMonitorList(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "ssm文件|*.ssm";
            if (saveFileDialog.ShowDialog() == true)
            {
                if (MTable.Save(saveFileDialog.FileName) != 0)
                {
                    MessageBox.Show("无法保存监视文件!");
                }
            }
        }

        private void OnLoadMonitorList(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ssm文件|*.ssm";
            if (openFileDialog.ShowDialog() == true)
            {
                if (MTable.Load(openFileDialog.FileName) != 0)
                {
                    MessageBox.Show("不正确的监视文件，监视文件已损坏!");
                }
            }
        }

        public event ShowTabItemEventHandler OpenChart;
        private void OnOpenChart(object sender, RoutedEventArgs e)
        {
            if (OpenChart != null)
            {
                ShowTabItemEventArgs _e = new ShowTabItemEventArgs("图表");
                OpenChart(sender, _e);
            }
        }
    }
}
