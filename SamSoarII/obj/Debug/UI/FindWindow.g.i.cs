﻿#pragma checksum "..\..\..\UI\FindWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E27585EA3329486B92D08075AC244A34"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using SamSoarII.AppMain.Properties;
using SamSoarII.AppMain.UI;
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


namespace SamSoarII.AppMain.UI {
    
    
    /// <summary>
    /// FindWindow
    /// </summary>
    public partial class FindWindow : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 24 "..\..\..\UI\FindWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox CB_Range;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\UI\FindWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TB_Input;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\UI\FindWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DG_List;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\UI\FindWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn DGTC_Detail;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\UI\FindWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn DGTC_Diagram;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\UI\FindWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn DGTC_Network;
        
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
            System.Uri resourceLocater = new System.Uri("/SamSoarII;component/ui/findwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UI\FindWindow.xaml"
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
            this.CB_Range = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.TB_Input = ((System.Windows.Controls.TextBox)(target));
            
            #line 39 "..\..\..\UI\FindWindow.xaml"
            this.TB_Input.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TB_Input_TextChanged);
            
            #line default
            #line hidden
            
            #line 40 "..\..\..\UI\FindWindow.xaml"
            this.TB_Input.KeyDown += new System.Windows.Input.KeyEventHandler(this.TB_Input_KeyDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.DG_List = ((System.Windows.Controls.DataGrid)(target));
            
            #line 46 "..\..\..\UI\FindWindow.xaml"
            this.DG_List.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.DG_List_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.DGTC_Detail = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 5:
            this.DGTC_Diagram = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 6:
            this.DGTC_Network = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

