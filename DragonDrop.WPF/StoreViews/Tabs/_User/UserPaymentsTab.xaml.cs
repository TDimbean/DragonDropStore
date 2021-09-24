using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    /// <summary>
    /// Interaction logic for UserPaymentsTab.xaml
    /// </summary>
    public partial class UserPaymentsTab : UserControl
    {
        private bool _isAdvExpanded = true;
        private UnityContainer _container;
        private IPaymentDataService _service;

        public UserPaymentsTab(IPaymentDataService service, int custId)
        {
            InitializeComponent();
            _service = service;

            _container = new UnityContainer();
            _container.RegisterInstance(service);
            _container.RegisterInstance(custId);

            ResetBtn.Focus();

            LoadData();
        }

        private void LoadData() => DataContext = _container.Resolve<UserPaymentsTabViewModel>();

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            // Get the ID of the Payment by custDG.SelectedItem, then print the receipt
        }

        public void HideAdvCal() => AdvDatePop.IsOpen = false;

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

        private void FilterBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) (DataContext as UserPaymentsTabViewModel).SearchCommand.Execute();
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            var wasPrevAdv = AdvSearch.IsVisible;
            LoadData();
            if (wasPrevAdv) (DataContext as UserPaymentsTabViewModel).AdvVisChangedCommand.Execute();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            AdvDateBox.Text = string.Empty;
            AdvAmountBox.Text = string.Empty;
            ContSearchToggle.Content = "Off";
            AdvFilCombo.SelectedIndex = 0;
        }

        private void ContSearchToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Off", "On");

        private void AdvAmountRangeToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Over: ", "Under: ");

        private void AdvDateRangeToggle_Click(object sender, RoutedEventArgs e)
            => ControlHelpers.ToggleBtnText(e, "Before: ", "After: ");

        private void AdvAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9,.]");

        private void AmountBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var decVal = 0m;
            var parses = decimal.TryParse(AdvAmountBox.Text, out decVal);

            if (parses) AdvAmountBox.Text = string.Format("{0:C}", decVal);

            FilterBox_KeyUp(sender, e);
        }

        private void ToggleAdvDatePop_Click(object sender, RoutedEventArgs e)
            => AdvDatePop.IsOpen = !AdvDatePop.IsOpen;

        private void ResetAdvSort_Click(object sender, RoutedEventArgs e)
        {
            AdvSortCombo.SelectedIndex = 0;
            AdvSortDirToggle.Content = "Ascending";
        }

        private void AdvSortDirToggle_Click(object sender, RoutedEventArgs e)
           => ControlHelpers.ToggleBtnText(e, "Ascending", "Descending");

        private void ResetAdvPage_Click(object sender, RoutedEventArgs e)=>AdvPageBox.Text = "All";

        private void PageSizeBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdvPgSizeTxt_LostFocus(null, e as RoutedEventArgs);
                (DataContext as UserPaymentsTabViewModel).SearchCommand.Execute();
            }
        }

        private void AdvPgSizeTxt_LostFocus(object sender, RoutedEventArgs e)
            => ControlHelpers.UpdatePaging(e, ref ResPerPageLbl);
    }
}
