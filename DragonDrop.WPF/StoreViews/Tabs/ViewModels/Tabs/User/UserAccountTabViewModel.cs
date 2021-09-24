using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class UserAccountTabViewModel : BindableBase
    {
        private Customer _cust;
        private ICustomerDataService _service;
        private UserAccountTab _tab;

        private string _userName;

        public UserAccountTabViewModel(Customer cust, ICustomerDataService service, UserAccountTab tab)
        {
            _tab = tab;
            _service = service;
            _cust = cust;
            _userName = _cust.Name;

            Fields = new ObservableCollection<InputField>
            {
                new InputField(service, "Name", "person", cust.Name),
                new InputField(service, "E-mail", "mail", cust.Email),
                new InputField(service, "Phone", "phoneReceiver", cust.Phone),
                new InputField(service, "Address", "mapMarker", cust.Address),
                new InputField(service, "City", "cityBuildings", cust.City),
                new InputField(service, "State", "state", cust.State)
            };

            SubmitClickCommand = new DelegateCommand(SubmitClickCommandExecute);
            SubmitKeyUpCommand = new DelegateCommand<KeyEventArgs>(SubmitKeyUpCommandExecute);
        }

        #region Properties

        public string UserName
        {
            get => GetAccTabHeader(_userName);
            set => SetProperty(ref _userName, value);
        }

        public ObservableCollection<InputField> Fields { get; }

        public DelegateCommand<KeyEventArgs> SubmitKeyUpCommand { get; }

        public DelegateCommand SubmitClickCommand { get; }

        #endregion

        #region Methods

        #region Commands

        private void SubmitClickCommandExecute()
        {
            var updCustomer = new Customer
            {
                CustomerId = _cust.CustomerId,
                Name = Fields[0].Text,
                Email = Fields[1].Text,
                Phone = Fields[2].Text,
                Address = Fields[3].Text,
                City = Fields[4].Text,
                State = Fields[5].Text
            };

            var validation = _service.ValidateCustomer(updCustomer);

            if (!validation.isValid)
            {
                _tab.ValidationMessage(validation.errorList);
                return;
            }


            StaticLogger.LogInfo(GetType(), "Customer with ID: " + _cust.CustomerId + 
                " hs requested an update to their account from the WPF appication.");
            _service.Update(updCustomer);

            foreach (var field in Fields) field.OgText = field.Text;

            _cust = updCustomer;

            UserName = _cust.Name;
        }

        private void SubmitKeyUpCommandExecute(KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SubmitClickCommandExecute();
        }

        #endregion

        #region Others

        //Needs improvement
        private string GetAccTabHeader(string userName)
        {
            if (userName.Length < 11) return userName;

            var firstName = userName.GetUntilOrEmpty(" ");

            if (!string.IsNullOrEmpty(firstName)&&firstName.Length < 10) return firstName + " " + userName.Replace(firstName, string.Empty).Substring(0, 1) + ".";

            return userName.Substring(0, 9) + "...";
        }

        #endregion

        #endregion
    }
}
