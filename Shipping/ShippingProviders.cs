using EcommerceOrder.Models;

namespace EcommerceOrder.Shipping;

// Strategy Interface
public interface IShippingProvider
{
    string Name { get; }
    decimal GetShippingCost(List<CartItem> items);
    string GenerateTrackingNumber();
}

// Concrete Strategies
public class FedExShippingProvider : IShippingProvider
{
    public string Name => "FedEx";

    public decimal GetShippingCost(List<CartItem> items)
    {
        int totalItems = items.Sum(i => i.Quantity);
        return 5.99m + (totalItems - 1) * 1.50m;
    }

    public string GenerateTrackingNumber() =>
        "FX" + DateTime.Now.Ticks.ToString()[^10..];
}

public class DHLShippingProvider : IShippingProvider
{
    public string Name => "DHL";

    public decimal GetShippingCost(List<CartItem> items)
    {
        decimal subtotal = items.Sum(i => i.Subtotal);
        return subtotal > 100 ? 0m : 7.99m; // Free over $100
    }

    public string GenerateTrackingNumber() =>
        "DHL" + Guid.NewGuid().ToString("N")[..10].ToUpper();
}

public class LocalCourierShippingProvider : IShippingProvider
{
    public string Name => "Local Courier";

    public decimal GetShippingCost(List<CartItem> items) => 3.99m; // Flat rate

    public string GenerateTrackingNumber() =>
        "LC" + new Random().Next(100000, 999999);
}
