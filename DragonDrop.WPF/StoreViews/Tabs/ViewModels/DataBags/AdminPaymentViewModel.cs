using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using Prism.Mvvm;
using System;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class AdminPaymentViewModel : BindableBase
    {
        #region Fields

        private IPaymentMethodRepository _methRepo;
        private ICustomerDataService _custServ;
        private IPaymentDataService _service;

        private Payment _payment;

        #endregion

        public AdminPaymentViewModel(IPaymentMethodRepository methRepo, 
            ICustomerDataService custServ, IPaymentDataService service, Payment payment)
        {
            _payment = payment;

            _methRepo = methRepo;
            _custServ = custServ;
            _service = service;


        }

        #region Properties

        #region Data

        public int PaymentId
        {
            get => _payment.PaymentId;
            set { }
        }

        public int CustomerId
        {
            get => _payment.CustomerId;
            set { }
        }

        public string PaymentMethod
        {
            get => _methRepo.GetMethodName(_payment.PaymentMethodId);
            set { }
        }

        public DateTime Date
        {
            get => _payment.Date.GetValueOrDefault();
            set { }
        }

        public decimal Amount
        {
            get => _payment.Amount.GetValueOrDefault();
            set { }
        }

        public Customer Customer
        {
            get => _custServ.Get(_payment.CustomerId);
            set { }
        }

        #endregion

        #region ViewModel Props

        public Visibility CustomerPopVisibility
        {
            get => Visibility.Collapsed;
            set { }
        }

        #endregion

        #endregion
    }
}
