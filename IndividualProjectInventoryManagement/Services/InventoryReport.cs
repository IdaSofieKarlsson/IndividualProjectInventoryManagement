using System.Globalization;
using System.Text;
using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;

namespace IndividualProjectInventoryManagement.Services
{
    // The totals for one category in the report
    public record CategorySummary(ProductCategory Category, int Products, int Units, decimal PurchaseValue, decimal SalesValue);

    // ------------------------------------------------------------------
    // InventoryReport - a snapshot of the stock at one moment
    // ------------------------------------------------------------------
    // The report is calculated ONCE in the constructor and then never changes,
    // like a printed report. Later changes to the inventory do not affect it.
    //
    // It only holds numbers and lists. ToText() turns them into readable text
    // but does not print anything, so the menu decides where the text goes
    // (the screen now, and a .txt file later).
    // ------------------------------------------------------------------
    public class InventoryReport
    {
        public DateTime GeneratedAt { get; }

        // Number of different products (rows)
        public int ProductCount { get; }

        // Number of pieces in total
        public int TotalUnits { get; }

        // What the stock cost the shop (pieces x purchase price)
        public decimal TotalPurchaseValue { get; }

        // What the stock is worth when sold (pieces x selling price)
        public decimal TotalSalesValue { get; }

        public IReadOnlyList<CategorySummary> Categories { get; }
        public IReadOnlyList<Product> LowStockProducts { get; }
        public IReadOnlyList<Product> OutOfStockProducts { get; }

        public InventoryReport(IEnumerable<Product> products, DateTime generatedAt)
        {
            // Make our own copy, so the list cannot change under us
            List<Product> list = products.ToList();

            GeneratedAt = generatedAt;
            ProductCount = list.Count;
            TotalUnits = list.Sum(product => product.Quantity);
            TotalPurchaseValue = list.Sum(product => product.StockValue);
            TotalSalesValue = list.Sum(product => product.SalesValue);

            // One summary per category, also for categories without any
            // products (they show as zeros, so the report always looks the same)
            Categories = Enum.GetValues<ProductCategory>()
                .Select(category =>
                {
                    List<Product> inCategory = list.Where(product => product.Category == category).ToList();

                    return new CategorySummary(
                        category,
                        inCategory.Count,
                        inCategory.Sum(product => product.Quantity),
                        inCategory.Sum(product => product.StockValue),
                        inCategory.Sum(product => product.SalesValue));
                })
                .ToList();

            LowStockProducts = list.Where(product => product.GetStockStatus() == StockStatus.LowStock).ToList();
            OutOfStockProducts = list.Where(product => product.GetStockStatus() == StockStatus.OutOfStock).ToList();
        }

        // The whole report as text
        // Pass a converter to also show the total in the chosen currency
        public string ToText(CurrencyConverter? converter = null)
        {
            // StringBuilder collects text efficiently. Appending to a normal
            // string many times creates a new string each time.
            StringBuilder text = new StringBuilder();
            string line = new string('-', 72);

            text.AppendLine("INVENTORY REPORT");
            text.AppendLine($"Generated: {GeneratedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)}");
            text.AppendLine(line);
            text.AppendLine($"Products: {ProductCount}    Units in stock: {TotalUnits}");
            text.AppendLine($"Stock value at purchase price: {FormatMoney(TotalPurchaseValue)}{InCurrency(converter, TotalPurchaseValue)}");
            text.AppendLine($"Stock value at selling price:  {FormatMoney(TotalSalesValue)}{InCurrency(converter, TotalSalesValue)}");

            if (converter != null && converter.Selected != Currency.SEK)
            {
                text.AppendLine($"(Converted with {converter.RateInfo})");
            }

            text.AppendLine();

            text.AppendLine("VALUE BY CATEGORY");
            // {x,-14} = left-aligned in 14 characters, {x,10} = right-aligned in 10
            text.AppendLine($"  {"Category",-14}{"Products",10}{"Units",8}{"Purchase value",18}{"Selling value",18}");

            foreach (CategorySummary summary in Categories)
            {
                text.AppendLine(
                    $"  {summary.Category.ToDisplayText(),-14}{summary.Products,10}{summary.Units,8}{FormatMoney(summary.PurchaseValue),18}{FormatMoney(summary.SalesValue),18}");
            }

            text.AppendLine();
            text.AppendLine($"LOW STOCK ({LowStockProducts.Count})");

            if (LowStockProducts.Count == 0)
            {
                text.AppendLine("  None");
            }

            foreach (Product product in LowStockProducts)
            {
                text.AppendLine($"  {product,-42} {product.Quantity,3} pcs  (low at {product.LowStockThreshold})");
            }

            text.AppendLine();
            text.AppendLine($"OUT OF STOCK ({OutOfStockProducts.Count})");

            if (OutOfStockProducts.Count == 0)
            {
                text.AppendLine("  None");
            }

            foreach (Product product in OutOfStockProducts)
            {
                text.AppendLine($"  {product}");
            }

            return text.ToString();
        }

        // "  (= 850.00 EUR)" when another currency is selected, otherwise nothing
        private static string InCurrency(CurrencyConverter? converter, decimal amountSek)
        {
            if (converter == null || converter.Selected == Currency.SEK)
            {
                return string.Empty;
            }

            return $"  (= {converter.Format(amountSek)})";
        }

        private static string FormatMoney(decimal amount)
            => $"{amount.ToString("F2", CultureInfo.InvariantCulture)} {Product.BaseCurrency}";
    }
}