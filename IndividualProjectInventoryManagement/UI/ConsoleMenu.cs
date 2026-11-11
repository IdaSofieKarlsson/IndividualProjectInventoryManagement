using System.Globalization;
using System.Text;
using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Services;

namespace IndividualProjectInventoryManagement.UI
{
    // ------------------------------------------------------------------
    // ConsoleMenu - the main loop of the program
    // ------------------------------------------------------------------
    // This class only does three things: show a menu, read a choice, and
    // ask Inventory to do the work. It contains NO business rules.
    // (Which sizes are allowed, whether a product is a duplicate, how
    // totals are calculated: that all lives in Models, Rules and Services.)
    //
    // The menu gets its Inventory through the constructor. It does not
    // create one itself. That is called dependency injection, and it means
    // Program.cs decides which inventory the menu works on.
    // ------------------------------------------------------------------
    public class ConsoleMenu
    {
        private readonly Inventory inventory;

        public ConsoleMenu(Inventory inventory)
        {
            this.inventory = inventory;
        }

        // Runs until the user chooses 0
        public void Run()
        {
            while (true)
            {
                ShowMainMenu();

                int choice = ConsoleInput.ReadChoice(8);

                if (choice == 0)
                {
                    Console.WriteLine("Goodbye!");
                    return;
                }

                try
                {
                    RunAction(choice);
                }
                // A rule was broken (duplicate product, too much removed from
                // stock, invalid value). Show the message and go back to the
                // menu instead of crashing.
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    Display.Error(ex.Message);
                }
                // The change was made in memory, but writing the file failed
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    Display.Error($"The change could not be saved to the file: {ex.Message}");
                }

                // Browse has its own screen loop and no need for a pause
                if (choice != 1)
                {
                    ConsoleInput.Pause();
                }
            }
        }

        private void ShowMainMenu()
        {
            Console.Clear();
            Display.Header("KNAPPEN'S YARN STORE");
            Console.WriteLine($"{inventory.Count} products in the inventory");
            Console.WriteLine($"Saved automatically to: {inventory.StorageLocation}");

            int needAttention = inventory.ViewProducts(new ProductFilter(NeedsAttentionOnly: true)).Count;

            if (needAttention > 0)
            {
                Display.Warning($"{needAttention} product(s) need attention (low or out of stock)");
            }

            Console.WriteLine();
            Console.WriteLine("  1. Browse products (filter, search, sort)");
            Console.WriteLine("  2. Show product details");
            Console.WriteLine("  3. Add product");
            Console.WriteLine("  4. Update product (stock, price, details)");
            Console.WriteLine("  5. Delete product");
            Console.WriteLine("  6. Inventory report");
            Console.WriteLine("  7. Products that need attention");
            Console.WriteLine("  8. Sorted by total value");
            Console.WriteLine("  0. Exit");
            Console.WriteLine();
        }

        private void RunAction(int choice)
        {
            switch (choice)
            {
                case 1: Browse(); break;
                case 2: ShowDetails(); break;
                case 3: AddProduct(); break;
                case 4: UpdateProduct(); break;
                case 5: DeleteProduct(); break;
                case 6: ShowReport(); break;
                case 7: ShowNeedsAttention(); break;
                case 8: ShowByValue(); break;
            }
        }

        // ==============================================================
        // 1. Browse: filter, search and sort
        // ==============================================================
        // The current choices are kept in three variables. After every
        // change the screen is drawn again with the new result.
        private void Browse()
        {
            ProductFilter filter = new ProductFilter();
            SortField sortBy = SortField.Id;
            bool descending = false;

            while (true)
            {
                Console.Clear();
                Display.Header("BROWSE PRODUCTS");

                Console.WriteLine(
                    $"Filter: category = {filter.Category?.ToDisplayText() ?? "all"}, " +
                    $"needs attention only = {(filter.NeedsAttentionOnly ? "yes" : "no")}, " +
                    $"search = {(string.IsNullOrWhiteSpace(filter.SearchText) ? "-" : $"\"{filter.SearchText}\"")}");
                Console.WriteLine($"Sort:   {sortBy.ToDisplayText()} ({(descending ? "descending" : "ascending")})");
                Console.WriteLine();

                TablePrinter.Print(inventory.ViewProducts(filter, sortBy, descending));

                Console.WriteLine();
                Console.WriteLine("  1. Filter by category");
                Console.WriteLine("  2. Show only products that need attention (on/off)");
                Console.WriteLine("  3. Search by text (Enter = clear)");
                Console.WriteLine("  4. Sort by...");
                Console.WriteLine("  5. Reverse sort direction");
                Console.WriteLine("  6. Clear all filters and sorting");
                Console.WriteLine("  0. Back");
                Console.WriteLine();

                switch (ConsoleInput.ReadChoice(6))
                {
                    case 0:
                        return;

                    case 1:
                        if (ConsoleInput.TryPick("Category", Enum.GetValues<ProductCategory>(),
                                item => item.ToDisplayText(), out ProductCategory category))
                        {
                            // "with" copies a record and changes only the named part
                            filter = filter with { Category = category };
                        }
                        else
                        {
                            // Cancel in this list means "show all categories"
                            filter = filter with { Category = null };
                        }
                        break;

                    case 2:
                        filter = filter with { NeedsAttentionOnly = !filter.NeedsAttentionOnly };
                        break;

                    case 3:
                        string text = ConsoleInput.ReadText("Search (for example \"merino\" or \"bamboo 2.5\")");
                        filter = filter with { SearchText = text.Length == 0 ? null : text };
                        break;

                    case 4:
                        if (ConsoleInput.TryPick("Sort by", Enum.GetValues<SortField>(),
                                field => field.ToDisplayText(), out SortField chosen))
                        {
                            sortBy = chosen;
                        }
                        break;

                    case 5:
                        descending = !descending;
                        break;

                    case 6:
                        filter = new ProductFilter();
                        sortBy = SortField.Id;
                        descending = false;
                        break;
                }
            }
        }

        // ==============================================================
        // 2. Details
        // ==============================================================
        private void ShowDetails()
        {
            Display.Header("PRODUCT DETAILS");
            Product? product = AskForProduct();

            if (product != null)
            {
                Console.WriteLine();
                Console.WriteLine(product.DisplayProductInfo());
            }
        }

        // ==============================================================
        // 3. Add
        // ==============================================================
        private void AddProduct()
        {
            Display.Header("ADD PRODUCT");

            if (!ConsoleInput.TryPick("What kind of product?", Enum.GetValues<ProductCategory>(),
                    category => category.ToDisplayText(), out ProductCategory chosenCategory))
            {
                Console.WriteLine("Cancelled.");
                return;
            }

            Product? product = ProductBuilder.Create(chosenCategory);

            if (product == null)
            {
                Console.WriteLine("Cancelled.");
                return;
            }

            // Throws if the same article already exists. Run() shows the message.
            inventory.AddProduct(product);
            Display.Success($"Added {product}.");
        }

        // ==============================================================
        // 4. Update
        // ==============================================================
        private void UpdateProduct()
        {
            Display.Header("UPDATE PRODUCT");
            Product? product = AskForProduct();

            if (product == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine(product.DisplayProductInfo());
            Console.WriteLine();
            Console.WriteLine("  1. Add or remove stock (delivery / sale)");
            Console.WriteLine("  2. Set exact stock (after counting)");
            Console.WriteLine("  3. Change price");
            Console.WriteLine("  4. Change other details (brand, name, size...)");
            Console.WriteLine("  0. Back");
            Console.WriteLine();

            switch (ConsoleInput.ReadChoice(4))
            {
                case 1:
                    int? amount = ConsoleInput.ReadInt("Amount (positive = delivery, negative = sale)", -100000, 100000);

                    if (amount != null)
                    {
                        inventory.AdjustStock(product.Id, amount.Value);
                        Display.Success($"{product} now has {product.Quantity} pcs.");
                        WarnIfLow(product);
                    }
                    break;

                case 2:
                    int? stock = ConsoleInput.ReadInt("New stock", 0, 100000);

                    if (stock != null)
                    {
                        inventory.SetStock(product.Id, stock.Value);
                        Display.Success($"{product} now has {product.Quantity} pcs.");
                        WarnIfLow(product);
                    }
                    break;

                case 3:
                    decimal? price = ConsoleInput.ReadDecimal($"New price in {Product.BaseCurrency}", 0.01m, 100000m);

                    if (price != null)
                    {
                        inventory.SetPrice(product.Id, price.Value);
                        Display.Success($"{product} now costs {product.Price.ToString("F2", CultureInfo.InvariantCulture)} {Product.BaseCurrency}.");
                    }
                    break;

                case 4:
                    Console.WriteLine();
                    Console.WriteLine("Enter the new details. The ID stays the same.");

                    // The product's identity (brand, name, size...) cannot be
                    // edited, so we build a new one and let Inventory swap it in.
                    Product? replacement = ProductBuilder.Create(product.Category);

                    if (replacement == null)
                    {
                        Console.WriteLine("Cancelled.");
                    }
                    else if (inventory.UpdateProduct(product.Id, replacement))
                    {
                        Display.Success($"Updated {replacement}.");
                    }
                    break;
            }
        }

        // ==============================================================
        // 5. Delete
        // ==============================================================
        private void DeleteProduct()
        {
            Display.Header("DELETE PRODUCT");
            Product? product = AskForProduct();

            if (product == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine(product.DisplayProductInfo());
            Console.WriteLine();

            if (ConsoleInput.Confirm($"Really delete {product}?"))
            {
                inventory.DeleteProduct(product.Id);
                Display.Success("Deleted.");
            }
            else
            {
                Console.WriteLine("Nothing was deleted.");
            }
        }

        // ==============================================================
        // 6. Report
        // ==============================================================
        private void ShowReport()
        {
            InventoryReport report = inventory.GenerateReport();

            Console.Clear();
            Console.WriteLine(report.ToText());

            if (ConsoleInput.Confirm("Save the report to a text file?"))
            {
                SaveReport(report);
            }
        }

        private static void SaveReport(InventoryReport report)
        {
            string fileName = $"inventory-report-{report.GeneratedAt.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture)}.txt";

            // AppContext.BaseDirectory = the folder where the program runs
            // (bin/Debug/net8.0 when started from Visual Studio)
            string path = Path.Combine(AppContext.BaseDirectory, fileName);

            try
            {
                File.WriteAllText(path, report.ToText(), Encoding.UTF8);
                Display.Success($"Report saved to: {path}");
            }
            // Files can fail for reasons we do not control: a full disk,
            // no permission. Tell the user and carry on.
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Display.Error($"Could not save the report: {ex.Message}");
            }
        }

        // ==============================================================
        // 7 and 8. Ready-made views
        // ==============================================================
        private void ShowNeedsAttention()
        {
            Display.Header("PRODUCTS THAT NEED ATTENTION");

            // Status descending: out of stock first, then low stock
            TablePrinter.Print(inventory.ViewProducts(
                new ProductFilter(NeedsAttentionOnly: true), SortField.Status, descending: true));
        }

        private void ShowByValue()
        {
            Display.Header("PRODUCTS BY TOTAL VALUE (HIGHEST FIRST)");
            TablePrinter.Print(inventory.ViewProducts(sortBy: SortField.TotalValue, descending: true));
        }

        // ==============================================================
        // Helpers
        // ==============================================================

        // Asks for an ID and finds the product. Returns null (after telling
        // the user why) if the input was cancelled or no product has that ID.
        private Product? AskForProduct()
        {
            int? id = ConsoleInput.ReadInt("Product ID", 1, 100000);

            if (id == null)
            {
                return null;
            }

            Product? product = inventory.FindById(id.Value);

            if (product == null)
            {
                Display.Error($"There is no product with ID {id.Value}.");
            }

            return product;
        }

        private static void WarnIfLow(Product product)
        {
            switch (product.GetStockStatus())
            {
                case StockStatus.OutOfStock:
                    Display.Error("This product is now out of stock.");
                    break;

                case StockStatus.LowStock:
                    Display.Warning($"Low stock: {product.Quantity} left (low at {product.LowStockThreshold}).");
                    break;
            }
        }
    }
}