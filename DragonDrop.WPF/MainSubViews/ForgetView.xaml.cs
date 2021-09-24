using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.MainSubViews.ViewModels;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace DragonDrop.WPF.MainSubViews
{
    /// <summary>
    /// Interaction logic for ForgetView.xaml
    /// </summary>
    public partial class ForgetView : Window
    {
        public ForgetView()
        {
            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerDataService, CustomerDataService>();

            DataContext = container.Resolve<ForgetViewModel>();
            InitializeComponent();

            EmailBox.Focus();
        }

        private void Close_Click(object sender, RoutedEventArgs e)=>BackToMain();

        private void Close_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BackToMain();
        }

        private void BackToMain()
        {
            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerDataService, CustomerDataService>();

            container.Resolve<MainWindow>().Show();
            Close();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) BackToMain();
        }
    }
}
