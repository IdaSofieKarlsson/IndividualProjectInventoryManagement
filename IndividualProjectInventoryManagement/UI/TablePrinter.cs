using System.Globalization;
using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Services;

namespace IndividualProjectInventoryManagement.UI
{
    // ------------------------------------------------------------------
    // TablePrinter - prints a list of products as a table
    // ------------------------------------------------------------------
    // The same idea as PrintAssetList in the Asset Tracking project: the
    // width of every column is measured from the actual content, so long
    // names never break the layout.
    //
    // Rows are coloured by stock status:
    //   red    = out of stock
    //   yellow = low stock
    //   normal = in stock
    // The last column also says it in text (OK / LOW / OUT), so the table
    // is readable even without colours.
    // ------------------------------------------------------------------
    public static class TablePrinter
    {
        private static readonly string[] Headers =
            { "ID", "Category", "Brand", "Name", "Material", "Type", "Size", "Length", "Qty", "Buy", "Sell", "Status" };

        // Numbers line up best when right-aligned
        private static readonly bool[] RightAligned =
            { true, false, false, false, false, false, true, true, true, true, true, false };

        public static void Print(IReadOnlyList<Product> products, CurrencyConverter converter)
        {
            if (products.Count == 0)
            {
                Display.Warning("No products found.");
                return;
            }

            // Turn every product into one row of text cells
            // The price columns show the chosen currency, e.g. "Buy (EUR)"
            string[] headers = (string[])Headers.Clone();
            headers[9] = $"Buy ({converter.Selected})";
            headers[10] = $"Sell ({converter.Selected})";

            List<string[]> rows = products.Select(product => ToRow(product, converter)).ToList();

            // Column width = the longest of the header and all its cells
            int[] widths = headers
                .Select((header, column) => Math.Max(header.Length, rows.Max(row => row[column].Length)))
                .ToArray();

            WriteRow(headers, widths);
            Console.WriteLine(new string('-', widths.Sum() + 2 * (widths.Length - 1)));

            for (int i = 0; i < products.Count; i++)
            {
                Console.ForegroundColor = products[i].GetStockStatus() switch
                {
                    StockStatus.OutOfStock => ConsoleColor.Red,
                    StockStatus.LowStock => ConsoleColor.Yellow,
                    _ => Console.ForegroundColor
                };

                WriteRow(rows[i], widths);
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine($"{products.Count} product(s). Buy = purchase price, Sell = selling price. Prices in {converter.Selected}.");
        }

        private static string[] ToRow(Product product, CurrencyConverter converter) => new[]
        {
            product.Id.ToString(CultureInfo.InvariantCulture),
            product.Category.ToDisplayText(),
            product.BrandText,
            product.NameText,
            product.MaterialText,
            product.ExtraText ?? "-",
            product.SizeText,
            product.LengthText,
            product.Quantity.ToString(CultureInfo.InvariantCulture),
            converter.FormatNumber(product.PurchasePrice),
            converter.FormatNumber(product.SellingPrice),
            product.GetStockStatus() switch
            {
                StockStatus.OutOfStock => "OUT",
                StockStatus.LowStock => "LOW",
                _ => "OK"
            }
        };

        private static void WriteRow(string[] cells, int[] widths)
        {
            // PadLeft / PadRight add spaces up to the column width
            IEnumerable<string> padded = cells.Select((cell, column) =>
                RightAligned[column] ? cell.PadLeft(widths[column]) : cell.PadRight(widths[column]));

            Console.WriteLine(string.Join("  ", padded));
        }
    }
}