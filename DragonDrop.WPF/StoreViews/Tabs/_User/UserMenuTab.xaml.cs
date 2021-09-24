using DragonDrop.DAL.Entities;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using System.Windows;
using System.Windows.Controls;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    /// <summary>
    /// Interaction logic for UserMenuTab.xaml
    /// </summary>
    public partial class UserMenuTab : UserControl
    {
        private StoreView _storeView;
        private string _email;

        public UserMenuTab(StoreView storeView, Customer cust = null)
        {
            InitializeComponent();
            _storeView = storeView;
            UserInfoLabel.Content = "Logged in as " + cust.Name;
            _email = cust.Email;
            _email = _email==(string)null ? string.Empty : _email;
        }

        private void SwitchAccount_Click(object sender, RoutedEventArgs e)
        {
            _storeView.SwitchToMain();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        => new MessageComposerWindow(false, _email).Show();

        private void Suggest_Click(object sender, RoutedEventArgs e)
        => new MessageComposerWindow(true, _email).Show();

        private void Rate_Click(object sender, RoutedEventArgs e)
        => MessageBox.Show("Thank you for your support. You wil be redirected to our Windows Store page shortly.",
                "Rate the App", MessageBoxButton.OK, MessageBoxImage.Information);

        private void Support_Click(object sender, RoutedEventArgs e)
        => MessageBox.Show("One of our Customer Suppport agents will contact you shortly. If the contact information" +
            " on your Account page is no longer valid, please update it and send this request again.",
            "Support", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
