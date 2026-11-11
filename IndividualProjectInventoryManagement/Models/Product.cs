using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IndividualProjectInventoryManagement.Models
{
    // ------------------------------------------------------------------
    // Product - ABSTRACT base class for everything the shop sells
    // ------------------------------------------------------------------
    // Same idea as Asset in the Asset Tracking project:
    //   - Nobody creates a plain Product with "new Product(...)". The shop
    //     sells yarn, needles, hooks and supplies, never just "a product".
    //   - Everything the four categories share is written ONCE, here.
    //   - Yarn, Needle, Hook and OtherSupply inherit it and add what is
    //     special about them.
    //
    // THE BIG DIFFERENCE FROM ASSET: brand, name and material are different
    // enums per category (a Yarn has a YarnBrand, a Hook has a HookBrand),
    // so they cannot be declared here. Instead:
    //   - each subclass has its own TYPED properties (Brand, Name, Material)
    //   - this class declares ABSTRACT text versions (BrandText, NameText,
    //     MaterialText) that every subclass must provide
    // Code that handles "any product", such as tables, sorting and search,
    // uses the text versions. Code that needs the exact enum asks the subclass.
    // ------------------------------------------------------------------
    public abstract class Product
    {
        // The shop's prices are stored in SEK. Currency conversion comes later.
        public const string BaseCurrency = "SEK";

        // --------------------------------------------------------------
        // Properties that are SET ONCE and never change
        // --------------------------------------------------------------
        // Brand, name, material, size and length together say WHICH product
        // this is. A 3.5 mm needle is a different product from a 4.0 mm
        // needle, so these cannot be edited afterwards. Only quantity and
        // price change over time. (To "edit" the rest, Inventory will later
        // replace the product with a new one that keeps the same Id.)
        // "{ get; }" = read-only: it can only be set in the constructor.

        // Assigned by Inventory when the product is added (0 = not added yet)
        public int Id { get; set; }

        public ProductCategory Category { get; }

        // "decimal?" = may be null. A null length means "not applicable"
        // (other supplies have no length).
        public decimal? Size { get; }
        public decimal? Length { get; }

        // --------------------------------------------------------------
        // Properties that CAN change, with protection
        // --------------------------------------------------------------
        // A property with its own set code checks the value BEFORE storing it.
        // The real value lives in a private "backing field". This makes it
        // impossible to end up with a negative quantity, whoever sets it.
        private int quantity;
        private decimal price;

        public int Quantity
        {
            get => quantity;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Quantity cannot be negative.");
                }

                quantity = value;
            }
        }

        // Price per piece, in SEK
        public decimal Price
        {
            get => price;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Price must be above 0.");
                }

                price = value;
            }
        }

        // --------------------------------------------------------------
        // ABSTRACT members: no code here, every subclass MUST write them
        // --------------------------------------------------------------
        // If a subclass forgets one, the code does not compile.
        public abstract string BrandText { get; }
        public abstract string NameText { get; }
        public abstract string MaterialText { get; }

        // --------------------------------------------------------------
        // VIRTUAL members: a default here, a subclass MAY change it
        // --------------------------------------------------------------
        // Extra information that only some categories have. Needle overrides
        // it to say "Circular"; the others keep the default (null = nothing).
        public virtual string? ExtraText => null;

        // At or below this quantity the product counts as LOW STOCK.
        // Yarn sells fast, so it overrides this with a higher number.
        public virtual int LowStockThreshold => 5;

        // --------------------------------------------------------------
        // Calculated properties: never stored, worked out when read
        // --------------------------------------------------------------
        // The value of everything in stock of this product
        public decimal TotalValue => Quantity * Price;

        // "4.0 mm", "50 g" or "-" when not applicable
        public string SizeText
        {
            get
            {
                if (Size is null)
                {
                    return "-";
                }

                CategoryRules rules = ProductRules.For(Category);

                // "F1" = fixed-point with 1 decimal ("F0" = none)
                string number = Size.Value.ToString("F" + rules.SizeDecimals, CultureInfo.InvariantCulture);
                return $"{number} {rules.SizeUnit}";
            }
        }

        // "80 cm", "200 m" or "-" when not applicable
        public string LengthText
        {
            get
            {
                if (Length is null)
                {
                    return "-";
                }

                string number = Length.Value.ToString("F0", CultureInfo.InvariantCulture);
                return $"{number} {ProductRules.For(Category).LengthUnit}";
            }
        }

        // --------------------------------------------------------------
        // Constructor
        // --------------------------------------------------------------
        // "protected" = only subclasses can call it (another sign that
        // Product is abstract). Each subclass passes its own category.
        //
        // We check size and length against ProductRules here, once, for
        // all categories. A product with an invalid size can never exist.
        protected Product(ProductCategory category, decimal? size, decimal? length, int quantity, decimal price)
        {
            if (!ProductRules.IsValidSize(category, size))
            {
                throw new ArgumentException($"{size?.ToString(CultureInfo.InvariantCulture) ?? "(none)"} is not an allowed size for {category.ToDisplayText()}.");
            }

            if (!ProductRules.IsValidLength(category, length))
            {
                throw new ArgumentException($"{length?.ToString(CultureInfo.InvariantCulture) ?? "(none)"} is not an allowed length for {category.ToDisplayText()}.");
            }

            Category = category;
            Size = size;
            Length = length;

            // These go through the properties above, so the checks for
            // quantity and price run here too.
            Quantity = quantity;
            Price = price;
        }

        // --------------------------------------------------------------
        // Methods
        // --------------------------------------------------------------

        // Red / yellow / normal, the stock version of the old lifecycle colours
        public StockStatus GetStockStatus()
        {
            // Check OutOfStock first: 0 is also "at or below" the threshold
            if (Quantity == 0)
            {
                return StockStatus.OutOfStock;
            }

            if (Quantity <= LowStockThreshold)
            {
                return StockStatus.LowStock;
            }

            return StockStatus.InStock;
        }

        // Adds to or removes from stock: +10 for a delivery, -3 for a sale
        public void AdjustQuantity(int amount)
        {
            if (Quantity + amount < 0)
            {
                throw new InvalidOperationException(
                    $"Cannot remove {-amount} pieces, only {Quantity} in stock.");
            }

            Quantity += amount;
        }

        // The method name comes from the course spec. It RETURNS the text
        // instead of printing it, so Product knows nothing about the console.
        // The menu decides how and where to show it (separation of concerns).
        public string DisplayProductInfo()
        {
            List<string> lines = new List<string>
            {
                $"#{Id}  {Category.ToDisplayText()}: {BrandText} {NameText}",
                $"  Material : {MaterialText}",
                $"  Size     : {SizeText}"
            };

            // Only show the lines that apply to this product
            if (Length != null)
            {
                lines.Add($"  Length   : {LengthText}");
            }

            if (ExtraText != null)
            {
                lines.Add($"  Type     : {ExtraText}");
            }

            lines.Add($"  In stock : {Quantity} pcs ({GetStockStatus().ToDisplayText()})");
            lines.Add($"  Price    : {FormatMoney(Price)}");
            lines.Add($"  Value    : {FormatMoney(TotalValue)}");

            return string.Join(Environment.NewLine, lines);
        }

        // Replaces ToString() so that printing a product gives something useful
        public override string ToString() => $"#{Id} {Category.ToDisplayText()} {BrandText} {NameText}";

        private static string FormatMoney(decimal amount)
            => $"{amount.ToString("F2", CultureInfo.InvariantCulture)} {BaseCurrency}";
    }
}
