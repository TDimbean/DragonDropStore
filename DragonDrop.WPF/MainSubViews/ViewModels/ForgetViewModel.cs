using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.Infrastructure;
using DragonDrop.WPF.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DragonDrop.WPF.MainSubViews.ViewModels
{
    public class ForgetViewModel : BindableBase
    {
        #region Fields

        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);
        private ICustomerDataService _service;

        #region Mail

        private string _emailBoxText;
        private Brush _emailBtnColour;
        private bool _emailBtnIsEnabled;

        #endregion

        #region SMS

        private string _smsBoxText;
        private Brush _smsBtnColour;
        private bool _smsBtnIsEnabled;

        #endregion

        #endregion

        public ForgetViewModel(ICustomerDataService service)
        {
            _service = service;

            #region Commands

            #region Mail

            EmailBtnClickedCommand = new DelegateCommand(EmailBtnClickedCommandExecute);
            EmailBtnKeyUpCommand = new DelegateCommand<KeyEventArgs>(EmailBtnKeyUpCommandExecute);
            EmailBoxTextChangedCommand = new DelegateCommand<TextChangedEventArgs>(EmailBoxTextChangedCommandExecute);

            #endregion

            #region SMS

            SmsBtnClickedCommand = new DelegateCommand(SmsBtnClickedCommandExecute);
            SmsBtnKeyUpCommand = new DelegateCommand<KeyEventArgs>(SmsBtnKeyUpCommandExecute);
            SmsBoxTextChangedCommand = new DelegateCommand<TextChangedEventArgs>(SmsBoxTextChangedCommandExecute);

            #endregion

            #endregion

            #region Init

            EmailBtnColour = Brushes.Gray;
            EmailBtnIsEnabled = false;

            SmsBtnColour = Brushes.Gray;
            SmsBtnIsEnabled = false;

            #endregion
        }

        #region Properties

        #region ViewModel Props

        #region Mail

        public string EmailBoxText
        {
            get => _emailBoxText;
            set => SetProperty(ref _emailBoxText, value);
        }

        public Brush EmailBtnColour
        {
            get => _emailBtnColour;
            set => SetProperty(ref _emailBtnColour, value);
        }

        public bool EmailBtnIsEnabled
        {
            get => _emailBtnIsEnabled;
            set => SetProperty(ref _emailBtnIsEnabled, value);
        }

        #endregion

        #region SMS

        public string SmsBoxText
        {
            get => _smsBoxText;
            set => SetProperty(ref _smsBoxText, value);
        }

        public Brush SmsBtnColour
        {
            get => _smsBtnColour;
            set => SetProperty(ref _smsBtnColour, value);
        }

        public bool SmsBtnIsEnabled
        {
            get => _smsBtnIsEnabled;
            set => SetProperty(ref _smsBtnIsEnabled, value);
        }

        #endregion

        #endregion

        #region Commands

        #region Mail

        public DelegateCommand EmailBtnClickedCommand { get; }
        public DelegateCommand<KeyEventArgs> EmailBtnKeyUpCommand { get; }
        public DelegateCommand<TextChangedEventArgs> EmailBoxTextChangedCommand { get; }

        #endregion

        #region SMS

        public DelegateCommand SmsBtnClickedCommand { get; }
        public DelegateCommand<KeyEventArgs> SmsBtnKeyUpCommand { get; }
        public DelegateCommand<TextChangedEventArgs> SmsBoxTextChangedCommand { get; }

        #endregion

        #endregion

        #endregion

        #region Private/Not Exposed Methods

        #region Mail

        private void EmailBtnKeyUpCommandExecute(KeyEventArgs e)
        {
            if (e.Key == Key.Enter) EmailBtnClickedCommandExecute();
        }

        private void EmailBtnClickedCommandExecute()
        {
            bool exists;
            try
            {
                exists = _asyncCalls ? _service.EmailExistsAsync(EmailBoxText).Result :
                    _service.EmailExists(EmailBoxText);
            }
            catch
            {
                exists = false;
            }

            if (exists)
            {
                MessageBox.Show("A recovery E-mail has been sent to: " + EmailBoxText,
                "Message Sent", MessageBoxButton.OK, MessageBoxImage.Information);
                StaticLogger.LogInfo(GetType(), "Recovery E-mail requested by: " + EmailBoxText);
            }
            else MessageBox.Show("The address: " + EmailBoxText + " could not be found in our database."
                , "E-mail not found.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void EmailBoxTextChangedCommandExecute(TextChangedEventArgs e)
        {
            var box = e.Source as TextBox;
            if (box.Text.Length!=0)
            {
                EmailBtnColour = Brushes.Gold;
                EmailBtnIsEnabled = true;
            }
            else
            {
                EmailBtnColour = Brushes.Gray;
                EmailBtnIsEnabled = false;
            }
        }

        #endregion

        #region SMS

        private void SmsBtnKeyUpCommandExecute(KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SmsBtnClickedCommandExecute();
        }

        private void SmsBtnClickedCommandExecute()
        {
            bool exists;
            try
            {
                exists = _asyncCalls ? _service.PhoneExistsAsync(SmsBoxText).Result :
                    _service.PhoneExists(SmsBoxText);
            }
            catch
            {
                exists = false;
            }

            if (exists)
            {
                MessageBox.Show("A recovery message has been texted to: " + SmsBoxText,
                "Message Sent", MessageBoxButton.OK, MessageBoxImage.Information);
                StaticLogger.LogInfo(GetType(), "Recovery Text requested by: " + SmsBoxText);
            }
            else MessageBox.Show("The number: " + SmsBoxText + " could not be found in our database."
                , "Phone Number not found.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SmsBoxTextChangedCommandExecute(TextChangedEventArgs e)
        {
            ControlHelpers.HandlePhoneNumberInput(e);

            if ((e.Source as TextBox).Text.Length != 0)
            {
                SmsBtnColour = Brushes.Gold;
                SmsBtnIsEnabled = true;
            }
            else
            {
                SmsBtnColour = Brushes.Gray;
                SmsBtnIsEnabled = false;
            }
        }

        #endregion

        #endregion
    }
}
