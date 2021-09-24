using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    /// <summary>
    /// Interaction logic for UserOrdersTab.xaml
    /// </summary>
    public partial class UserOrdersTab : UserControl, IReloadableControl
    {
        private bool _isAdvExpanded = true;
        private UnityContainer _container;

        public UserOrdersTab(IOrderDataService service, int custId)
        {
            InitializeComponent();

            _container = new UnityContainer();
            _container.RegisterType<IOrderRepository, OrderRepository>();
            _container.RegisterType<IOrderDataService, OrderDataService>();
            _container.RegisterInstance(custId);

            ResetBtn.Focus();

            LoadData();
        }

        private void LoadData() => DataContext = _container.Resolve<UserOrdersTabViewModel>();

        private void AdvSearchExpanderBtn_Click(object sender, RoutedEventArgs e)
        {
            _isAdvExpanded = !_isAdvExpanded;
            if (_isAdvExpanded)
            {
                AdvSearch.Visibility = Visibility.Collapsed;
                AdvSearchArrow.RenderTransform = new ScaleTransform(1, 1);
                custDG.Height = 760;
                return;
            }
            AdvSearch.Visibility = Visibility.Visible;
            AdvSearchArrow.RenderTransform = new ScaleTransform(1, -1);
            custDG.Height = 646;

            // GRID HEIGHT
            //Very wonky behaviour, don't change the default height. In theory, the minHeight(when Advanced Search is expanded) should
            //be the regular height - 94(Combined height of all 3 AdvSearch Rows). It was initially calculated with the def height
            //being 750, yet it seems the DataGrid only changed heights at certain intervals, so there's no use fiddling with it.
        }

        private void AdvSortDirToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Ascending", "Descending");

        private void ContSearchToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Off", "On");

        private void AdvPgSizeTxt_LostFocus(object sender, RoutedEventArgs e)
                 => ControlHelpers.UpdatePaging(e, ref ResPerPageLbl);

        // Resets

        private void ResetContSearch_Click(object sender, RoutedEventArgs e)
        {
            if ((string)ContSearchToggle.Content == "On") ContSearchToggle.PerformClick();
        }

        private void ResetAdvSort_Click(object sender, RoutedEventArgs e)
        {
            AdvSortCombo.SelectedIndex = 0;
            AdvSortDirToggle.Content = "Ascending";
        }

        private void ResetAdvPage_Click(object sender, RoutedEventArgs e)
        {
            AdvPageBox.Text = "All";
        }

        private void PageSizeBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdvPgSizeTxt_LostFocus(null, e as RoutedEventArgs);
                (DataContext as UserOrdersTabViewModel).SearchCommand.Execute();
            }
        }

        private void FilterBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) (DataContext as UserOrdersTabViewModel).SearchCommand.Execute();
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e) => LoadData();

        public void Reload() => ResetBtn.PerformClick();

        private void EntryOptions_Click(object sender, RoutedEventArgs e)
        => new UserOrderOptions(custDG.SelectedItem as Order).Show();
    }
}
