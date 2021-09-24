using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.DTOs;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    //TODO simplify all the repo and name stuff. use generics,  make a combined claass, n/ething to clean this mess up

    public class EditDefaultsViewModel : BindableBase
    {
        #region Fields

        private int _repoIndex;

        private string _repoTitle;

        private Dictionary<int,string> _repoTitles =
            new Dictionary<int, string>
            {
                {0, "Order Statuses" },
                {1, "Payment Methods" },
                {2, "Shipping Methods" },
                {3, "Product Categories" }
            };

        private List<DefaultValueDto> _valuesList;

        #endregion

        public EditDefaultsViewModel(int startingIndex = 0)
        {
            _repoIndex = startingIndex;

            CycleLeftCommand = new DelegateCommand(CycleLeftCommandExecute);
            CycleRightCommand = new DelegateCommand(CycleRightCommandExecute);

            UpdateRepoTitle();
            UpdateValues();
        }

        #region Properties

        public List<DefaultValueDto> ValuesList
        {
            get => _valuesList;
            set => SetProperty(ref _valuesList, value);
        }

        public string RepoTitle
        {
            get => _repoTitle;
            set => SetProperty(ref _repoTitle, value);
        }

        public int RepoType
        {
            get => _repoIndex;
        }

        public DelegateCommand CycleLeftCommand { get; }
        public DelegateCommand CycleRightCommand { get; }

        #endregion

        private void CycleLeftCommandExecute() => Cycle();
        private void CycleRightCommandExecute() => Cycle(true);

        private void Cycle(bool toRight = false)
        {
            _repoIndex += toRight ? 1 : -1;
            _repoIndex = _repoIndex < 0 ? 3 : _repoIndex > 3 ? 0 : _repoIndex;
            UpdateRepoTitle();
            UpdateValues();
        }
        private void UpdateValues()
        {
            var container = new UnityContainer();

            switch (_repoIndex)
            {
                case 1:
                    ValuesList = ValuesListFromRepoDump(container.Resolve<PaymentMethodRepository>().GetAll().ToList());
                    break;
                case 2:
                    ValuesList = ValuesListFromRepoDump(container.Resolve<ShippingMethodRepository>().GetAll().ToList());
                    break;
                case 3:
                    ValuesList = ValuesListFromRepoDump(container.Resolve<CategoryRepository>().GetAll().ToList());
                    break;
                default:
                    ValuesList = ValuesListFromRepoDump(container.Resolve<OrderStatusRepository>().GetAll().ToList());
                    break;
            }
        }

        private void UpdateRepoTitle()
        {
            var value = string.Empty;
            var realKey = _repoTitles.TryGetValue(_repoIndex, out value);
            if (!realKey)
            {
                StaticLogger.LogError(GetType(), "Something went wrong when trying to display the Default Resource Repo Values for" +
                    " editing in the WPF admin form. A key could not be found.");
                return;
            }
            RepoTitle = value;
        }

        private List<DefaultValueDto> ValuesListFromRepoDump(object dump)
        {
            var result = new List<DefaultValueDto>();

            var statList = dump as List<OrderStatus>;
            var payList = dump as List<PaymentMethod>;
            var shipList = dump as List<ShippingMethod>;
            var catList = dump as List<Category>;
                
            if (statList!=null)
                foreach (var item in statList)
                    result.Add(new DefaultValueDto
                    {
                        ID = item.OrderStatusId,
                        Name = item.Name
                    });

            if (payList != null)
                foreach (var item in payList)
                    result.Add(new DefaultValueDto
                    {
                        ID = item.PaymentMethodId,
                        Name = item.Name
                    });


            if (shipList != null)
                foreach (var item in shipList)
                    result.Add(new DefaultValueDto
                    {
                        ID = item.ShippingMethodId,
                        Name = item.Name
                    });


            if (catList != null)
                foreach (var item in catList)
                    result.Add(new DefaultValueDto
                    {
                        ID = item.CategoryId,
                        Name = item.Name
                    });

            return result;
        }
    }
}
