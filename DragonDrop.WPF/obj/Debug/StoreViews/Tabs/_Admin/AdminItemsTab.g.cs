#pragma checksum "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CA112E878B5E132DDFB99802EED04BAEAED49556"
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
using DragonDrop.WPF.StoreViews.Tabs;
using DragonDrop.WPF.StoreViews.Tabs.Converters;
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


namespace DragonDrop.WPF.StoreViews.Tabs {
    
    
    /// <summary>
    /// AdminItemsTab
    /// </summary>
    public partial class AdminItemsTab : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 156 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox IdBox;
        
        #line default
        #line hidden
        
        
        #line 164 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ResetBtn;
        
        #line default
        #line hidden
        
        
        #line 251 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox AdvSortCombo;
        
        #line default
        #line hidden
        
        
        #line 255 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AdvSortDirToggle;
        
        #line default
        #line hidden
        
        
        #line 270 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox AdvPageBox;
        
        #line default
        #line hidden
        
        
        #line 277 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ResPerPageLbl;
        
        #line default
        #line hidden
        
        
        #line 436 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid custDG;
        
        #line default
        #line hidden
        
        
        #line 473 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTemplateColumn ButtonsCollumn;
        
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
            System.Uri resourceLocater = new System.Uri("/DragonDrop.WPF;component/storeviews/tabs/_admin/adminitemstab.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
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
            
            #line 148 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ResetIdFilter_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 154 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.IdToggle_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.IdBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 156 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            this.IdBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.IntBox_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 157 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            this.IdBox.KeyUp += new System.Windows.Input.KeyEventHandler(this.IntBox_KeyUp);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ResetBtn = ((System.Windows.Controls.Button)(target));
            
            #line 164 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            this.ResetBtn.Click += new System.Windows.RoutedEventHandler(this.ResetBtn_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 245 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ResetAdvSort_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.AdvSortCombo = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.AdvSortDirToggle = ((System.Windows.Controls.Button)(target));
            
            #line 256 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            this.AdvSortDirToggle.Click += new System.Windows.RoutedEventHandler(this.AdvSortDirToggle_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 261 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ResetAdvPage_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.AdvPageBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 269 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            this.AdvPageBox.LostFocus += new System.Windows.RoutedEventHandler(this.AdvPgSizeTxt_LostFocus);
            
            #line default
            #line hidden
            
            #line 270 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            this.AdvPageBox.KeyUp += new System.Windows.Input.KeyEventHandler(this.PageSizeBox_KeyUp);
            
            #line default
            #line hidden
            return;
            case 10:
            this.ResPerPageLbl = ((System.Windows.Controls.Label)(target));
            return;
            case 11:
            
            #line 374 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.EditOrder_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 425 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.EditProduct_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.custDG = ((System.Windows.Controls.DataGrid)(target));
            
            #line 437 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            this.custDG.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.CustDG_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 14:
            this.ButtonsCollumn = ((System.Windows.Controls.DataGridTemplateColumn)(target));
            return;
            case 15:
            
            #line 475 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NewEntry_Click);
            
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
            switch (connectionId)
            {
            case 16:
            
            #line 481 "..\..\..\..\..\StoreViews\Tabs\_Admin\AdminItemsTab.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.EditEntry_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

