using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.MainSubViews.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace DragonDrop.WPF.MainSubViews
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : Window, IRemoteCloseControl
    {
        public RegisterView()
        {
            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerDataService, CustomerDataService>();
            container.RegisterInstance(this as IRemoteCloseControl);

            DataContext = container.Resolve<RegisterViewModel>();
            InitializeComponent();

            NameBox.Focus();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ReturnToMain();
            base.OnClosing(e);
        }

        public void ReturnToMain()
        {
            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerDataService, CustomerDataService>();

            container.Resolve<MainWindow>().Show();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        public void Stop() => Close();
    }
}
