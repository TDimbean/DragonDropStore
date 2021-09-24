using System.Windows;
using System.Windows.Controls;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for MessageComposerWindow.xaml
    /// </summary>
    public partial class MessageComposerWindow : Window
    {
        public MessageComposerWindow(bool isSuggestion = false, string email = "No Email Provided")
        {
            InitializeComponent();

            Title = isSuggestion ? "Suggest a feature" : "Report a bug";
            msgLbl.Content = isSuggestion ? "Suggestion" : "Bug Details";
            emailBox.Text = email;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thank you for your feedback! We will get on it as soon as possible and may follow-up with" +
                " questionaires or updates via e-mail.");
            //Open up local Mailing Service and auto-compose a message for the user using an application ressource e-mail for your 
            //company (e.g. suggestions@dragondropstore.com) with the provided message as body and the provided e-mail as login prompt
        }

        private void EditMail_Click(object sender, RoutedEventArgs e)
        => emailBox.IsReadOnly = false;

        private void EmailBox_LostFocus(object sender, RoutedEventArgs e)
        => (e.Source as TextBox).IsReadOnly = true;

        private void Message_TextChanged(object sender, TextChangedEventArgs e)
        => charCountLbl.Content = (e.Source as TextBox).Text.Length + "/1250";
    }
}
