﻿#pragma checksum "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A2E7023E4C0C5EFFB3142FDD7583ACF5"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using SamSoarII.Properties;
using SamSoarII.Shell.Dialogs;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SamSoarII.Shell.Dialogs {
    
    
    /// <summary>
    /// OptionDialog
    /// </summary>
    public partial class OptionDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox MyListBox;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ContentGrid;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button EnsureButton;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CancelButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SamSoarII;component/shell/dialogs/systemoption/optiondialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml"
            ((SamSoarII.Shell.Dialogs.OptionDialog)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.Window_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml"
            ((SamSoarII.Shell.Dialogs.OptionDialog)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.OnClosing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MyListBox = ((System.Windows.Controls.ListBox)(target));
            
            #line 16 "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml"
            this.MyListBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ContentGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.EnsureButton = ((System.Windows.Controls.Button)(target));
            
            #line 54 "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml"
            this.EnsureButton.Click += new System.Windows.RoutedEventHandler(this.OnEnsureButtonClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            
            #line 55 "..\..\..\..\..\..\Shell\Dialogs\SystemOption\OptionDialog.xaml"
            this.CancelButton.Click += new System.Windows.RoutedEventHandler(this.OnCancelButtonClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

