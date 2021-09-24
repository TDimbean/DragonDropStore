using DragonDrop.DAL.Entities;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for ProductDetailsPop.xaml
    /// </summary>
    public partial class ProductDetailsPop : Window
    {
        private int _prodId;
        private IReloadableControl _callingTab;
        private StoreView _storeView;

        public ProductDetailsPop(Product prod, IReloadableControl callingTab, StoreView storeView=null)
        {
            _storeView = storeView;
            _callingTab = callingTab;
            _prodId = prod.ProductId;
            InitializeComponent();

            DataContext = new ProductPopViewModel(prod);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            new ProductEditWindow(_prodId, _callingTab, _storeView).Show();
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
