using EcommerceOrder.Catalog;
using EcommerceOrder.Cart;
using EcommerceOrder.Orders;
using EcommerceOrder.Payment;
using EcommerceOrder.Shipping;
using EcommerceOrder.Discounts;
using EcommerceOrder.Models;
using EcommerceOrder.Services;

Console.Title = "E-Commerce Order System";
Console.OutputEncoding = System.Text.Encoding.UTF8;

// --- Bootstrap Services ---
IProductCatalogService catalog = new ProductCatalogService();
IShoppingCartService cart = new ShoppingCartService();
IDiscountService discountService = new DiscountService();
IOrderService orderService = new OrderService();

// Register Payment Providers
IPaymentProvider[] paymentProviders = new IPaymentProvider[]
{
    new CreditCardPaymentProvider(),
    new PayPalPaymentProvider(),
    new CryptoPaymentProvider()
};

// Register Shipping Couriers
IShippingProvider[] shippingProviders = new IShippingProvider[]
{
    new FedExShippingProvider(),
    new DHLShippingProvider(),
    new LocalCourierShippingProvider()
};

// Register Discount Campaigns
discountService.RegisterCampaign(new PercentageDiscountCampaign("SUMMER20", 20));
discountService.RegisterCampaign(new FixedAmountDiscountCampaign("SAVE10", 10));
discountService.RegisterCampaign(new BuyXGetYDiscountCampaign(3, 1));

// Seed catalog
catalog.SeedSampleProducts();

// --- Main Loop ---
bool running = true;
while (running)
{
    PrintHeader();
    PrintMainMenu();
    string choice = Console.ReadLine()?.Trim() ?? "";

    switch (choice)
    {
        case "1": BrowseCatalog(catalog); break;
        case "2": ManageCart(cart, catalog); break;
        case "3": ViewCart(cart); break;
        case "4": Checkout(cart, discountService, paymentProviders, shippingProviders, orderService); break;
        case "5": ViewOrders(orderService); break;
        case "0": running = false; break;
        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  Invalid option. Press any key...");
            Console.ResetColor();
            Console.ReadKey();
            break;
    }
}

Console.Clear();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n  Thank you for using E-Commerce Order System. Goodbye!\n");
Console.ResetColor();

// ==============================
//        MENU HANDLERS
// ==============================

void PrintHeader()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.DarkMagenta;
    Console.WriteLine("╔══════════════════════════════════════════╗");
    Console.WriteLine("║       🛒  E-COMMERCE ORDER SYSTEM         ║");
    Console.WriteLine("╚══════════════════════════════════════════╝");
    Console.ResetColor();
}

void PrintMainMenu()
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("  MAIN MENU");
    Console.ResetColor();
    Console.WriteLine("  [1] Browse Product Catalog");
    Console.WriteLine("  [2] Add Item to Cart");
    Console.WriteLine("  [3] View Shopping Cart");
    Console.WriteLine("  [4] Checkout");
    Console.WriteLine("  [5] View My Orders");
    Console.WriteLine("  [0] Exit");
    Console.WriteLine();
    Console.Write("  Select option: ");
}

void BrowseCatalog(IProductCatalogService catalog)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("  ── PRODUCT CATALOG ─────────────────────────");
    Console.ResetColor();
    var products = catalog.GetAllProducts();
    Console.WriteLine($"  {"ID",-4} {"Name",-25} {"Category",-15} {"Price",8}");
    Console.WriteLine("  " + new string('─', 56));
    foreach (var p in products)
    {
        Console.WriteLine($"  {p.Id,-4} {p.Name,-25} {p.Category,-15} ${p.Price,7:F2}");
    }
    Console.WriteLine();
    Console.WriteLine("  Press any key to return...");
    Console.ReadKey();
}

void ManageCart(IShoppingCartService cart, IProductCatalogService catalog)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("  ── ADD TO CART ──────────────────────────────");
    Console.ResetColor();
    var products = catalog.GetAllProducts();
    Console.WriteLine($"  {"ID",-4} {"Name",-25} {"Price",8}");
    Console.WriteLine("  " + new string('─', 40));
    foreach (var p in products)
        Console.WriteLine($"  {p.Id,-4} {p.Name,-25} ${p.Price,7:F2}");
    Console.WriteLine();
    Console.Write("  Enter Product ID: ");
    if (!int.TryParse(Console.ReadLine(), out int productId))
    {
        PrintError("Invalid ID."); return;
    }
    var product = catalog.GetProductById(productId);
    if (product == null) { PrintError("Product not found."); return; }

    Console.Write("  Enter Quantity: ");
    if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 1)
    {
        PrintError("Invalid quantity."); return;
    }
    cart.AddItem(product, qty);
    PrintSuccess($"  ✔ Added {qty}x '{product.Name}' to cart.");
    Console.ReadKey();
}

void ViewCart(IShoppingCartService cart)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("  ── SHOPPING CART ────────────────────────────");
    Console.ResetColor();
    var items = cart.GetItems();
    if (items.Count == 0)
    {
        Console.WriteLine("  Your cart is empty.");
        Console.ReadKey(); return;
    }
    Console.WriteLine($"  {"Product",-25} {"Qty",5} {"Unit Price",12} {"Subtotal",12}");
    Console.WriteLine("  " + new string('─', 58));
    foreach (var item in items)
    {
        Console.WriteLine($"  {item.Product.Name,-25} {item.Quantity,5} ${item.Product.Price,11:F2} ${item.Subtotal,11:F2}");
    }
    Console.WriteLine("  " + new string('─', 58));
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"  {"TOTAL",-43} ${cart.GetTotal(),11:F2}");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("  Press any key to return...");
    Console.ReadKey();
}

void Checkout(
    IShoppingCartService cart,
    IDiscountService discountService,
    IPaymentProvider[] paymentProviders,
    IShippingProvider[] shippingProviders,
    IOrderService orderService)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("  ── CHECKOUT ─────────────────────────────────");
    Console.ResetColor();

    var items = cart.GetItems();
    if (items.Count == 0) { PrintError("Your cart is empty."); return; }

    decimal subtotal = cart.GetTotal();
    Console.WriteLine($"  Subtotal: ${subtotal:F2}");

    // Discount
    Console.Write("\n  Enter discount code (or press Enter to skip): ");
    string code = Console.ReadLine()?.Trim() ?? "";
    decimal discountAmount = 0;
    if (!string.IsNullOrEmpty(code))
    {
        var result = discountService.ApplyDiscount(code, items, subtotal);
        if (result.Success)
        {
            discountAmount = result.DiscountAmount;
            PrintSuccess($"  ✔ Discount applied: -${discountAmount:F2}");
        }
        else
        {
            PrintError($"  ✘ {result.Message}");
        }
    }

    // Shipping
    Console.WriteLine("\n  Select Shipping Provider:");
    for (int i = 0; i < shippingProviders.Length; i++)
        Console.WriteLine($"  [{i + 1}] {shippingProviders[i].Name} — ${shippingProviders[i].GetShippingCost(items):F2}");
    Console.Write("  Choice: ");
    if (!int.TryParse(Console.ReadLine(), out int shipChoice) || shipChoice < 1 || shipChoice > shippingProviders.Length)
    {
        PrintError("Invalid choice."); return;
    }
    var selectedShipping = shippingProviders[shipChoice - 1];
    decimal shippingCost = selectedShipping.GetShippingCost(items);

    // Payment
    Console.WriteLine("\n  Select Payment Method:");
    for (int i = 0; i < paymentProviders.Length; i++)
        Console.WriteLine($"  [{i + 1}] {paymentProviders[i].Name}");
    Console.Write("  Choice: ");
    if (!int.TryParse(Console.ReadLine(), out int payChoice) || payChoice < 1 || payChoice > paymentProviders.Length)
    {
        PrintError("Invalid choice."); return;
    }
    var selectedPayment = paymentProviders[payChoice - 1];

    decimal total = subtotal - discountAmount + shippingCost;

    Console.WriteLine();
    Console.WriteLine("  ─── ORDER SUMMARY ───────────────────────────");
    Console.WriteLine($"  Subtotal:   ${subtotal:F2}");
    Console.WriteLine($"  Discount:  -${discountAmount:F2}");
    Console.WriteLine($"  Shipping:  +${shippingCost:F2}");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"  TOTAL:      ${total:F2}");
    Console.ResetColor();
    Console.WriteLine($"  Payment:    {selectedPayment.Name}");
    Console.WriteLine($"  Courier:    {selectedShipping.Name}");

    Console.Write("\n  Confirm order? (y/n): ");
    if (Console.ReadLine()?.ToLower() != "y") { Console.WriteLine("  Order cancelled."); Console.ReadKey(); return; }

    var paymentResult = selectedPayment.ProcessPayment(total);
    if (!paymentResult.Success)
    {
        PrintError($"  ✘ Payment failed: {paymentResult.Message}");
        Console.ReadKey(); return;
    }

    var order = orderService.CreateOrder(items, selectedPayment.Name, selectedShipping.Name, subtotal, discountAmount, shippingCost, total);
    cart.Clear();

    PrintSuccess($"\n  ✔ Order #{order.OrderId} placed successfully!");
    Console.WriteLine($"  Tracking: {order.TrackingNumber}");
    Console.ReadKey();
}

void ViewOrders(IOrderService orderService)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("  ── MY ORDERS ────────────────────────────────");
    Console.ResetColor();
    var orders = orderService.GetAllOrders();
    if (orders.Count == 0) { Console.WriteLine("  No orders yet."); Console.ReadKey(); return; }

    foreach (var o in orders)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  Order #{o.OrderId} | {o.CreatedAt:yyyy-MM-dd HH:mm} | {o.Status}");
        Console.ResetColor();
        Console.WriteLine($"  Payment: {o.PaymentMethod} | Courier: {o.ShippingProvider} | Tracking: {o.TrackingNumber}");
        foreach (var item in o.Items)
            Console.WriteLine($"    - {item.Product.Name} x{item.Quantity}  ${item.Subtotal:F2}");
        Console.WriteLine($"  Discount: -${o.DiscountAmount:F2} | Shipping: +${o.ShippingCost:F2} | TOTAL: ${o.TotalAmount:F2}");
    }
    Console.WriteLine();
    Console.WriteLine("  Press any key to return...");
    Console.ReadKey();
}

void PrintError(string msg) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(msg); Console.ResetColor(); Console.ReadKey(); }
void PrintSuccess(string msg) { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(msg); Console.ResetColor(); }
