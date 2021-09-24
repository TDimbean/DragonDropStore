using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace DragonDrop.DAL.Entities
{
    public class TempResourceStore : DragonDrop_DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderStatus>()
              .Property(s => s.OrderStatusId)
              .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Category>()
              .Property(c => c.CategoryId)
              .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<PaymentMethod>()
              .Property(p => p.PaymentMethodId)
              .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<ShippingMethod>()
              .Property(m => m.ShippingMethodId)
              .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            base.OnModelCreating(modelBuilder);
        }
    }
}
