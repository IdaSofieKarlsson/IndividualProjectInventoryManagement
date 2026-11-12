using System.Globalization;
using System.Text;
using IndividualProjectInventoryManagement.Data;
using IndividualProjectInventoryManagement.Models;
using IndividualProjectInventoryManagement.Models.Enums;

namespace IndividualProjectInventoryManagement.Services
{
    // ------------------------------------------------------------------
    // Inventory - owns the list of products
    // ------------------------------------------------------------------
    // This is the Asset Tracking "AssetTracker" grown up. ONE class is
    // responsible for the data: adding, updating, deleting, finding,
    // filtering, sorting and reporting. The menu only ASKS Inventory
    // for things and never touches the list directly (encapsulation).
    //
    // The method names AddProduct, UpdateProduct, DeleteProduct,
    // ViewProducts and GenerateReport come from the course spec.
    //
    // How problems are reported:
    //   - "No product with that ID"  -> the method returns false / null
    //     (that is normal: the user typed a wrong ID)
    //   - "This breaks a rule"       -> the method throws an exception
    //     (a duplicate product, or removing more than is in stock)
    //
    // Storage: the list lives in memory while the program runs, and every
    // method that changes something ends with Save() (autosave). Inventory
    // only knows the IProductRepository contract, not that it is a JSON file.
    // That is why all changes must go THROUGH Inventory: a change made
    // directly on a product would never be saved.
    // ------------------------------------------------------------------
    public class Inventory
    {
        // "private" = no code outside this class can reach the list.
        // It holds Yarn, Needle, Hook and OtherSupply objects side by side,
        // because they are all Products.
        private readonly List<Product> products = new List<Product>();

        // The ID that the next new product receives. It only counts upwards.
        private int nextId = 1;

        // The storage. It is handed in (injected) from Program.cs.
        private readonly IProductRepository repository;

        public Inventory(IProductRepository repository)
        {
            this.repository = repository;
        }

        public int Count => products.Count;

        // Where the data is stored, for display in the menu
        public string StorageLocation => repository.Location;

        // ==============================================================
        // LOAD AND SAVE
        // ==============================================================
        // Reads the saved products. Broken entries are skipped and reported
        // as warnings. Nothing is written back until the next change.
        public LoadOutcome Load()
        {
            LoadResult result = repository.Load();

            products.Clear();
            nextId = 1;

            List<string> warnings = new List<string>(result.Warnings);

            foreach (Product product in result.Products)
            {
                try
                {
                    AddWithoutSaving(product);
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    warnings.Add($"Skipped {product}: {ex.Message}");
                }
            }

            // Never go below the saved counter, so deleted IDs are not reused
            nextId = Math.Max(nextId, result.NextId);

            return new LoadOutcome(result.FileFound, warnings);
        }

        private void Save()
        {
            repository.Save(products, nextId);
        }

        // ==============================================================
        // CREATE
        // ==============================================================
        // Adds a product and gives it an ID. Returns the product, so the
        // caller can show "Added #14 ...".
        public Product AddProduct(Product product)
        {
            Product added = AddWithoutSaving(product);
            Save();
            return added;
        }

        // Adds many products and saves ONCE at the end (used for the sample data)
        public void AddProducts(IEnumerable<Product> newProducts)
        {
            foreach (Product product in newProducts)
            {
                AddWithoutSaving(product);
            }

            Save();
        }

        // The real work of adding. Load() uses it too, and must not save.
        private Product AddWithoutSaving(Product product)
        {
            if (product.Id < 0)
            {
                throw new ArgumentException("A product ID cannot be negative.");
            }

            // The same article must not appear twice. Add stock to the
            // existing product instead.
            EnsureNotDuplicate(product, ignoreId: null);

            if (product.Id == 0)
            {
                // A brand new product: hand out the next free ID
                // ("nextId++" uses the current value FIRST, then adds 1)
                product.Id = nextId++;
            }
            else
            {
                // The product already HAS an ID (it will come from the JSON
                // file later). Keep it, but check that it is free, and make
                // sure new products never receive an ID at or below it.
                if (products.Any(existing => existing.Id == product.Id))
                {
                    throw new InvalidOperationException($"There is already a product with ID {product.Id}.");
                }

                nextId = Math.Max(nextId, product.Id + 1);
            }

            products.Add(product);
            return product;
        }

        // ==============================================================
        // READ
        // ==============================================================
        // Finds one product by ID, or returns null if there is none.
        // FirstOrDefault (LINQ) returns the first match, or null if nothing matches.
        public Product? FindById(int id)
        {
            return products.FirstOrDefault(product => product.Id == id);
        }

        // Returns the products the user wants to see, filtered and sorted.
        //
        // The parameters have default values, so all of these work:
        //     ViewProducts()                                      everything, in ID order
        //     ViewProducts(new ProductFilter(Category: ProductCategory.Yarn))
        //     ViewProducts(sortBy: SortField.SellingPrice, descending: true)
        //
        // It returns a NEW list. The private list itself is never handed out,
        // so nobody outside can add or remove items behind our back.
        public List<Product> ViewProducts(
            ProductFilter? filter = null,
            SortField sortBy = SortField.Id,
            bool descending = false)
        {
            // "??=" means: if filter is null, use an empty filter
            // (= no filtering) instead.
            filter ??= new ProductFilter();

            // A LINQ query is built up step by step. Nothing is calculated
            // until ToList() at the end asks for the result.
            IEnumerable<Product> query = products;

            if (filter.Category != null)
            {
                query = query.Where(product => product.Category == filter.Category);
            }

            if (filter.NeedsAttentionOnly)
            {
                query = query.Where(product => product.GetStockStatus() != StockStatus.InStock);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                // Split "järbo merino" into the words "jarbo" and "merino"
                string[] terms = Normalise(filter.SearchText)
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // A product matches when EVERY word is found somewhere in its text
                query = query.Where(product => MatchesAllTerms(product, terms));
            }

            // Sorting. The ID is the final tie-breaker, so two products with
            // the same price always come out in the same order.
            Func<Product, IComparable> sortKey = GetSortKey(sortBy);

            IOrderedEnumerable<Product> sorted = descending
                ? query.OrderByDescending(sortKey)
                : query.OrderBy(sortKey);

            return sorted.ThenBy(product => product.Id).ToList();
        }

        // ==============================================================
        // UPDATE
        // ==============================================================
        // Replaces a product with a new one, keeping the ID and the
        // position in the list. This is how brand, name, material, size
        // and length are "edited": they cannot be changed on an existing
        // product, so the menu builds a new product from the user's new
        // choices and hands it in here. The constructor has already
        // checked every rule before we get here.
        // Returns false if there is no product with that ID.
        public bool UpdateProduct(int id, Product replacement)
        {
            // FindIndex returns the position in the list, or -1 if not found
            int index = products.FindIndex(product => product.Id == id);

            if (index < 0)
            {
                return false;
            }

            // The new version must not collide with ANOTHER product
            // (it is allowed to look like the one it replaces)
            EnsureNotDuplicate(replacement, ignoreId: id);

            replacement.Id = id;
            products[index] = replacement;
            Save();
            return true;
        }

        // Adds to or removes from the stock: +10 for a delivery, -3 for a sale.
        // Throws if more is removed than there is.
        public bool AdjustStock(int id, int amount)
        {
            Product? product = FindById(id);

            if (product is null)
            {
                return false;
            }

            product.AdjustQuantity(amount);
            Save();
            return true;
        }

        // Sets the stock to an exact number, for example after counting the shelves
        public bool SetStock(int id, int quantity)
        {
            Product? product = FindById(id);

            if (product is null)
            {
                return false;
            }

            product.Quantity = quantity;
            Save();
            return true;
        }

        // The selling price follows automatically (purchase price x 3)
        public bool SetPurchasePrice(int id, decimal purchasePrice)
        {
            Product? product = FindById(id);

            if (product is null)
            {
                return false;
            }

            product.PurchasePrice = purchasePrice;
            Save();
            return true;
        }

        public bool SetDescription(int id, string description)
        {
            Product? product = FindById(id);

            if (product is null)
            {
                return false;
            }

            product.ProductDescription = description;
            Save();
            return true;
        }

        // ==============================================================
        // DELETE
        // ==============================================================
        // RemoveAll removes every product matching the condition and returns
        // how many it removed. IDs are unique, so that is 0 or 1.
        public bool DeleteProduct(int id)
        {
            bool removed = products.RemoveAll(product => product.Id == id) > 0;

            if (removed)
            {
                Save();
            }

            return removed;
        }

        // ==============================================================
        // REPORT
        // ==============================================================
        public InventoryReport GenerateReport()
        {
            return new InventoryReport(products, DateTime.Now);
        }

        // ==============================================================
        // Private helpers
        // ==============================================================

        // Throws if another product is the same article as this one
        private void EnsureNotDuplicate(Product candidate, int? ignoreId)
        {
            // "existing.Id != ignoreId" compares an int with an int?.
            // When ignoreId is null this is always true, so nothing is ignored.
            Product? existing = products.FirstOrDefault(
                product => product.IdentityKey == candidate.IdentityKey && product.Id != ignoreId);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    $"{existing} is already in the inventory. Adjust its stock instead.");
            }
        }

        // Does the product contain every search word?
        private static bool MatchesAllTerms(Product product, string[] terms)
        {
            // Everything a customer can SEE about the product, as one text
            string haystack = Normalise(string.Join(
                " ",
                product.Category.ToDisplayText(),
                product.BrandText,
                product.NameText,
                product.MaterialText,
                product.ExtraText,
                product.ProductDescription,
                product.SizeText,
                product.LengthText));

            // All(...) is true only if EVERY word passes the test
            return terms.All(term => haystack.Contains(term));
        }

        // Makes text easy to compare: lower case and without accents, so a
        // search for "jarbo" finds "Järbo" and "SANDNES" finds "Sandnes Garn".
        private static string Normalise(string text)
        {
            // FormD splits "ä" into "a" + a separate accent mark.
            // Then we keep everything except the accent marks.
            string decomposed = text.Normalize(NormalizationForm.FormD);

            IEnumerable<char> withoutAccents = decomposed.Where(
                character => CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark);

            return new string(withoutAccents.ToArray()).ToLowerInvariant();
        }

        // Picks WHAT to sort on. The result is a small function that takes a
        // product and returns the value to compare.
        // IComparable means "can be compared with another of the same kind":
        // numbers, text and enums all can, so one function type covers them all.
        private static Func<Product, IComparable> GetSortKey(SortField field) => field switch
        {
            SortField.Id => product => product.Id,
            SortField.Category => product => product.Category,
            SortField.Brand => product => product.BrandText,
            SortField.Name => product => product.NameText,
            SortField.Material => product => product.MaterialText,
            // "?? 0m" = if there is no size/length, count it as 0
            SortField.Size => product => product.Size ?? 0m,
            SortField.Length => product => product.Length ?? 0m,
            SortField.Quantity => product => product.Quantity,
            SortField.PurchasePrice => product => product.PurchasePrice,
            SortField.SellingPrice => product => product.SellingPrice,
            SortField.StockValue => product => product.StockValue,
            SortField.Status => product => product.GetStockStatus(),
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };
    }
}