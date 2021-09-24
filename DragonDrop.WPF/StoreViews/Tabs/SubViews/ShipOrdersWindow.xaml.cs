using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for ShipOrdersWindow.xaml
    /// </summary>
    public partial class ShipOrdersWindow : Window, IRemoteCloseControl
    {
        private StoreView _storeView;

        public ShipOrdersWindow(IOrderDataService ordServ, StoreView storeView)
        {
            storeView.Lock();
            _storeView = storeView;

            DataContext = new ShipOrdersViewModel(ordServ, this);

            InitializeComponent();
        }

        private ShipOrdersViewModel GetVM => DataContext as ShipOrdersViewModel;

        private void DateBox_KeyUp(object sender, KeyEventArgs e)
        {
                if (e.Key == Key.Enter) GetVM.ShipCommand.Execute();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)=> Close();

        private void ProcDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selOrd = procDG.SelectedItem as Order;
            if (selOrd != null) HandleGridSelection(selOrd.OrderId, false);
        }

        public void RefreshDateVis()
        {
            var vm = GetVM;
            dateBox.Visibility = vm.DateBoxVis;
        }

        public void RefreshArrows()
        {
            var vm = GetVM;

            arrow1.RenderTransform = vm.ArrowOrientation;
        }

        private void ShipDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selOrd = shipDG.SelectedItem as Order;
            if (selOrd != null) HandleGridSelection(selOrd.OrderId, true);
        }

        private void HandleGridSelection(int ordId, bool fromShipped)
            => GetVM.SelectCommand.Execute((ordId, fromShipped));

        private void ToggleDatePicker_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            datePickerPop.IsOpen = !datePickerPop.IsOpen;
        }

        public void Stop() => Close();

        protected override void OnClosing(CancelEventArgs e)
        {
            _storeView.RefreshAdminOrders();
            _storeView.Unlock();
            base.OnClosing(e);
        }
        
        public void EmptyShipSel()=>shipDG.SelectedItem = null;
        public void EmptyProcSel()=>procDG.SelectedItem = null;
    }
}
