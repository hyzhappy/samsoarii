﻿#pragma checksum "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B03337E247A7ABE42E249F414BA45E4E"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using SamSoarII.AppMain;
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
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


namespace SamSoarII.AppMain.Project {
    
    
    /// <summary>
    /// FuncBlockViewModel
    /// </summary>
    public partial class FuncBlockViewModel : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 24 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ICSharpCode.AvalonEdit.TextEditor CodeTextBox;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid CodeCompletePanel;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle Scroll;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border Cursor;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Profix;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Line ProfixCursor;
        
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
            System.Uri resourceLocater = new System.Uri("/SamSoarII;component/project/program/funcblockviewmodel.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
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
            
            #line 13 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.FindCommandCanExecute);
            
            #line default
            #line hidden
            
            #line 13 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnFindCommandExecute);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 14 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.ReplaceCommandCanExecute);
            
            #line default
            #line hidden
            
            #line 14 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnReplaceCommandExecute);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 15 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OnSelectAllCommandExecute);
            
            #line default
            #line hidden
            return;
            case 4:
            this.CodeTextBox = ((ICSharpCode.AvalonEdit.TextEditor)(target));
            return;
            case 5:
            this.CodeCompletePanel = ((System.Windows.Controls.Grid)(target));
            
            #line 33 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            this.CodeCompletePanel.MouseMove += new System.Windows.Input.MouseEventHandler(this.CodeCompletePanel_MouseMove);
            
            #line default
            #line hidden
            
            #line 34 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            this.CodeCompletePanel.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.CodeCompletePanel_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Scroll = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 61 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            this.Scroll.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Scroll_MouseEnter);
            
            #line default
            #line hidden
            
            #line 62 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            this.Scroll.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Scroll_MouseLeave);
            
            #line default
            #line hidden
            
            #line 63 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            this.Scroll.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Scroll_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 64 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            this.Scroll.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.Scroll_MouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 65 "..\..\..\..\..\Project\Program\FuncBlockViewModel.xaml"
            this.Scroll.MouseMove += new System.Windows.Input.MouseEventHandler(this.Scroll_MouseMove);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Cursor = ((System.Windows.Controls.Border)(target));
            return;
            case 8:
            this.Profix = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.ProfixCursor = ((System.Windows.Shapes.Line)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

