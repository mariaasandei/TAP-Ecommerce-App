using EcommerceOrder.Models;

namespace EcommerceOrder.Cart;

public interface IShoppingCartService
{
    void AddItem(Product product, int quantity);
    void RemoveItem(int productId);
    void UpdateQuantity(int productId, int quantity);
    List<CartItem> GetItems();
    decimal GetTotal();
    void Clear();
}

public class ShoppingCartService : IShoppingCartService
{
    private readonly List<CartItem> _items = new();

    public void AddItem(Product product, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);
        if (existing != null)
            existing.Quantity += quantity;
        else
            _items.Add(new CartItem { Product = product, Quantity = quantity });
    }

    public void RemoveItem(int productId) =>
        _items.RemoveAll(i => i.Product.Id == productId);

    public void UpdateQuantity(int productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.Product.Id == productId);
        if (item != null)
        {
            if (quantity <= 0) RemoveItem(productId);
            else item.Quantity = quantity;
        }
    }

    public List<CartItem> GetItems() => _items.ToList();
    public decimal GetTotal() => _items.Sum(i => i.Subtotal);
    public void Clear() => _items.Clear();
}
