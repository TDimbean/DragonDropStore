using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.WPF.MainSubViews;
using DragonDrop.WPF.StoreViews;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows;
using System.Windows.Input;

namespace DragonDrop.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NameValueCollection _appConfig;
        private ICustomerDataService _service;

        public MainWindow(ICustomerDataService service)
        {
            _service = service;

            DataContext = new MainWindowViewModel(service);
            InitializeComponent();

            _appConfig = ConfigurationManager.AppSettings;
            VersionLabel.Content = "Version: " + _appConfig.Get("Version_Number");
            CopyrightLabel.Content = _appConfig.Get("Copyright_Text");

            AdrBox.Focus();
        }

        private void AccessButton_Click(object sender, RoutedEventArgs e) => SwitchToStore();
        private void Access_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SwitchToStore();
        }
        private void ForgetBtn_Click(object sender, RoutedEventArgs e) => SwitchToForget();
        private void RegisterBtn_Click(object sender, RoutedEventArgs e) => SwitchToRegister();
        private void ForgetBtn_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SwitchToForget();
        }
        private void RegisterBtn_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SwitchToRegister();
        }

        private void SwitchToStore()
        {
            if (AdrApproved.Visibility == Visibility.Hidden || PassApproved.Visibility == Visibility.Hidden)
            {
                var acc = _appConfig.Get("Admin_Account");
                var pass = _appConfig.Get("Admin_Password");

                if (AdrBox.Text==acc && PassBox.Password==pass)
                {
                    new StoreView(true).Show();
                    Close();
                    return;
                }
                return;
            }

            var customer = _service.Get(_service.FindIdByEmail(AdrBox.Text).GetValueOrDefault());

            if (customer == null)
            {
                MessageBox.Show("There was a problem signing into your account.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new StoreView(cust: customer).Show();
            Close();
        }
        private void SwitchToForget()
        {
            new ForgetView().Show();
            Close();
        }
        private void SwitchToRegister()
        {
            new RegisterView().Show();
            Close();
        }

        private void PassBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SwitchToStore();
                return;
            }

            var vm = (DataContext as MainWindowViewModel);
            vm.PassBoxText = PassBox.Password;
            vm.PassBoxTextChangedCommand.Execute();

        }

        private void AdrBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) PassBox.Focus();
        }
    }
}
