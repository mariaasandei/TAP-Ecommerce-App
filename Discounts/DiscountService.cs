using EcommerceOrder.Models;

namespace EcommerceOrder.Discounts;

// Strategy Interface
public interface IDiscountCampaign
{
    string Code { get; }
    DiscountResult Apply(List<CartItem> items, decimal subtotal);
}

// Concrete Strategies
public class PercentageDiscountCampaign : IDiscountCampaign
{
    public string Code { get; }
    private readonly decimal _percentage;

    public PercentageDiscountCampaign(string code, decimal percentage)
    {
        Code = code;
        _percentage = percentage;
    }

    public DiscountResult Apply(List<CartItem> items, decimal subtotal) =>
        new() { Success = true, DiscountAmount = Math.Round(subtotal * _percentage / 100, 2), Message = $"{_percentage}% off applied" };
}

public class FixedAmountDiscountCampaign : IDiscountCampaign
{
    public string Code { get; }
    private readonly decimal _amount;

    public FixedAmountDiscountCampaign(string code, decimal amount)
    {
        Code = code;
        _amount = amount;
    }

    public DiscountResult Apply(List<CartItem> items, decimal subtotal)
    {
        var discount = Math.Min(_amount, subtotal);
        return new() { Success = true, DiscountAmount = discount, Message = $"${discount} off applied" };
    }
}

public class BuyXGetYDiscountCampaign : IDiscountCampaign
{
    public string Code => "BUYXGETY";
    private readonly int _buyX;
    private readonly int _getY;

    public BuyXGetYDiscountCampaign(int buyX, int getY)
    {
        _buyX = buyX;
        _getY = getY;
    }

    public DiscountResult Apply(List<CartItem> items, decimal subtotal)
    {
        int totalQty = items.Sum(i => i.Quantity);
        if (totalQty < _buyX + _getY)
            return new() { Success = false, Message = $"Need at least {_buyX + _getY} items for Buy {_buyX} Get {_getY}." };

        var cheapest = items
            .OrderBy(i => i.Product.Price)
            .Take(_getY)
            .Sum(i => i.Product.Price * _getY);

        return new() { Success = true, DiscountAmount = cheapest, Message = $"Buy {_buyX} Get {_getY} applied" };
    }
}

// Discount Service
public interface IDiscountService
{
    void RegisterCampaign(IDiscountCampaign campaign);
    DiscountResult ApplyDiscount(string code, List<CartItem> items, decimal subtotal);
}

public class DiscountService : IDiscountService
{
    private readonly Dictionary<string, IDiscountCampaign> _campaigns = new(StringComparer.OrdinalIgnoreCase);

    public void RegisterCampaign(IDiscountCampaign campaign) =>
        _campaigns[campaign.Code] = campaign;

    public DiscountResult ApplyDiscount(string code, List<CartItem> items, decimal subtotal)
    {
        if (!_campaigns.TryGetValue(code, out var campaign))
            return new() { Success = false, Message = "Invalid discount code." };

        return campaign.Apply(items, subtotal);
    }
}
