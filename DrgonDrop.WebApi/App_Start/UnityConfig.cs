using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Interfaces;
using DragonDrop.DAL.Implementation;
using System;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace DrgonDrop.WebApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerDataService, CustomerDataService>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IOrderDataService, OrderDataService>();
            container.RegisterType<IOrderItemRepository, OrderItemRepository>();
            container.RegisterType<IOrderItemDataService, OrderItemDataService>();
            container.RegisterType<IPaymentRepository, PaymentRepository>();
            container.RegisterType<IPaymentDataService, PaymentDataService>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IProductDataService, ProductDataService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}