namespace EcommerceOrder.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Description { get; set; } = "";
}

public class CartItem
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Subtotal => Product.Price * Quantity;
}

public class Order
{
    public string OrderId { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public List<CartItem> Items { get; set; } = new();
    public string PaymentMethod { get; set; } = "";
    public string ShippingProvider { get; set; } = "";
    public string TrackingNumber { get; set; } = "";
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Processing";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public string TransactionId { get; set; } = "";
}

public class DiscountResult
{
    public bool Success { get; set; }
    public decimal DiscountAmount { get; set; }
    public string Message { get; set; } = "";
}
