﻿#pragma checksum "..\..\..\..\..\Shell\Dialogs\NewProjectDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E1CBD9CA655D6E06607199C818D8CEF9"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using SamSoarII.PLCDevice;
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
    /// NewProjectDialog
    /// </summary>
    public partial class NewProjectDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\..\..\..\Shell\Dialogs\NewProjectDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox PLCTypeComboBox;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\..\..\Shell\Dialogs\NewProjectDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox setting;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\..\Shell\Dialogs\NewProjectDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NameTextBox;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\..\Shell\Dialogs\NewProjectDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PathTextBox;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\..\Shell\Dialogs\NewProjectDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BrowseButton;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\..\Shell\Dialogs\NewProjectDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button EnsureButton;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\..\Shell\Dialogs\NewProjectDialog.xaml"
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
            System.Uri resourceLocater = new System.Uri("/SamSoarII;component/shell/dialogs/newprojectdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Shell\Dialogs\NewProjectDialog.xaml"
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
            this.PLCTypeComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.setting = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 3:
            this.NameTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.PathTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.BrowseButton = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.EnsureButton = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

