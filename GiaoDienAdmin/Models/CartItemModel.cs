namespace GiaoDienAdmin.Models
{
    public class CartItemModel
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public decimal Total { get { return Quantity * Price; } }
        public CartItemModel() { }

        public CartItemModel(Product product)
        {
            ProductId = product.Id;
            ProductName = product.Name;
            Price = product.Price;
            Quantity = 1;
            Image = product.Img;
        }
    }
}
