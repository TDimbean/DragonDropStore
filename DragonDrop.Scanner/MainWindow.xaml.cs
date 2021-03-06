using DragonDrop.BLL.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DragonDrop.Scanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MockScannerViewModel(new ProductDataService(new ProductRepository(new DragonDrop_DbContext())));
        }

        private void CodeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
           => LimitToDigits(e);

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => LimitToDigits(e);

        private void LimitToDigits(TextCompositionEventArgs e)
            => e.Handled = new Regex("[^0-9]").IsMatch(e.Text);

        protected override void OnClosing(CancelEventArgs e)
            => (DataContext as MockScannerViewModel).ShutDownCommand.Execute();
    }
}
