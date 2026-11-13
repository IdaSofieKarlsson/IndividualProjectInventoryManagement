using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;

namespace IndividualProjectInventoryManagement.Models
{
    // A Yarn IS a Product (inheritance), so it gets all the Product code.
    // Yarn is the simplest product: just a brand and a name. Every ball is assumed to weigh 50 g, so there is no size,
    // no length and no material to choose.
    // The base class gets null for size and length, which ProductRules accepts for yarn ("not applicable").
    public class Yarn : Product
    {
        // The TYPED values: a Yarn can only have a YarnBrand and a YarnName.
        // The compiler enforces it.
        public YarnBrand Brand { get; }
        public YarnName Name { get; }

        // "override" fills in the abstract members from Product.
        // Here the typed enum values are turned into display text ("Jarbo" -> "Järbo").
        public override string BrandText => Brand.ToDisplayText();
        public override string NameText => Name.ToDisplayText();

        // A yarn has no material in this shop. Product.NoValue is "-".
        public override string MaterialText => NoValue;

        // Yarn sells fast, so it warns earlier than the default of 5
        public override int LowStockThreshold => 10;

        public Yarn(
            YarnBrand brand,
            YarnName name,
            int quantity,
            decimal purchasePrice,
            string productDescription)
            // ": base(...)" runs the Product constructor first. It checks
            // quantity, price and description (size and length are null).
            : base(ProductCategory.Yarn, null, null, quantity, purchasePrice, productDescription)
        {
            // Then the check only a Yarn can do: does this brand sell this yarn?
            if (!BrandCatalog.IsValidCombination(brand, name))
            {
                throw new ArgumentException($"{brand.ToDisplayText()} does not sell a yarn called {name.ToDisplayText()}.");
            }

            Brand = brand;
            Name = name;
        }
    }
}