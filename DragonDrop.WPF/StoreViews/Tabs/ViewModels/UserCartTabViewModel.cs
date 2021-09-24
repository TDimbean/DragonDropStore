using DragonDrop.BLL.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using Prism.Mvvm;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    class UserCartTabViewModel : BindableBase
    {
        private string _subTotal = "10.345";
        private float _tax = 1.655f;
        private int _total = 12;

        public string Subtotal
        {
            get => _subTotal;
            set => SetProperty(ref _subTotal, value);
        }

        public float Tax
        {
            get => _subTotal==null? 0f : (float)(decimal.Parse(_subTotal)/(int)App.Current.Resources["Tax"]);
            set => SetProperty(ref _tax, value);
        }

        public int Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

    }
}
