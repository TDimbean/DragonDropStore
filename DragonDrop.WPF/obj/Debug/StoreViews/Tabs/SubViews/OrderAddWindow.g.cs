﻿#pragma checksum "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "F39893691DAFFDB057EB825833990974F8CB89D5"
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
    /// OrderAddWindow
    /// </summary>
    public partial class OrderAddWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 147 "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CustBox;
        
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
            System.Uri resourceLocater = new System.Uri("/DragonDrop.WPF;component/storeviews/tabs/subviews/orderaddwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml"
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
            case 2:
            this.CustBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 148 "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml"
            this.CustBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.CustBox_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 189 "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml"
            ((System.Windows.Controls.ComboBox)(target)).SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Combo_SelectionChanged);
            
            #line default
            #line hidden
            
            #line 190 "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml"
            ((System.Windows.Controls.ComboBox)(target)).KeyUp += new System.Windows.Input.KeyEventHandler(this.Combo_KeyUp);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 197 "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml"
            ((System.Windows.Controls.ComboBox)(target)).SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Combo_SelectionChanged);
            
            #line default
            #line hidden
            
            #line 199 "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml"
            ((System.Windows.Controls.ComboBox)(target)).KeyUp += new System.Windows.Input.KeyEventHandler(this.Combo_KeyUp);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 208 "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml"
            ((System.Windows.Controls.ComboBox)(target)).SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Combo_SelectionChanged);
            
            #line default
            #line hidden
            
            #line 210 "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml"
            ((System.Windows.Controls.ComboBox)(target)).KeyUp += new System.Windows.Input.KeyEventHandler(this.Combo_KeyUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 1:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.KeyUpEvent;
            
            #line 121 "..\..\..\..\..\StoreViews\Tabs\SubViews\OrderAddWindow.xaml"
            eventSetter.Handler = new System.Windows.Input.KeyEventHandler(this.TextBox_KeyUp);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            }
        }
    }
}

