using DragonDrop.Infrastructure.DTOs;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for EditDefaultsWindow.xaml
    /// </summary>
    public partial class EditDefaultsWindow : Window, IReloadableControl
    {
        private DefaultValueDto _swapCell1;

        private Style _visStyle = new Style(typeof(Image));
        private Style _hidStyle = new Style(typeof(Image));

        public EditDefaultsWindow()
        {
            InitializeComponent();

            DataContext = new EditDefaultsViewModel();
        }

        private int CurrentRepoType() => (DataContext as EditDefaultsViewModel).RepoType;

        public void Reload()
            => DataContext = new EditDefaultsViewModel((DataContext as EditDefaultsViewModel).RepoType);

        private void EditCell_Click(object sender, RoutedEventArgs e)
            => new ValueCellEditWindow(
                this, custDG.SelectedItem as DefaultValueDto, CurrentRepoType())
                .Show();

        private void NewRow_Click(object sender, RoutedEventArgs e)
            => new ValueCellAddWindow(this, (DataContext as EditDefaultsViewModel).RepoType).Show();

        private void SwapCell_Click(object sender, RoutedEventArgs e)
        {
            if (_swapCell1 == null)
            {
                _swapCell1 = custDG.SelectedItem as DefaultValueDto;
                return;
            }

            var container = new UnityContainer();

            var swapCell2 = custDG.SelectedItem as DefaultValueDto;
            var temp = swapCell2.Name;

            var repo = ResRepoHelpers.GetRepo(CurrentRepoType());
            repo.Update(new DefaultValueDto { ID = swapCell2.ID, Name = _swapCell1.Name },true);
            repo.Update(new DefaultValueDto { ID = _swapCell1.ID, Name = temp },true);

            _swapCell1 = null;
            Reload();
        }

        private void DelCell_Click(object sender, RoutedEventArgs e)
            => new ValueCellRemoveWindow
            (this, custDG.SelectedItem as DefaultValueDto, CurrentRepoType())
            .Show();

        private void ResetValues_Click(object sender, RoutedEventArgs e)
        {
            var decission = MessageBox.Show("Are you sure you want to flush the current default values and reload the original ones?",
                "Reset Values", MessageBoxButton.YesNo);
            if (decission == MessageBoxResult.No) return;

            ResRepoHelpers.GetRepo(CurrentRepoType()).Reset();

            _swapCell1 = null;
            Reload();
        }
    
        private void Destroy_Click(object sender, RoutedEventArgs e)
        {
            var decission = MessageBox.Show("Are you sure you want to flush all values?",
                "Nuke Values", MessageBoxButton.YesNo);
            if (decission == MessageBoxResult.No) return;
            
            ResRepoHelpers.GetRepo(CurrentRepoType()).Nuke();

            _swapCell1 = null;
            Reload();
        }
    }
}
