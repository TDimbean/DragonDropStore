using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for UserProdDetailWindow.xaml
    /// </summary>
    public partial class UserProdDetailWindow : Window
    {
        public UserProdDetailWindow(int prodId)
        {
            InitializeComponent();

            DataContext = new UserProductDetailsViewModel(prodId);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DetailsStack.Visibility == Visibility.Visible)
            {
                DetailsStack.Visibility = Visibility.Collapsed;
                AdrBtnLabel.Content = "Details";
                return;
            }
            DetailsStack.Visibility = Visibility.Visible;
            AdrBtnLabel.Content = "Hide Details";
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

    }
}
