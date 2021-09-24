using DragonDrop.DAL.Entities;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for ProductPop.xaml
    /// </summary>
    public partial class ProductPop : Window
    {
        public ProductPop(Product prod)
        {
            InitializeComponent();

            DataContext = new ProductPopViewModel(prod);
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

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            DetailsStack.Visibility = DetailsStack.Visibility == Visibility.Visible ?
                Visibility.Collapsed : Visibility.Visible;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        //public void DetachContent()=>RemoveLogicalChild(Content);
    }
}
