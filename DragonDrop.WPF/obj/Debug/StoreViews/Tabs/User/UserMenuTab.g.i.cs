﻿#pragma checksum "..\..\..\..\..\StoreViews\Tabs\User\UserMenuTab.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "AB8A8CACA1915FD1C3B918457231E436E9A2EA39"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DragonDrop.WPF.StoreViews.Tabs;
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


namespace DragonDrop.WPF.StoreViews.Tabs {
    
    
    /// <summary>
    /// UserMenuTab
    /// </summary>
    public partial class UserMenuTab : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\..\StoreViews\Tabs\User\UserMenuTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DragonDrop.WPF.StoreViews.Tabs.UserMenuTab UserMenuTabUC;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\..\StoreViews\Tabs\User\UserMenuTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label UserInfoLabel;
        
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
            System.Uri resourceLocater = new System.Uri("/DragonDrop.WPF;component/storeviews/tabs/user/usermenutab.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\StoreViews\Tabs\User\UserMenuTab.xaml"
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
            this.UserMenuTabUC = ((DragonDrop.WPF.StoreViews.Tabs.UserMenuTab)(target));
            return;
            case 2:
            
            #line 29 "..\..\..\..\..\StoreViews\Tabs\User\UserMenuTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SwitchAccount_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 30 "..\..\..\..\..\StoreViews\Tabs\User\UserMenuTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Quit_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 31 "..\..\..\..\..\StoreViews\Tabs\User\UserMenuTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Report_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 32 "..\..\..\..\..\StoreViews\Tabs\User\UserMenuTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Suggest_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 33 "..\..\..\..\..\StoreViews\Tabs\User\UserMenuTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Rate_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 34 "..\..\..\..\..\StoreViews\Tabs\User\UserMenuTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Support_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.UserInfoLabel = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

