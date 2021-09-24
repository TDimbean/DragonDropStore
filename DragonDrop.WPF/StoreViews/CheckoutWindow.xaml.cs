using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DragonDrop.WPF.StoreViews
{
    /// <summary>
    /// Interaction logic for CheckoutWindow.xaml
    /// </summary>
    public partial class CheckoutWindow : Window, IRemoteCloseControl
    {
        private StoreViewModel _storeVM;

        public CheckoutWindow(StoreViewModel storeVM, IOrderDataService ordService, 
            IProductDataService prodService, IOrderItemDataService itemService)
        {
            _storeVM = storeVM;

            InitializeComponent();

            DataContext = new CheckoutViewModel(storeVM, ordService, itemService, prodService);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)=>Close();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
            => _storeVM.UnlockStoreCommand.Execute();

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
                => ControlHelpers.PopComboBoxPlaceholder(e.Source as ComboBox);
      
        public void Alert(string msg) => MessageBox.Show(msg, "Something went wrong.", 
             MessageBoxButton.OK, MessageBoxImage.Exclamation);

        public void Stop() => Close();
    }
}
