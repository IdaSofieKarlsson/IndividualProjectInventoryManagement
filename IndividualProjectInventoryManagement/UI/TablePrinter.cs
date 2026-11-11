using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IndividualProjectInventoryManagement.UI
{
    // ------------------------------------------------------------------
    // TablePrinter - prints a list of products as a table
    // ------------------------------------------------------------------
    // The width of every column is measured from the actual content, so long
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
            { "ID", "Category", "Brand", "Name", "Material", "Type", "Size", "Length", "Qty", "Price", "Status" };

        // Numbers line up best when right-aligned
        private static readonly bool[] RightAligned =
            { true, false, false, false, false, false, true, true, true, true, false };

        public static void Print(IReadOnlyList<Product> products)
        {
            if (products.Count == 0)
            {
                Display.Warning("No products found.");
                return;
            }

            // Turn every product into one row of text cells
            List<string[]> rows = products.Select(ToRow).ToList();

            // Column width = the longest of the header and all its cells
            int[] widths = Headers
                .Select((header, column) => Math.Max(header.Length, rows.Max(row => row[column].Length)))
                .ToArray();

            WriteRow(Headers, widths);
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
            Console.WriteLine($"{products.Count} product(s). Prices in {Product.BaseCurrency}.");
        }

        private static string[] ToRow(Product product) => new[]
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
            product.Price.ToString("F2", CultureInfo.InvariantCulture),
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
