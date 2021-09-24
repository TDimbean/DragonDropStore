using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for ShipSingleOrderWindow.xaml
    /// </summary>
    public partial class ShipSingleOrderWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        private Window _callingView;

        public ShipSingleOrderWindow(int ordId, IOrderDataService service, Window callingView)
        {
            InitializeComponent();

            _callingView = callingView;

            DataContext = new ShipSingleViewModel(ordId, service, this);
        }
            
        private void ToggleDatePicker_Click(object sender, RoutedEventArgs e)
            => datePickerPop.IsOpen = !datePickerPop.IsOpen;

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void DateBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) (DataContext as ShipSingleViewModel).DateTextUpdateCommand.Execute();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Test
        }

        public void RefreshParent()
        {
            (_callingView as IReloadableControl).Reload();
            (_callingView as IRelayReloadControl).RefreshParent();
        }

        public void Stop()=>Close();
    }
}
