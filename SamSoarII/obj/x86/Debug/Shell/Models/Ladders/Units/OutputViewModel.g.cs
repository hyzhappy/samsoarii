﻿#pragma checksum "..\..\..\..\..\..\..\Shell\Models\Ladders\Units\OutputViewModel.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D7FEED0CCA5125392F7609EFEE18FEFA"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using SamSoarII.Shell.Managers;
using SamSoarII.Shell.Models;
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


namespace SamSoarII.Shell.Models {
    
    
    /// <summary>
    /// OutputViewModel
    /// </summary>
    public partial class OutputViewModel : SamSoarII.Shell.Models.LadderUnitViewModel, System.Windows.Markup.IComponentConnector {
        
        
        #line 52 "..\..\..\..\..\..\..\Shell\Models\Ladders\Units\OutputViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ValueTextBlock;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\..\..\..\..\Shell\Models\Ladders\Units\OutputViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas CenterCanvas;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\..\..\..\..\Shell\Models\Ladders\Units\OutputViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock CenterTextBlock;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\..\..\..\..\Shell\Models\Ladders\Units\OutputViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock CountTextBlock;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\..\..\..\..\Shell\Models\Ladders\Units\OutputViewModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel CommentArea;
        
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
            System.Uri resourceLocater = new System.Uri("/SamSoarII;component/shell/models/ladders/units/outputviewmodel.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\..\Shell\Models\Ladders\Units\OutputViewModel.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.ValueTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.CenterCanvas = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.CenterTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.CountTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.CommentArea = ((System.Windows.Controls.StackPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

