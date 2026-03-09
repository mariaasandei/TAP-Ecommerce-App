using EcommerceOrder.Models;

namespace EcommerceOrder.Payment;

// Strategy Interface
public interface IPaymentProvider
{
    string Name { get; }
    PaymentResult ProcessPayment(decimal amount);
}

// Concrete Strategies
public class CreditCardPaymentProvider : IPaymentProvider
{
    public string Name => "Credit Card";

    public PaymentResult ProcessPayment(decimal amount)
    {
        // Simulate payment processing
        Console.WriteLine("  [Credit Card] Connecting to payment gateway...");
        Thread.Sleep(500);
        return new PaymentResult
        {
            Success = true,
            Message = "Payment authorized",
            TransactionId = "CC-" + Guid.NewGuid().ToString("N")[..8].ToUpper()
        };
    }
}

public class PayPalPaymentProvider : IPaymentProvider
{
    public string Name => "PayPal";

    public PaymentResult ProcessPayment(decimal amount)
    {
        Console.WriteLine("  [PayPal] Redirecting to PayPal...");
        Thread.Sleep(500);
        return new PaymentResult
        {
            Success = true,
            Message = "PayPal payment confirmed",
            TransactionId = "PP-" + Guid.NewGuid().ToString("N")[..8].ToUpper()
        };
    }
}

public class CryptoPaymentProvider : IPaymentProvider
{
    public string Name => "Cryptocurrency";

    public PaymentResult ProcessPayment(decimal amount)
    {
        Console.WriteLine("  [Crypto] Broadcasting transaction to blockchain...");
        Thread.Sleep(800);
        return new PaymentResult
        {
            Success = true,
            Message = "Blockchain transaction confirmed",
            TransactionId = "BTC-" + Guid.NewGuid().ToString("N")[..8].ToUpper()
        };
    }
}
