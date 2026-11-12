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
        private readonly CurrencyConverter currencyConverter;

        public ConsoleMenu(Inventory inventory, CurrencyConverter currencyConverter)
        {
            this.inventory = inventory;
            this.currencyConverter = currencyConverter;
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
            Console.WriteLine($"Prices shown in {currencyConverter.Selected} ({currencyConverter.RateInfo})");

            int needAttention = inventory.ViewProducts(new ProductFilter(NeedsAttentionOnly: true)).Count;

            if (needAttention > 0)
            {
                Display.Warning($"{needAttention} product(s) need attention (low or out of stock)");
            }

            Console.WriteLine();
            Console.WriteLine("  1. Browse products (filter, search, sort)");
            Console.WriteLine("  2. Present products");
            Console.WriteLine("  3. Show product details");
            Console.WriteLine("  4. Add product");
            Console.WriteLine("  5. Update product (stock, price, description, details)");
            Console.WriteLine("  6. Delete product");
            Console.WriteLine("  7. Inventory report");
            Console.WriteLine("  8. Change display currency");
            Console.WriteLine("  0. Exit");
            Console.WriteLine();
        }

        private void RunAction(int choice)
        {
            switch (choice)
            {
                case 1: Browse(); break;
                case 2: PresentProducts(); break;
                case 3: ShowDetails(); break;
                case 4: AddProduct(); break;
                case 5: UpdateProduct(); break;
                case 6: DeleteProduct(); break;
                case 7: ShowReport(); break;
                case 8: ChangeCurrency(); break;
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

                PrintTable(inventory.ViewProducts(filter, sortBy, descending));

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
        // 2. Present products (a customer-friendly list)
        // ==============================================================
        // One line per product:  Brand - Name - Description - Selling price
        // The price is shown in the currency chosen in the menu.
        private void PresentProducts()
        {
            Display.Header("PRESENT PRODUCTS");

            // One heading per category, and the brands sorted within it
            foreach (ProductCategory category in Enum.GetValues<ProductCategory>())
            {
                List<Product> products = inventory.ViewProducts(new ProductFilter(Category: category), SortField.Brand);

                if (products.Count == 0)
                {
                    continue;
                }

                Console.WriteLine();
                Display.Coloured(category.ToDisplayText().ToUpperInvariant(), ConsoleColor.Cyan);

                foreach (Product product in products)
                {
                    Console.WriteLine(
                        $"{product.BrandText} - {product.NameText} - {product.ProductDescription} - {currencyConverter.Format(product.SellingPrice)}");
                }
            }
        }

        // ==============================================================
        // 3. Details
        // ==============================================================
        private void ShowDetails()
        {
            Display.Header("PRODUCT DETAILS");
            Product? product = AskForProduct();

            if (product != null)
            {
                Console.WriteLine();
                Console.WriteLine(product.DisplayProductInfo());

                if (currencyConverter.Selected != Currency.SEK)
                {
                    Console.WriteLine($"  In {currencyConverter.Selected,-5}  : purchase {currencyConverter.Format(product.PurchasePrice)}, selling {currencyConverter.Format(product.SellingPrice)}");
                }
            }
        }

        // ==============================================================
        // 4. Add
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
        // 5. Update
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
            Console.WriteLine("  3. Change purchase price (selling price follows)");
            Console.WriteLine("  4. Change description");
            Console.WriteLine("  5. Change other details (brand, name, size...)");
            Console.WriteLine("  0. Back");
            Console.WriteLine();

            switch (ConsoleInput.ReadChoice(5))
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
                    decimal? price = ConsoleInput.ReadDecimal($"New purchase price in {Product.BaseCurrency}", 1m, 100000m);

                    if (price != null)
                    {
                        inventory.SetPurchasePrice(product.Id, price.Value);
                        Display.Success($"{product}: purchase price {price.Value.ToString("0.##", CultureInfo.InvariantCulture)} {Product.BaseCurrency}, selling price {product.SellingPrice} {Product.BaseCurrency}.");
                    }
                    break;

                case 4:
                    string? description = ProductBuilder.AskForDescription();

                    if (description != null)
                    {
                        inventory.SetDescription(product.Id, description);
                        Display.Success("The description was updated.");
                    }
                    break;

                case 5:
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
        // 6. Delete
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
        // 7. Report
        // ==============================================================
        private void ShowReport()
        {
            InventoryReport report = inventory.GenerateReport();

            Console.Clear();
            Console.WriteLine(report.ToText(currencyConverter));

            if (ConsoleInput.Confirm("Save the report to a text file?"))
            {
                SaveReport(report, currencyConverter);
            }
        }

        private static void SaveReport(InventoryReport report, CurrencyConverter converter)
        {
            string fileName = $"inventory-report-{report.GeneratedAt.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture)}.txt";

            // AppContext.BaseDirectory = the folder where the program runs
            // (bin/Debug/net8.0 when started from Visual Studio)
            string path = Path.Combine(AppContext.BaseDirectory, fileName);

            try
            {
                File.WriteAllText(path, report.ToText(converter), Encoding.UTF8);
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
        // 8. Display currency
        // ==============================================================
        // Only changes what is SHOWN. Prices are always stored in SEK.
        private void ChangeCurrency()
        {
            Display.Header("DISPLAY CURRENCY");
            Console.WriteLine($"Rates: {currencyConverter.RateInfo}");

            if (!currencyConverter.IsUsingLiveRates && currencyConverter.ErrorMessage != null)
            {
                Display.Warning(currencyConverter.ErrorMessage);
            }

            if (ConsoleInput.TryPick("Show prices in", Enum.GetValues<Currency>(),
                    item => item == Currency.SEK
                        ? "SEK (the shop's own currency)"
                        : $"{item} (1 SEK = {currencyConverter.GetRate(item).ToString("0.####", CultureInfo.InvariantCulture)} {item})",
                    out Currency chosen))
            {
                currencyConverter.Selected = chosen;
                Display.Success($"Prices are now shown in {chosen}.");
            }
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

        private void PrintTable(IReadOnlyList<Product> products) => TablePrinter.Print(products, currencyConverter);

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