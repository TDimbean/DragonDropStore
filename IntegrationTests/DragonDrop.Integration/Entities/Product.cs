namespace DragonDrop.Integration.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public decimal? UnitPrice { get; set; }
        public string BarCode { get; set; }
        public string Manufacturer
        {
            get => BarCode == null || BarCode.Length<6 ? null : BarCode.Substring(0, 6);
        }
    }
}
