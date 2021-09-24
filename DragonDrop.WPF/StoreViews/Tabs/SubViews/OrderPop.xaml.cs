using DragonDrop.DAL.Entities;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for OrderPop.xaml
    /// </summary>
    public partial class OrderPop : Window
    {
        public OrderPop(Order ord)
        {
            InitializeComponent();

            DataContext = new OrderPopViewModel(ord);
        }

        public void Edit_Click(object sender, RoutedEventArgs e)
        {
            // Sometime, this stub will grow to bring up the Order Edit Window
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        public void DetachContent()

        {

            RemoveLogicalChild(Content);

        }
    }
}
