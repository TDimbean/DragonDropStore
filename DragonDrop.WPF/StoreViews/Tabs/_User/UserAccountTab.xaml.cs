using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    /// <summary>
    /// Interaction logic for UserAccountTab.xaml
    /// </summary>
    public partial class UserAccountTab : UserControl
    {
        public UserAccountTab(ICustomerDataService service, Customer cust)
        {
            InitializeComponent();

            DataContext = new UserAccountTabViewModel(cust, service, this);
        }

        public void ValidationMessage(string msg)
            => MessageBox.Show(msg, "Validation Summary", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
