﻿#pragma checksum "..\..\..\..\..\..\Shell\Dialogs\ElementPropertyModify\OutputPropModel.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "11B38937B8E6F744B218426D57A7859F"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// OutputPropModel
    /// </summary>
    public partial class OutputPropModel : SamSoarII.Shell.Dialogs.BasePropModel, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\..\..\..\..\Shell\Dialogs\ElementPropertyModify\OutputPropModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ValueTextBox;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\..\..\Shell\Dialogs\ElementPropertyModify\OutputPropModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas CenterCanvas;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\..\..\Shell\Dialogs\ElementPropertyModify\OutputPropModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock CenterTextBlock;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\..\..\Shell\Dialogs\ElementPropertyModify\OutputPropModel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CountTextBox;
        
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
            System.Uri resourceLocater = new System.Uri("/SamSoarII;component/shell/dialogs/elementpropertymodify/outputpropmodel.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\Shell\Dialogs\ElementPropertyModify\OutputPropModel.xaml"
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
            this.ValueTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 26 "..\..\..\..\..\..\Shell\Dialogs\ElementPropertyModify\OutputPropModel.xaml"
            this.ValueTextBox.GotFocus += new System.Windows.RoutedEventHandler(this.ValueTextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 27 "..\..\..\..\..\..\Shell\Dialogs\ElementPropertyModify\OutputPropModel.xaml"
            this.ValueTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.ValueTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CenterCanvas = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.CenterTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.CountTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 40 "..\..\..\..\..\..\Shell\Dialogs\ElementPropertyModify\OutputPropModel.xaml"
            this.CountTextBox.GotFocus += new System.Windows.RoutedEventHandler(this.CountTextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 41 "..\..\..\..\..\..\Shell\Dialogs\ElementPropertyModify\OutputPropModel.xaml"
            this.CountTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.CountTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

