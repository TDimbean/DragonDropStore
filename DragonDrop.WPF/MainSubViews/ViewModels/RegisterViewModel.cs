using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DragonDrop.WPF.MainSubViews.ViewModels
{
    public class RegisterViewModel : BindableBase
    {
        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);
        private IRemoteCloseControl _callingView;

        #region Fields

        private Dictionary<string, List<string>> _validationErrors;

        #region Name
        private Visibility _nameCheckVisibility;
        private Visibility _nameErrorVisibility;
        private string _nameErrorText;
        private string _nameBoxText;
        #endregion

        #region Email
        private Visibility _emailCheckVisibility;
        private Visibility _emailErrorVisibility;
        private string _emailErrorText;
        private string _emailBoxText;
        #endregion

        #region Phone
        private Visibility _phoneCheckVisibility;
        private Visibility _phoneErrorVisibility;
        private string _phoneErrorText;
        private string _phoneBoxText;
        #endregion

        #region Address
        private Visibility _adrCheckVisibility;
        private Visibility _adrErrorVisibility;
        private string _adrErrorText;
        private string _adrBoxText;
        #endregion

        #region City
        private Visibility _cityCheckVisibility;
        private Visibility _cityErrorVisibility;
        private string _cityErrorText;
        private string _cityBoxText;
        #endregion

        #region State
        private Visibility _stateCheckVisibility;
        private Visibility _stateErrorVisibility;
        private string _stateErrorText;
        private string _stateBoxText;
        #endregion

        private readonly ICustomerDataService _service;

        #endregion

        public RegisterViewModel(ICustomerDataService service, IRemoteCloseControl callingView = null)
        {
            _callingView = callingView;
            _service = service;

            #region Commands

            SubmitBtnClickCommand = _asyncCalls ? new DelegateCommand(SubmitBtnClickCommandExecuteAsync) :
                new DelegateCommand(SubmitBtnClickCommandExecute);
     
            EmailTextChangedCommand = new DelegateCommand(EmailTextChangedCommandExecute);

            PhoneTextChangedCommand = new DelegateCommand<TextChangedEventArgs>(PhoneTextChangedCommandExecute);

            AdrTextChangedCommand = new DelegateCommand(AdrTextChangedCommandExecute);

            CityTextChangedCommand = new DelegateCommand(CityTextChangedCommandExecute);

            StateTextChangedCommand = new DelegateCommand(StateTextChangedCommandExecute);

            #endregion

            #region Init

            ValidationErrors = new Dictionary<string, List<string>>
            {
                {"Name", new List<string>() },
                {"Email", new List<string>() },
                {"Phone", new List<string>() },
                {"Address", new List<string>() },
                {"City", new List<string>() },
                {"State", new List<string>() }
            };

            #region Name

            NameCheckVisibility = Visibility.Hidden;
            NameErrorVisibility = Visibility.Hidden;
            NameErrorText = "I am ERROR.";

            #endregion

            #region Email

            EmailCheckVisibility = Visibility.Hidden;
            EmailErrorVisibility = Visibility.Hidden;
            EmailErrorText = "I am ERROR.";

            #endregion

            #region Phone

            PhoneCheckVisibility = Visibility.Hidden;
            PhoneErrorVisibility = Visibility.Hidden;
            PhoneErrorText = "I am ERROR.";

            #endregion

            #region Address

            AdrCheckVisibility = Visibility.Hidden;
            AdrErrorVisibility = Visibility.Hidden;
            AdrErrorText = "I am ERROR.";

            #endregion

            #region City

            CityCheckVisibility = Visibility.Hidden;
            CityErrorVisibility = Visibility.Hidden;
            CityErrorText = "I am ERROR.";

            #endregion

            #region State

            StateCheckVisibility = Visibility.Hidden;
            StateErrorVisibility = Visibility.Hidden;
            StateErrorText = "I am ERROR.";

            #endregion

            #endregion
        }

        #region Properties

        #region ViewModel Props

        public Dictionary<string, List<string>> ValidationErrors
        {
            get => _validationErrors;
            set => SetProperty(ref _validationErrors, value);
        }

        #region Name

        public Visibility NameCheckVisibility
        {
            get => _nameCheckVisibility;
            set => SetProperty(ref _nameCheckVisibility, value);
        }

        public Visibility NameErrorVisibility
        {
            get => _nameErrorVisibility;
            set => SetProperty(ref _nameErrorVisibility, value);
        }

        public string NameErrorText
        {
            get => _nameErrorText;
            set => SetProperty(ref _nameErrorText, value);
        }

        public string NameBoxText
        {
            get => _nameBoxText;
            set => SetProperty(ref _nameBoxText, value);
        }

        #endregion

        #region Email

        public Visibility EmailCheckVisibility
        {
            get => _emailCheckVisibility;
            set => SetProperty(ref _emailCheckVisibility, value);
        }

        public Visibility EmailErrorVisibility
        {
            get => _emailErrorVisibility;
            set => SetProperty(ref _emailErrorVisibility, value);
        }

        public string EmailErrorText
        {
            get => _emailErrorText;
            set => SetProperty(ref _emailErrorText, value);
        }

        public string EmailBoxText
        {
            get => _emailBoxText;
            set => SetProperty(ref _emailBoxText, value);
        }

        #endregion

        #region Phone

        public Visibility PhoneCheckVisibility
        {
            get => _phoneCheckVisibility;
            set => SetProperty(ref _phoneCheckVisibility, value);
        }

        public Visibility PhoneErrorVisibility
        {
            get => _phoneErrorVisibility;
            set => SetProperty(ref _phoneErrorVisibility, value);
        }

        public string PhoneErrorText
        {
            get => _phoneErrorText;
            set => SetProperty(ref _phoneErrorText, value);
        }

        public string PhoneBoxText
        {
            get => _phoneBoxText;
            set => SetProperty(ref _phoneBoxText, value);
        }

        #endregion

        #region Address

        public Visibility AdrCheckVisibility
        {
            get => _adrCheckVisibility;
            set => SetProperty(ref _adrCheckVisibility, value);
        }

        public Visibility AdrErrorVisibility
        {
            get => _adrErrorVisibility;
            set => SetProperty(ref _adrErrorVisibility, value);
        }

        public string AdrErrorText
        {
            get => _adrErrorText;
            set => SetProperty(ref _adrErrorText, value);
        }

        public string AdrBoxText
        {
            get => _adrBoxText;
            set => SetProperty(ref _adrBoxText, value);
        }

        #endregion

        #region City

        public Visibility CityCheckVisibility
        {
            get => _cityCheckVisibility;
            set => SetProperty(ref _cityCheckVisibility, value);
        }

        public Visibility CityErrorVisibility
        {
            get => _cityErrorVisibility;
            set => SetProperty(ref _cityErrorVisibility, value);
        }

        public string CityErrorText
        {
            get => _cityErrorText;
            set => SetProperty(ref _cityErrorText, value);
        }

        public string CityBoxText
        {
            get => _cityBoxText;
            set => SetProperty(ref _cityBoxText, value);
        }

        #endregion

        #region State

        public Visibility StateCheckVisibility
        {
            get => _stateCheckVisibility;
            set => SetProperty(ref _stateCheckVisibility, value);
        }

        public Visibility StateErrorVisibility
        {
            get => _stateErrorVisibility;
            set => SetProperty(ref _stateErrorVisibility, value);
        }

        public string StateErrorText
        {
            get => _stateErrorText;
            set => SetProperty(ref _stateErrorText, value);
        }

        public string StateBoxText
        {
            get => _stateBoxText;
            set => SetProperty(ref _stateBoxText, value);
        }

        #endregion

        #endregion

        #region Commands

        public DelegateCommand SubmitBtnClickCommand { get; }

        public DelegateCommand NameTextChangedCommand { get; }

        public DelegateCommand EmailTextChangedCommand { get; }

        public DelegateCommand<TextChangedEventArgs> PhoneTextChangedCommand { get; }

        public DelegateCommand AdrTextChangedCommand { get; }

        public DelegateCommand CityTextChangedCommand { get; }

        public DelegateCommand StateTextChangedCommand { get; }

        #endregion

        #endregion

        #region Private/Not Exposed Methods

        #region Handlers

        private async void SubmitBtnClickCommandExecuteAsync()
        {
            var customer = new Customer
            {
                Name = NameBoxText,
                Email = EmailBoxText,
                Phone = PhoneBoxText,
                Address = AdrBoxText,
                City = CityBoxText,
                State = StateBoxText
            };

            var validation = _service.ValidateCustomer(customer);

            if (!validation.isValid)
            {
                var errorMessage = new StringBuilder();
                var errorList = validation.errorList;
                while (!string.IsNullOrEmpty(errorList))
                {
                    var error = errorList.GetUntilOrEmpty(".");
                    errorMessage.Append(error + "\n");
                    errorList = errorList.Replace(error, "");
                }
                MessageBox.Show(errorMessage.ToString(), "Failed to Register", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StaticLogger.LogInfo(GetType(), "Account registration submitted by WPF form for Customer: " +
                customer.Name + " with Phone: " + customer.Phone);
            await _service.CreateAsync(customer);
            MessageBox.Show("Your account has been registered.", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
            if (_callingView != null) _callingView.Stop();
        }

        private void SubmitBtnClickCommandExecute()
        {
            var customer = new Customer
            {
                Name = NameBoxText,
                Email = EmailBoxText,
                Phone = PhoneBoxText,
                Address = AdrBoxText,
                City = CityBoxText,
                State = StateBoxText
            };

            var validation = _service.ValidateCustomer(customer);

            if (!validation.isValid)
            {
                var errorMessage = new StringBuilder();
                var errorList = validation.errorList;
                while (!string.IsNullOrEmpty(errorList))
                {
                    var error = errorList.GetUntilOrEmpty(".");
                    errorMessage.Append(error + "\n");
                    errorList = errorList.Replace(error, "");
                }
                MessageBox.Show(errorMessage.ToString(), "Failed to Register", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StaticLogger.LogInfo(GetType(), "Account registration submitted by WPF form for Customer: " +
                customer.Name + " with Phone: " + customer.Phone);
            _service.Create(customer);
            MessageBox.Show("Your account has been registered.", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
            if (_callingView != null) _callingView.Stop();
        }


        private void NameTextChangedCommandExecute()
        {
            var validation = ValidateName(NameBoxText);

            if (validation.isValid)
            {
                NameCheckVisibility = Visibility.Visible;
                NameErrorVisibility = Visibility.Hidden;
            }
            else
            {
                NameErrorText = validation.errorList.FirstOrDefault();

                NameCheckVisibility = Visibility.Hidden;
                NameErrorVisibility = Visibility.Visible;
            }
        }

        private void EmailTextChangedCommandExecute()
        {
            var validation = ValidateEmail(EmailBoxText);

            if (validation.isValid)
            {
                EmailCheckVisibility = Visibility.Visible;
                EmailErrorVisibility = Visibility.Hidden;
            }
            else
            {
                EmailErrorText = validation.errorList.FirstOrDefault();

                EmailCheckVisibility = Visibility.Hidden;
                EmailErrorVisibility = Visibility.Visible;
            }
        }
     
        private void PhoneTextChangedCommandExecute(TextChangedEventArgs e)
        {
            ControlHelpers.HandlePhoneNumberInput(e);

            var validation = ValidatePhone(PhoneBoxText);
        
            if (validation.isValid)
            {
                PhoneCheckVisibility = Visibility.Visible;
                PhoneErrorVisibility = Visibility.Hidden;
            }
            else
            {
                PhoneErrorText = validation.errorList.FirstOrDefault();

                PhoneCheckVisibility = Visibility.Hidden;
                PhoneErrorVisibility = Visibility.Visible;
            }
        }

        private void AdrTextChangedCommandExecute()
        {
            var validation = ValidateAddress(AdrBoxText);

            if (validation.isValid)
            {
                AdrCheckVisibility = Visibility.Visible;
                AdrErrorVisibility = Visibility.Hidden;
            }
            else
            {
                AdrErrorText = validation.errorList.FirstOrDefault();

                AdrCheckVisibility = Visibility.Hidden;
                AdrErrorVisibility = Visibility.Visible;
            }
        }

        private void CityTextChangedCommandExecute()
        {
            var validation = ValidateCity(CityBoxText);

            if (validation.isValid)
            {
                CityCheckVisibility = Visibility.Visible;
                CityErrorVisibility = Visibility.Hidden;
            }
            else
            {
                CityErrorText = validation.errorList.FirstOrDefault();

                CityCheckVisibility = Visibility.Hidden;
                CityErrorVisibility = Visibility.Visible;
            }
        }

        private void StateTextChangedCommandExecute()
        {
            var validation = ValidateState(StateBoxText);

            if (validation.isValid)
            {
                StateCheckVisibility = Visibility.Visible;
                StateErrorVisibility = Visibility.Hidden;
            }
            else
            {
                StateErrorText = validation.errorList.FirstOrDefault();

                StateCheckVisibility = Visibility.Hidden;
                StateErrorVisibility = Visibility.Visible;
            }
        }

        #endregion

        #region Methods

        private bool IsValidName(string name) => _service.ValidateName(name).isValid;
        private bool IsValidEmail(string email) => _service.ValidateEmail(email).isValid;
        private bool IsValidPhone(string phone) => _service.ValidatePhone(phone).isValid;
        private bool IsValidAddress(string adr) => _service.ValidateAddress(adr).isValid;
        private bool IsValidCity(string city) => _service.ValidateCity(city).isValid;
        private bool IsValidState(string state) => _service.ValidateState(state).isValid;

        private void FinalValidation()
        {
            ValidationErrors["Name"] = ValidateName(NameBoxText).errorList;
            ValidationErrors["Email"] = ValidateEmail(EmailBoxText).errorList;
            ValidationErrors["Phone"] = ValidatePhone(PhoneBoxText).errorList;
            ValidationErrors["Address"] = ValidateAddress(AdrBoxText).errorList;
            ValidationErrors["City"] = ValidateCity(CityBoxText).errorList;
            ValidationErrors["State"] = ValidateState(StateBoxText).errorList;
        }

        public (bool isValid, List<string> errorList) ValidateName(string name)
        {
            var validation = _service.ValidateName(name);
            var errorList = new List<string>();
            var valString = validation.errorList;

            var currErr = "";
            while (!string.IsNullOrEmpty(valString))
            {
                currErr = valString.GetUntilOrEmpty(".");
                valString = valString.Replace(currErr, "");
                errorList.Add(currErr);
            }

            return (validation.isValid, errorList);
        }

        public (bool isValid, List<string> errorList) ValidateAddress(string adr)
        {
            var validation = _service.ValidateAddress(adr);
            var errorList = new List<string>();
            var valString = validation.errorList;

            var currErr = "";
            while (!string.IsNullOrEmpty(valString))
            {
                currErr = valString.GetUntilOrEmpty(".");
                valString = valString.Replace(currErr, "");
                errorList.Add(currErr);
            }

            return (validation.isValid, errorList);
        }
     
        public (bool isValid, List<string> errorList) ValidateEmail(string email)
        {
            var validation = _service.ValidateEmail(email);
            var errorList = new List<string>();
            var valString = validation.errorList;

            var currErr = "";
            while (!string.IsNullOrEmpty(valString))
            {
                currErr = valString.GetUntilOrEmpty(".");
                valString = valString.Replace(currErr, "");
                errorList.Add(currErr);
            }

            return (validation.isValid, errorList);
        }
   
        public (bool isValid, List<string> errorList) ValidatePhone(string phone)
        {
            var validation = _service.ValidatePhone(phone);
            var errorList = new List<string>();
            var valString = validation.errorList;

            var currErr = "";
            while (!string.IsNullOrEmpty(valString))
            {
                currErr = valString.GetUntilOrEmpty(".");
                valString = valString.Replace(currErr, "");
                errorList.Add(currErr);
            }

            return (validation.isValid, errorList);
        }

        public (bool isValid, List<string> errorList) ValidateCity(string city)
        {
            var validation = _service.ValidateCity(city);
            var errorList = new List<string>();
            var valString = validation.errorList;

            var currErr = "";
            while (!string.IsNullOrEmpty(valString))
            {
                currErr = valString.GetUntilOrEmpty(".");
                valString = valString.Replace(currErr, "");
                errorList.Add(currErr);
            }

            return (validation.isValid, errorList);
        }
      
        public (bool isValid, List<string> errorList) ValidateState(string state)
        {
            var validation = _service.ValidateState(state);
            var errorList = new List<string>();
            var valString = validation.errorList;

            var currErr = "";
            while (!string.IsNullOrEmpty(valString))
            {
                currErr = valString.GetUntilOrEmpty(".");
                valString = valString.Replace(currErr, "");
                errorList.Add(currErr);
            }

            return (validation.isValid, errorList);
        }

        #endregion

        #endregion

    }
}
