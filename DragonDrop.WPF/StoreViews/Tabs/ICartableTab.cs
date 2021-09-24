using DragonDrop.WPF.StoreViews.ViewModels;

namespace DragonDrop.WPF.StoreViews.Tabs
{
    public interface ICartableTab
    {
        StoreViewModel ParentViewModel { get; set; }
    }
}
