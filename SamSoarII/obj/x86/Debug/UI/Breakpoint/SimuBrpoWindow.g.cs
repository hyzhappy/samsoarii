﻿#pragma checksum "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "78E78F5F1A08FB320F5D2A7674F34C94"
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
using SamSoarII.Simulation.UI.Breakpoint;
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


namespace SamSoarII.Simulation.UI.Breakpoint {
    
    
    /// <summary>
    /// SimuBrpoWindow
    /// </summary>
    public partial class SimuBrpoWindow : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 69 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DG_Main;
        
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
            System.Uri resourceLocater = new System.Uri("/SamSoarII;component/ui/breakpoint/simubrpowindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
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
            
            #line 12 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.OnJumpCommandCanExecuted);
            
            #line default
            #line hidden
            
            #line 13 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnJumpCommandExecuted);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 16 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.OnActiveSwapCommandCanExecuted);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnActiveSwapCommandExecuted);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 20 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.OnReleaseCommandCanExecuted);
            
            #line default
            #line hidden
            
            #line 21 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnReleaseCommandExecuted);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 24 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.OnReleaseAllCommandCanExecuted);
            
            #line default
            #line hidden
            
            #line 25 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnReleaseAllCommandExecuted);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DG_Main = ((System.Windows.Controls.DataGrid)(target));
            
            #line 71 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
            this.DG_Main.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.DG_Main_SelectionChanged);
            
            #line default
            #line hidden
            
            #line 72 "..\..\..\..\..\UI\Breakpoint\SimuBrpoWindow.xaml"
            this.DG_Main.CellEditEnding += new System.EventHandler<System.Windows.Controls.DataGridCellEditEndingEventArgs>(this.DG_Main_CellEditEnding);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

