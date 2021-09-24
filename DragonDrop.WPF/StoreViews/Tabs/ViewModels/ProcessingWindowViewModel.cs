using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class ProcessingWindowViewModel : BindableBase
    {
        public ProcessingWindowViewModel(IOrderDataService ordService)
        {
            OrderList = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]) ?
                ordService.GetAllUnprocessedAsync().Result.ToList() :
                ordService.GetAllUnprocessed().ToList();
        }

        public List<Order> OrderList { get; set; }
    }
}
