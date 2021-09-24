using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Debug;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Unity;

namespace DragonDrop.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent,
           new MouseButtonEventHandler(SelectivelyHandleMouseButton), true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent,
              new RoutedEventHandler(SelectAllText), true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.LostFocusEvent,
                new RoutedEventHandler(AbandonText), true);

            base.OnStartup(e);
            GoToMain();
        }

        private static void AbandonText(object sender, RoutedEventArgs e)
        {
            var box = e.Source as TextBox;
            if ((string)box.Tag == "LosslessTxtBox" ||
                (string)box.Tag == "SelectableTxtBox") return;
                box.Foreground = Brushes.DarkGray;
                box.FontStyle = FontStyles.Italic;
        }

        private static void SelectivelyHandleMouseButton(object sender, MouseButtonEventArgs e)
        {
            var box = sender as TextBox;
            //if ((string)box.Tag == "LosslessTxtBox" || (string)box.Tag == "SelectableTxtBox") return;
            if (!box.IsKeyboardFocusWithin)
            {
                if (e.OriginalSource.GetType().Name == "TextBoxView")
                {
                    e.Handled = true;
                    box.Focus();
                }
            }
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var box = e.OriginalSource as TextBox;
            if ((string)box.Tag == "LosslessTxtBox") return;
            if ((string)box.Tag!="SelectableTxtBox")
            {
                box.Foreground = Brushes.Black;
                box.FontStyle = FontStyles.Normal;
            }
                box.SelectAll();
        }

        private static void GoToMain()
        {
            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerDataService, CustomerDataService>();

#if DEBUG
            var debugRepo = container.Resolve<DebugRepository>();

            debugRepo.RemoveNewOrders();
            debugRepo.ResetOrderStatuses();
            debugRepo.RemoveNewCustomers();
#endif

            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => GoToMain();

        private void Close_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) GoToMain();
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) GoToMain();
        }
    }
}
