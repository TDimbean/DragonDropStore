#pragma checksum "..\..\..\..\..\StoreViews\Tabs\SubViews\ValueCellAddWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7BE4307CF59EFE0E27ED82CF58E866B0457821AA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DragonDrop.WPF;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
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


namespace DragonDrop.WPF.StoreViews.Tabs.SubViews {
    
    
    /// <summary>
    /// ValueCellAddWindow
    /// </summary>
    public partial class ValueCellAddWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\..\..\..\StoreViews\Tabs\SubViews\ValueCellAddWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox entryBox;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\..\StoreViews\Tabs\SubViews\ValueCellAddWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button submitBtn;
        
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
            System.Uri resourceLocater = new System.Uri("/DragonDrop.WPF;component/storeviews/tabs/subviews/valuecelladdwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\StoreViews\Tabs\SubViews\ValueCellAddWindow.xaml"
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
            this.entryBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 25 "..\..\..\..\..\StoreViews\Tabs\SubViews\ValueCellAddWindow.xaml"
            this.entryBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.EntryBox_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.submitBtn = ((System.Windows.Controls.Button)(target));
            return;
            case 3:
            
            #line 34 "..\..\..\..\..\StoreViews\Tabs\SubViews\ValueCellAddWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Close_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

