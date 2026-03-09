using EcommerceOrder.Models;
using EcommerceOrder.Shipping;

namespace EcommerceOrder.Orders;

public interface IOrderService
{
    Order CreateOrder(
        List<CartItem> items,
        string paymentMethod,
        string shippingProvider,
        decimal subtotal,
        decimal discountAmount,
        decimal shippingCost,
        decimal totalAmount);

    List<Order> GetAllOrders();
    Order? GetOrderById(string orderId);
}

public class OrderService : IOrderService
{
    private readonly List<Order> _orders = new();

    // Inject shipping providers to generate tracking numbers
    private readonly Dictionary<string, IShippingProvider> _shippingProviders;

    public OrderService()
    {
        _shippingProviders = new Dictionary<string, IShippingProvider>(StringComparer.OrdinalIgnoreCase)
        {
            ["FedEx"]         = new EcommerceOrder.Shipping.FedExShippingProvider(),
            ["DHL"]           = new EcommerceOrder.Shipping.DHLShippingProvider(),
            ["Local Courier"] = new EcommerceOrder.Shipping.LocalCourierShippingProvider(),
        };
    }

    public Order CreateOrder(
        List<CartItem> items,
        string paymentMethod,
        string shippingProvider,
        decimal subtotal,
        decimal discountAmount,
        decimal shippingCost,
        decimal totalAmount)
    {
        string tracking = _shippingProviders.TryGetValue(shippingProvider, out var provider)
            ? provider.GenerateTrackingNumber()
            : "TRK" + Guid.NewGuid().ToString("N")[..8].ToUpper();

        var order = new Order
        {
            Items = items.Select(i => new CartItem { Product = i.Product, Quantity = i.Quantity }).ToList(),
            PaymentMethod = paymentMethod,
            ShippingProvider = shippingProvider,
            TrackingNumber = tracking,
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            ShippingCost = shippingCost,
            TotalAmount = totalAmount,
            Status = "Processing",
            CreatedAt = DateTime.Now
        };

        _orders.Add(order);
        return order;
    }

    public List<Order> GetAllOrders() => _orders.ToList();
    public Order? GetOrderById(string orderId) =>
        _orders.FirstOrDefault(o => o.OrderId.Equals(orderId, StringComparison.OrdinalIgnoreCase));
}
