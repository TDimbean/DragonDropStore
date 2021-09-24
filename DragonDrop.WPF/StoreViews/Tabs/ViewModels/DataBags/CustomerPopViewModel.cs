using DragonDrop.DAL.Entities;
using Prism.Mvvm;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class CustomerPopViewModel : BindableBase
    {
        private Customer _cust;

        public CustomerPopViewModel(Customer cust) => _cust = cust;

        public int CustomerId { get => _cust.CustomerId; set { } }
        public string Name { get => _cust.Name; set { } }
        public string Email { get => _cust.Email; set { } }
        public string Phone { get => _cust.Phone; set { } }
        public string Address { get => _cust.Address; set { } }
        public string City { get => _cust.City; set { } }
        public string State { get => _cust.State; set { } }
    }
}
