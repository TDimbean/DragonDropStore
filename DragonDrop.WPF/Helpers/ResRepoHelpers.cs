using DragonDrop.DAL.Implementation.Resources;
using Unity;

namespace DragonDrop.WPF.Helpers
{
    public static class ResRepoHelpers
    {
        public static IResourceRepo GetRepo(int repoType)
        {
            if (repoType < 0 || repoType > 3) return null;
            var container = new UnityContainer();
            switch (repoType)
            {
                case 1: return container.Resolve<PaymentMethodRepository>();
                case 2: return container.Resolve<ShippingMethodRepository>();
                case 3: return container.Resolve<CategoryRepository>();
                default:return container.Resolve<OrderStatusRepository>();
            }
        }
    }
}
