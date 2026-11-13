using IndividualProjectInventoryManagement.Models.Enums;
using IndividualProjectInventoryManagement.Rules;

namespace IndividualProjectInventoryManagement.Models
{
    // An OtherSupply IS a Product (inheritance): stitch markers, measuring tape, needle gauges and similar small things.
    // Size = pieces per pack. There is no length, so null goes to the base class.
    public class OtherSupply : Product
    {
        public OtherSupplyBrand Brand { get; }
        public OtherSupplyName Name { get; }
        public OtherSupplyMaterial Material { get; }

        public override string BrandText => Brand.ToDisplayText();
        public override string NameText => Name.ToDisplayText();
        public override string MaterialText => Material.ToDisplayText();

        // No LowStockThreshold override: this category keeps the default of 5.

        public OtherSupply(
            OtherSupplyBrand brand,
            OtherSupplyName name,
            OtherSupplyMaterial material,
            decimal size,
            int quantity,
            decimal purchasePrice,
            string productDescription)
            // The length is "not applicable", so we pass null
            : base(ProductCategory.OtherSupply, size, null, quantity, purchasePrice, productDescription)
        {
            if (!BrandCatalog.IsValidCombination(brand, name))
            {
                throw new ArgumentException($"{brand.ToDisplayText()} does not sell {name.ToDisplayText()}.");
            }

            Brand = brand;
            Name = name;
            Material = material;
        }
    }
}