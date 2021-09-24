using DragonDrop.DAL.Entities;
using System.Collections.Generic;

namespace DragonDrop.WPF.StoreViews.ViewModels
{
    public class Cart
    {
        public List<(Product prod, int qty)> ProdTupleList;
    }
}
