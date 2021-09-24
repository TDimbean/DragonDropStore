using DragonDrop.DAL.Entities;
using Prism.Mvvm;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class AddressPopViewModel : BindableBase
    {
        private Customer _cust;

        public AddressPopViewModel(Customer cust) => _cust=cust;

        public string Title
        {
            get => _cust.Name + "'s Address:";
            set { }
        }

        public string Address { get => _cust.Address; set { } }
        public string City { get => _cust.City; set { } }
        public string State { get => _cust.State; set { } }
        public string Email { get => _cust.Email; set { } }
    }
}
