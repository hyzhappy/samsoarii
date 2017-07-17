using SamSoarII.Core.Models;
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
using System.Windows.Shapes;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// LadderNetworkCommentEditDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LadderNetworkCommentEditDialog : Window, IDisposable
    {
        public int NetworkNumber { get; set; }
        public string NetworkBrief { get; set; }
        public string NetworkDescription { get; set; }

        public event RoutedEventHandler EnsureButtonClick;

        public LadderNetworkCommentEditDialog(LadderNetworkModel core)
        {
            InitializeComponent();
            NetworkNumber = core.ID;
            NetworkBrief = core.Brief;
            NetworkDescription = core.Description;
            KeyDown += OnKeyboardDown;
            NetworkNumberTextBlock.DataContext = this;
            NetworkBriefTextBox.DataContext = this;
            NetworkDescriptionTextBox.DataContext = this;
        }

        public void Dispose()
        {
            KeyDown -= OnKeyboardDown;
            NetworkNumberTextBlock.DataContext = null;
            NetworkBriefTextBox.DataContext = null;
            NetworkDescriptionTextBox.DataContext = null;
        }


        #region Event handler

        private void OnKeyboardDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
        public void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            if (EnsureButtonClick != null)
            {
                EnsureButtonClick.Invoke(sender, e);
            }
        }

        public void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
